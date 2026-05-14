// 修正点
// 1. _StraightDistance → _straightDistance（命名規則統一）
// 2. AnimationCurve を Inspector で編集できるように [SerializeField] を追加
// 3. 待機時間 (_MinPointWaitingTime / _MaxPointWaitingTime) を実装
// 4. 直線移動区間 (_straightDistance) を実装
//    - 開始地点から straightDistance 分だけ等速移動
//    - その後、AnimationCurve による補間で終点まで移動
//
// 移動イメージ
// Start ----(直線移動)---- Curve補間 ---- End

using UnityEngine;

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

    /// <summary>
    /// Rigidbody ベースで往復移動するプッシュプレート
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class PushPlate2 : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private MoveDirection _moveDirection = MoveDirection.PositiveZ;
        [SerializeField, Min(0f)] private float _moveDistance = 1.0f;
        [SerializeField, Min(0f)] private float _straightDistance = 0.0f;
        [SerializeField, Min(0f)] private float _moveSpeed = 1.0f;
        [SerializeField, Min(0f)] private float _minPointWaitingTime = 0.3f;
        [SerializeField, Min(0f)] private float _maxPointWaitingTime = 0.3f;

        [SerializeField]
        private AnimationCurve _animationCurve =
            AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Header("Debug")]
        [SerializeField] private bool _isActive = true;
        [SerializeField] private float _height = 0f;

        private Rigidbody _rigidbody;

        private Vector3 _startPosition;
        private Vector3 _endPosition;

        private float _t;                 // 0～1
        private int _direction = 1;       // 1: forward, -1: backward
        private float _waitTimer = 0f;    // 待機タイマー

        #region Unity Methods

        private void Awake()
        {
            InitializeRigidbody();
            CachePositions();
        }

        private void FixedUpdate()
        {
            if (!CanMove())
                return;

            if (UpdateWaiting())
                return;

            UpdateInterpolation();
            MovePlatform();
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            DrawMovementGizmos();
        }
#endif

        #endregion

        #region Initialization

        private void InitializeRigidbody()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.useGravity = false;
            _rigidbody.isKinematic = true;
        }

        private void CachePositions()
        {
            _startPosition = transform.position;
            _endPosition = _startPosition + GetDirectionVector() * _moveDistance;

            // 範囲内に制限
            _straightDistance = Mathf.Clamp(_straightDistance, 0f, _moveDistance);
        }

        #endregion

        #region Movement

        private bool CanMove()
        {
            return _isActive &&
                   _moveDistance > 0f &&
                   _moveSpeed > 0f;
        }

        /// <summary>
        /// 待機中なら true を返す
        /// </summary>
        private bool UpdateWaiting()
        {
            if (_waitTimer <= 0f)
                return false;

            _waitTimer -= Time.fixedDeltaTime;
            return true;
        }

        private void UpdateInterpolation()
        {
            float delta = (_moveSpeed / _moveDistance) * Time.fixedDeltaTime;
            _t += delta * _direction;

            // 終点到達
            if (_t >= 1f)
            {
                _t = 1f;
                _direction = -1;
                _waitTimer = _maxPointWaitingTime;
            }
            // 始点到達
            else if (_t <= 0f)
            {
                _t = 0f;
                _direction = 1;
                _waitTimer = _minPointWaitingTime;
            }
        }

        private void MovePlatform()
        {
            float evaluatedT = EvaluateMovement(_t);

            Vector3 targetPosition = Vector3.Lerp(
                _startPosition,
                _endPosition,
                evaluatedT);

            _rigidbody.MovePosition(targetPosition);
        }

        /// <summary>
        /// 直線移動区間 + カーブ補間を適用した t を返す
        /// </summary>
        private float EvaluateMovement(float t)
        {
            if (_moveDistance <= 0f)
                return t;

            // 全体に対する直線区間の割合
            float linearRatio = _straightDistance / _moveDistance;

            // straightDistance がない場合は通常のカーブ
            if (linearRatio <= 0f)
                return EvaluateCurve(t);

            // 全体が直線区間ならそのまま
            if (linearRatio >= 1f)
                return t;

            // 直線区間
            if (t <= linearRatio)
                return t;

            // 残り区間を 0～1 に正規化
            float normalized = Mathf.InverseLerp(linearRatio, 1f, t);

            // カーブ適用
            float curved = EvaluateCurve(normalized);

            // 元の区間へ戻す
            return Mathf.Lerp(linearRatio, 1f, curved);
        }

        private float EvaluateCurve(float t)
        {
            return _animationCurve != null
                ? _animationCurve.Evaluate(t)
                : t;
        }

        #endregion

        #region Utility

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

        /// <summary>
        /// Inspector の変更を反映
        /// </summary>
        public void Refresh()
        {
            CachePositions();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_moveDistance < 0f)
                _moveDistance = 0f;

            _straightDistance = Mathf.Clamp(_straightDistance, 0f, _moveDistance);
        }
#endif

        #endregion

#if UNITY_EDITOR
        #region Gizmos

        private void DrawMovementGizmos()
        {
            Vector3 offset = Vector3.up * _height;

            Vector3 start = Application.isPlaying
                ? _startPosition + offset
                : transform.position + offset;

            Vector3 direction = GetDirectionVector();

            Vector3 straightPoint = start + direction * _straightDistance;
            Vector3 end = start + direction * _moveDistance;

            // 全体
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(start, end);

            // 直線区間
            if (_straightDistance > 0f)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(start, straightPoint);
                Gizmos.DrawSphere(straightPoint, 0.04f);
            }

            // 始点・終点
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(start, 0.05f);

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(end, 0.05f);
        }

        #endregion
#endif
    }
}