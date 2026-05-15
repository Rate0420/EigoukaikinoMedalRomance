using System;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace EMR.PushPlate
{
    public enum MoveDirection
    {
        PositiveX,
        PositiveY,
        PositiveZ,
        NegativeX,
        NegativeY,
        NegativeZ
    }


    [RequireComponent(typeof(Rigidbody))]
    public class PushPlate2 : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private MoveDirection _moveDirection = MoveDirection.PositiveZ;
        [SerializeField, Min(0f)] private float _moveDistance = 1.0f;
        [SerializeField, Min(0f)] private float _offset = 0.0f;
        [SerializeField, Min(0f)] private float _moveSpeed = 1.0f;
        [SerializeField, Min(0f)] private float _minPointWaitingTime = 0.3f;
        [SerializeField, Min(0f)] private float _maxPointWaitingTime = 0.3f;

        [SerializeField]
        private AnimationCurve _animationCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Header("Debug")]
        [SerializeField] private bool _isActive = true;
        [SerializeField] private float _height = 0f;


        private Rigidbody _rigidbody;

        private Vector3 _startPosition;
        private Vector3 _endPosition;

        private float _t;           // 0 ～ 1
        private int _direction = 1; // 1 = forward, -1 = backward


        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.useGravity = false;
            _rigidbody.isKinematic = true;

            CachePositions();
        }

        private void Start()
        {
            MoveLoopAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }


        private void CachePositions()
        {
            Vector3 direction = GetDirectionVector();

            _startPosition = transform.position + (-direction * _offset);
            _endPosition = _startPosition + direction * _moveDistance;

            float positionRate = _moveDistance > 0f ? Mathf.Clamp01(_offset / _moveDistance) : 0f;

            _t = GetCurveTimeFromValue(positionRate);
        }

        private float GetCurveTimeFromValue(float value)
        {
            if (_animationCurve == null)
                return value;

            value = Mathf.Clamp01(value);

            float low = 0f;
            float high = 1f;

            for (int i = 0; i < 16; i++)
            {
                float mid = (low + high) * 0.5f;
                float evaluated = EvaluateCurve(mid);

                if (evaluated < value)
                    low = mid;
                else
                    high = mid;
            }

            return (low + high) * 0.5f;
        }

        /// <summary>
        /// メイン移動ループ
        /// </summary>
        private async UniTaskVoid MoveLoopAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                // 非アクティブ、または設定不足なら待機
                if (_moveDistance <= 0f && _moveSpeed <= 0f)
                {
                    await UniTask.Yield(PlayerLoopTiming.FixedUpdate, cancellationToken);
                    continue;
                }

                // 現在の方向に応じた移動
                if (_direction > 0)
                {
                    await MoveToAsync(1f, _maxPointWaitingTime, cancellationToken);
                }
                else
                {
                    await MoveToAsync(0f, _minPointWaitingTime, cancellationToken);
                }
            }
        }

        /// <summary>
        /// 指定した t まで移動し、到達後に待機する
        /// </summary>
        private async UniTask MoveToAsync(float targetT, float waitTime, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (_moveDistance <= 0f && _moveSpeed <= 0f)
                {
                    await UniTask.Yield(PlayerLoopTiming.FixedUpdate, cancellationToken);
                    continue;
                }

                float delta = (_moveSpeed / _moveDistance) * Time.fixedDeltaTime;
                float nextT = _t + delta * _direction;

                // 到達判定
                if (_direction > 0)
                {
                    _t = Mathf.Min(nextT, targetT);
                }
                else
                {
                    _t = Mathf.Max(nextT, targetT);
                }

                MovePlatform();

                // 目標到達
                if (Mathf.Approximately(_t, targetT))
                {
                    _direction *= -1;

                    if (waitTime > 0f)
                    {
                        await UniTask.Delay(
                            TimeSpan.FromSeconds(waitTime),
                            DelayType.DeltaTime,
                            PlayerLoopTiming.Update,
                            cancellationToken);
                    }

                    return;
                }

                await UniTask.Yield(PlayerLoopTiming.FixedUpdate, cancellationToken);
            }
        }


        private void MovePlatform()
        {
            float curvedT = EvaluateCurve(_t);

            Vector3 targetPosition = Vector3.Lerp(_startPosition, _endPosition, curvedT);

            _rigidbody.MovePosition(targetPosition);
        }


        private float EvaluateCurve(float t)
        {
            if (_animationCurve == null) return Mathf.Clamp01(t);

            return Mathf.Clamp01(_animationCurve.Evaluate(t));
        }


        private Vector3 GetDirectionVector()
        {
            return _moveDirection switch
            {
                MoveDirection.PositiveX => Vector3.right,
                MoveDirection.PositiveY => Vector3.up,
                MoveDirection.PositiveZ => Vector3.forward,
                MoveDirection.NegativeX => Vector3.left,
                MoveDirection.NegativeY => Vector3.down,
                MoveDirection.NegativeZ => Vector3.back,
                _ => Vector3.zero
            };
        }


#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_moveDistance < 0f)
                _moveDistance = 0f;

            if (_moveSpeed < 0f)
                _moveSpeed = 0f;

            _offset = Mathf.Clamp(_offset, 0f, _moveDistance);
        }

        private void OnDrawGizmosSelected()
        {
            if (!_isActive) return;

            Vector3 offset = Vector3.up * _height;
            Vector3 direction = GetDirectionVector();

            Vector3 start = Application.isPlaying
                ? _startPosition + offset
                : transform.position + offset + (-direction * _offset);

            Vector3 end = start + direction * _moveDistance;

            // 全体ライン
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(start, end);

            // 始点
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(start, 0.05f);

            // 終点
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(end, 0.05f);
        }
#endif
    }
}