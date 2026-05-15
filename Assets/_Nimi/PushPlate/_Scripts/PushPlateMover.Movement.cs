using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace EMR.PushPlate
{
    public partial class PushPlateMover
    {
        private void StartMoveLoop()
        {
            MoveLoopAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        private async UniTaskVoid MoveLoopAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (!CanMove())
                {
                    await UniTask.Yield(PlayerLoopTiming.FixedUpdate, cancellationToken);
                    continue;
                }

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

        private async UniTask MoveToAsync(float targetT, float waitTime, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (!CanMove())
                {
                    await UniTask.Yield(PlayerLoopTiming.FixedUpdate, cancellationToken);
                    continue;
                }

                float delta = (_moveSpeed / _moveDistance) * Time.fixedDeltaTime;
                float nextT = _t + delta * _direction;

                _t = _direction > 0
                    ? Mathf.Min(nextT, targetT)
                    : Mathf.Max(nextT, targetT);

                MovePlatform();

                if (Mathf.Approximately(_t, targetT))
                {
                    _direction *= -1;
                    await WaitAtPointAsync(waitTime, cancellationToken);
                    return;
                }

                await UniTask.Yield(PlayerLoopTiming.FixedUpdate, cancellationToken);
            }
        }

        private async UniTask WaitAtPointAsync(float waitTime, CancellationToken cancellationToken)
        {
            if (waitTime <= 0f)
                return;

            await UniTask.Delay(
                TimeSpan.FromSeconds(waitTime),
                DelayType.DeltaTime,
                PlayerLoopTiming.Update,
                cancellationToken);
        }

        private void MovePlatform()
        {
            float curvedT = EvaluateCurve(_t);
            Vector3 targetPosition = Vector3.Lerp(_startPosition, _endPosition, curvedT);

            _rigidbody.MovePosition(targetPosition);
        }

        private bool CanMove()
        {
            return _moveDistance > 0f && _moveSpeed > 0f;
        }
    }
}