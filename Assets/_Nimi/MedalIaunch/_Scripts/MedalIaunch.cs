using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using EMR.Utility;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EMR.Medal.Launch
{
    /// <summary>
    /// メダルを発射するクラス
    /// </summary>
    public class MedalIaunch : MonoBehaviour
    {
        private enum LaunchMotionType
        {
            Impulse,
            BallisticToLandingPoint,
            CurveToLandingPoint
        }

        [Header("メダルの設定")]
        [SerializeField, FormerlySerializedAs("_medalPrefab")]
        private Rigidbody _medalRigidbodyPrefab; // 発射するメダルのPrefab

        [Header("入力の設定")]
        [SerializeField, FormerlySerializedAs("_inputCamera")]
        private Camera _mouseInputCamera; // マウス位置の計算に使用するカメラ

        [Header("発射位置の設定")]
        [SerializeField, FormerlySerializedAs("_launchTransform")]
        private Transform _launchReferenceTransform; // 発射位置の基準となるTransform

        [SerializeField, FormerlySerializedAs("_launchOffset")]
        private Vector3 _launchLocalOffset; // 発射位置のオフセット

        [SerializeField, FormerlySerializedAs("_launchRotation")]
        private Vector3 _launchRotationOffset; // 発射位置・方向に加える回転

        [Header("発射の設定")]
        [SerializeField]
        private LaunchMotionType _launchMotionType = LaunchMotionType.Impulse; // メダルの飛ばし方

        [SerializeField, FormerlySerializedAs("_isFollowingRotation")]
        private bool _useLaunchRotationForDirection = false; // 発射方向を回転に追尾させるか

        [SerializeField, FormerlySerializedAs("_launchDirection")]
        private MoveDirection _baseLaunchDirection = MoveDirection.PositiveZ; // 基準となる発射方向

        [SerializeField, FormerlySerializedAs("_launchForce"), Min(0)]
        private float _launchImpulse = 10f; // Impulse発射時の強さ

        [Header("着地時間の設定")]
        [SerializeField, Min(0.01f)]
        private float _timeToLanding = 1f; // 着地するまでの時間

        [Header("カーブ発射の設定")]
        [SerializeField]
        private AnimationCurve _flightProgressCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f); // 着地点まで進む速さ

        [SerializeField]
        private AnimationCurve _flightHeightCurve = new AnimationCurve(
            new Keyframe(0f, 0f),
            new Keyframe(0.5f, 1f),
            new Keyframe(1f, 0f)); // 飛行中の高さ

        [SerializeField, Min(0f)]
        private float _flightHeight = 3f; // カーブ発射時に上へ持ち上げる高さ

        [SerializeField]
        private bool _restorePhysicsAfterCurve = true; // カーブ移動後にRigidbodyの物理挙動へ戻すか

        [Header("発射位置の制限")]
        [SerializeField, FormerlySerializedAs("_positionA")]
        private Vector3 _launchRangeStartLocalPosition = Vector3.left * 45f; // 発射可能範囲の開始位置

        [SerializeField, FormerlySerializedAs("_positionB")]
        private Vector3 _launchRangeEndLocalPosition = Vector3.right * 45f; // 発射可能範囲の終了位置

        [Header("マウス入力範囲")]
        [SerializeField, FormerlySerializedAs("_offset")]
        private Vector3 _mouseInputWorldOffset; // マウス入力範囲全体に加えるワールドオフセット

        [SerializeField, FormerlySerializedAs("_leftMaxPosition")]
        private Vector3 _mouseInputLeftWorldPosition = Vector3.right * -45f; // カメラから見たマウス入力範囲の左端

        [SerializeField, FormerlySerializedAs("_rightMaxPosition")]
        private Vector3 _mouseInputRightWorldPosition = Vector3.right * 45f; // カメラから見たマウス入力範囲の右端

        [Header("デバッグ設定")]
        [SerializeField, FormerlySerializedAs("_isDrawGizmos")]
        private bool _drawGizmos = true; // Gizmosを表示するか

        private Vector3 _mouseScreenPosition = Vector3.zero; // 現在のマウススクリーン座標
        private Vector3 _currentLaunchPosition = Vector3.zero; // 実際にメダルを発射する位置

        private void Reset()
        {
            // コンポーネント追加時、自身のTransformを基準にする
            _launchReferenceTransform = transform;
        }

        private void Awake()
        {
            // 未設定の場合は自身のTransformを発射基準にする
            if (!_launchReferenceTransform)
            {
                _launchReferenceTransform = transform;
            }
        }

        private void Update()
        {
            // 毎フレーム、マウス位置に応じた発射位置を更新する
            UpdateCurrentLaunchPosition();

            // 左クリックでメダルを発射する
            if (Input.GetMouseButtonDown(0))
            {
                Launch();
            }
        }

        /// <summary>
        /// メダルを生成して発射する
        /// </summary>
        public void Launch()
        {
            if (!_medalRigidbodyPrefab)
            {
                Debug.LogWarning($"{nameof(MedalIaunch)}: Medal Prefab is not assigned.", this);
                return;
            }

            // 計算済みの発射位置と回転でメダルを生成する
            var medal = Instantiate(_medalRigidbodyPrefab, _currentLaunchPosition, GetLaunchRotation());

            ApplyLaunchMotion(medal);
        }

        /// <summary>
        /// 設定された発射方式でメダルを飛ばす
        /// </summary>
        private void ApplyLaunchMotion(Rigidbody medal)
        {
            switch (_launchMotionType)
            {
                case LaunchMotionType.BallisticToLandingPoint:
                    LaunchBallisticToLandingPoint(medal);
                    break;

                case LaunchMotionType.CurveToLandingPoint:
                    StartCoroutine(LaunchAlongCurve(medal));
                    break;

                case LaunchMotionType.Impulse:
                default:
                    LaunchByImpulse(medal);
                    break;
            }
        }

        /// <summary>
        /// 発射方向にImpulseで力を加える
        /// </summary>
        private void LaunchByImpulse(Rigidbody medal)
        {
            medal.AddForce(GetLaunchDirection() * _launchImpulse, ForceMode.Impulse);
        }

        /// <summary>
        /// 着地点と着地時間から初速を計算して物理発射する
        /// </summary>
        private void LaunchBallisticToLandingPoint(Rigidbody medal)
        {
            SetRigidbodyVelocity(medal, CalculateBallisticInitialVelocity(
                medal.position,
                GetLandingPosition(),
                GetSafeTimeToLanding()));
        }

        /// <summary>
        /// AnimationCurveで制御した軌道に沿って着地点まで移動させる
        /// </summary>
        private IEnumerator LaunchAlongCurve(Rigidbody medal)
        {
            var startPosition = medal.position;
            var landingPosition = GetLandingPosition();
            var timeToLanding = GetSafeTimeToLanding();
            var originalIsKinematic = medal.isKinematic;
            var originalUseGravity = medal.useGravity;

            SetRigidbodyVelocity(medal, Vector3.zero);
            medal.angularVelocity = Vector3.zero;
            medal.useGravity = false;
            medal.isKinematic = true;

            var elapsedTime = 0f;

            while (elapsedTime < timeToLanding)
            {
                if (!medal)
                {
                    yield break;
                }

                var normalizedTime = Mathf.Clamp01(elapsedTime / timeToLanding);
                medal.MovePosition(EvaluateCurveLaunchPosition(startPosition, landingPosition, normalizedTime));

                elapsedTime += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            if (!medal)
            {
                yield break;
            }

            medal.MovePosition(landingPosition);

            if (!_restorePhysicsAfterCurve)
            {
                yield break;
            }

            medal.isKinematic = originalIsKinematic;
            medal.useGravity = originalUseGravity;

            if (!medal.isKinematic)
            {
                SetRigidbodyVelocity(medal, CalculateCurveLandingVelocity(startPosition, landingPosition, timeToLanding));
            }
        }

        /// <summary>
        /// Unityのバージョン差を吸収してRigidbodyの速度を設定する
        /// </summary>
        private void SetRigidbodyVelocity(Rigidbody targetRigidbody, Vector3 velocity)
        {
#if UNITY_6000_0_OR_NEWER
            targetRigidbody.linearVelocity = velocity;
#else
            targetRigidbody.velocity = velocity;
#endif
        }

        /// <summary>
        /// 着地点を取得する
        /// </summary>
        private Vector3 GetLandingPosition()
        {
            return GetMouseInputPosition(GetCurrentInputRatio());
        }

        /// <summary>
        /// 現在のマウス位置から入力範囲上の割合を取得する
        /// </summary>
        private float GetCurrentInputRatio()
        {
            var camera = GetMouseInputCamera();
            return camera ? CalculateMouseInputRatio(camera, Input.mousePosition) : 0.5f;
        }

        /// <summary>
        /// 入力範囲上の割合から、マウス入力位置のワールド座標を取得する
        /// </summary>
        private Vector3 GetMouseInputPosition(float inputRatio)
        {
            GetMouseInputRange(out var mouseInputLeftPosition, out var mouseInputRightPosition);
            return Vector3.Lerp(mouseInputLeftPosition, mouseInputRightPosition, Mathf.Clamp01(inputRatio));
        }

        /// <summary>
        /// 着地時間が0以下にならないようにする
        /// </summary>
        private float GetSafeTimeToLanding()
        {
            return Mathf.Max(0.01f, _timeToLanding);
        }

        /// <summary>
        /// 指定時間後に着地点へ到達するための初速を計算する
        /// </summary>
        private Vector3 CalculateBallisticInitialVelocity(Vector3 startPosition, Vector3 landingPosition, float timeToLanding)
        {
            return (landingPosition - startPosition - Physics.gravity * (0.5f * timeToLanding * timeToLanding)) / timeToLanding;
        }

        /// <summary>
        /// 物理弾道上の位置を計算する
        /// </summary>
        private Vector3 EvaluateBallisticPosition(Vector3 startPosition, Vector3 landingPosition, float normalizedTime)
        {
            var timeToLanding = GetSafeTimeToLanding();
            var elapsedTime = Mathf.Clamp01(normalizedTime) * timeToLanding;
            var initialVelocity = CalculateBallisticInitialVelocity(startPosition, landingPosition, timeToLanding);

            return startPosition + initialVelocity * elapsedTime + Physics.gravity * (0.5f * elapsedTime * elapsedTime);
        }

        /// <summary>
        /// カーブ発射時の位置を計算する
        /// </summary>
        private Vector3 EvaluateCurveLaunchPosition(Vector3 startPosition, Vector3 landingPosition, float normalizedTime)
        {
            var time = Mathf.Clamp01(normalizedTime);
            var progress = time <= 0f
                ? 0f
                : time >= 1f
                    ? 1f
                    : Mathf.Clamp01(_flightProgressCurve.Evaluate(time));

            var height = time <= 0f || time >= 1f
                ? 0f
                : _flightHeightCurve.Evaluate(time) * _flightHeight;

            return Vector3.Lerp(startPosition, landingPosition, progress) + Vector3.up * height;
        }

        /// <summary>
        /// カーブ移動終了後に物理挙動へ戻すときの速度を計算する
        /// </summary>
        private Vector3 CalculateCurveLandingVelocity(Vector3 startPosition, Vector3 landingPosition, float timeToLanding)
        {
            var sampleTime = Mathf.Min(Time.fixedDeltaTime / timeToLanding, 0.25f);
            var previousPosition = EvaluateCurveLaunchPosition(startPosition, landingPosition, 1f - sampleTime);

            return (landingPosition - previousPosition) / (sampleTime * timeToLanding);
        }

        /// <summary>
        /// マウス位置から発射位置を更新する
        /// </summary>
        private void UpdateCurrentLaunchPosition()
        {
            _mouseScreenPosition = Input.mousePosition;
            _currentLaunchPosition = CalculateLaunchPositionFromScreenPoint(_mouseScreenPosition);
        }

        /// <summary>
        /// スクリーン座標から、発射可能範囲上の位置を計算する
        /// </summary>
        private Vector3 CalculateLaunchPositionFromScreenPoint(Vector3 screenPosition)
        {
            GetLaunchRange(out var launchStartPosition, out var launchEndPosition);

            var camera = GetMouseInputCamera();

            // カメラが無い場合は中央から発射する
            if (!camera)
            {
                return Vector3.Lerp(launchStartPosition, launchEndPosition, 0.5f);
            }

            var inputT = CalculateMouseInputRatio(camera, screenPosition);
            return Vector3.Lerp(launchStartPosition, launchEndPosition, inputT);
        }

        /// <summary>
        /// カメラから見たマウス入力範囲上の割合を計算する
        /// </summary>
        private float CalculateMouseInputRatio(Camera camera, Vector3 screenPosition)
        {
            GetMouseInputRange(out var mouseInputLeftPosition, out var mouseInputRightPosition);

            var leftScreenPosition = camera.WorldToScreenPoint(mouseInputLeftPosition);
            var rightScreenPosition = camera.WorldToScreenPoint(mouseInputRightPosition);

            // 入力範囲がカメラの裏側にある場合は中央扱いにする
            if (leftScreenPosition.z <= 0f || rightScreenPosition.z <= 0f)
            {
                return 0.5f;
            }

            var inputStart = new Vector2(leftScreenPosition.x, leftScreenPosition.y);
            var inputEnd = new Vector2(rightScreenPosition.x, rightScreenPosition.y);
            var inputSegment = inputEnd - inputStart;

            // 左右の入力位置が画面上で重なっている場合は中央扱いにする
            if (inputSegment.sqrMagnitude <= Mathf.Epsilon)
            {
                return 0.5f;
            }

            var mouseScreenPoint = new Vector2(screenPosition.x, screenPosition.y);
            var inputRatio = Vector2.Dot(mouseScreenPoint - inputStart, inputSegment) / inputSegment.sqrMagnitude;

            // mouseInputLeftPositionで0、mouseInputRightPositionで1になるように制限する
            return Mathf.Clamp01(inputRatio);
        }

        /// <summary>
        /// 発射可能範囲の開始地点と終了地点を取得する
        /// </summary>
        private void GetLaunchRange(out Vector3 launchStartPosition, out Vector3 launchEndPosition)
        {
            var launchReferenceTransform = GetLaunchReferenceTransform();
            var rotationOffset = Quaternion.Euler(_launchRotationOffset);

            // 発射範囲は_launchReferenceTransform基準のローカル位置として扱い、回転も反映する
            launchStartPosition = launchReferenceTransform.TransformPoint(_launchLocalOffset + rotationOffset * _launchRangeStartLocalPosition);
            launchEndPosition = launchReferenceTransform.TransformPoint(_launchLocalOffset + rotationOffset * _launchRangeEndLocalPosition);
        }

        /// <summary>
        /// カメラから見たマウス入力範囲の左端と右端を取得する
        /// </summary>
        private void GetMouseInputRange(out Vector3 mouseInputLeftPosition, out Vector3 mouseInputRightPosition)
        {
            mouseInputLeftPosition = _mouseInputWorldOffset + _mouseInputLeftWorldPosition;
            mouseInputRightPosition = _mouseInputWorldOffset + _mouseInputRightWorldPosition;
        }

        /// <summary>
        /// メダル生成時の回転を取得する
        /// </summary>
        private Quaternion GetLaunchRotation()
        {
            return GetLaunchReferenceTransform().rotation * Quaternion.Euler(_launchRotationOffset);
        }

        /// <summary>
        /// 発射方向を取得する
        /// </summary>
        private Vector3 GetLaunchDirection()
        {
            var direction = DirectionExtensions.ToVector3(_baseLaunchDirection).normalized;

            // 回転に追尾しない場合は、MoveDirectionのワールド方向をそのまま使う
            if (!_useLaunchRotationForDirection)
            {
                return direction;
            }

            // 回転に追尾する場合は、発射基準の回転と_launchRotationOffsetを反映する
            return GetLaunchRotation() * direction;
        }

        /// <summary>
        /// 発射基準のTransformを取得する
        /// </summary>
        private Transform GetLaunchReferenceTransform()
        {
            return _launchReferenceTransform ? _launchReferenceTransform : transform;
        }

        /// <summary>
        /// 入力計算に使うカメラを取得する
        /// </summary>
        private Camera GetMouseInputCamera()
        {
            return _mouseInputCamera ? _mouseInputCamera : Camera.main;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (!_drawGizmos) return;

            GetLaunchRange(out var launchStartPosition, out var launchEndPosition);
            GetMouseInputRange(out var mouseInputLeftPosition, out var mouseInputRightPosition);

            var inputRatio = GetPreviewInputRatio();
            var launchPosition = Vector3.Lerp(launchStartPosition, launchEndPosition, inputRatio);
            var mouseInputPosition = Vector3.Lerp(mouseInputLeftPosition, mouseInputRightPosition, inputRatio);

            DrawLaunchRangeGizmos(launchStartPosition, launchEndPosition);
            DrawMouseInputRangeGizmos(mouseInputLeftPosition, mouseInputRightPosition, mouseInputPosition);
            DrawLaunchPreviewGizmos(launchPosition, mouseInputPosition, inputRatio);
            DrawLandingGizmos(launchPosition, mouseInputPosition);
            DrawSettingsLabel(launchPosition, inputRatio);
        }

        private void DrawLandingGizmos(Vector3 launchPosition, Vector3 landingPosition)
        {
            if (_launchMotionType == LaunchMotionType.Impulse)
            {
                return;
            }

            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(landingPosition, 0.3f);
            DrawWireDisc(landingPosition, Vector3.up, 0.65f, Color.magenta);
            DrawGizmoLabel(landingPosition + Vector3.up * 0.8f, $"着地点: マウス入力位置\nTime: {GetSafeTimeToLanding():0.##}s");

            const int trajectorySegmentCount = 24;
            var previousPosition = launchPosition;

            for (var i = 1; i <= trajectorySegmentCount; i++)
            {
                var normalizedTime = i / (float)trajectorySegmentCount;
                var nextPosition = _launchMotionType == LaunchMotionType.BallisticToLandingPoint
                    ? EvaluateBallisticPosition(launchPosition, landingPosition, normalizedTime)
                    : EvaluateCurveLaunchPosition(launchPosition, landingPosition, normalizedTime);

                Gizmos.DrawLine(previousPosition, nextPosition);
                previousPosition = nextPosition;

                if (i % 6 == 0)
                {
                    Gizmos.DrawSphere(nextPosition, 0.12f);
                    DrawGizmoLabel(nextPosition + Vector3.up * 0.25f, $"{normalizedTime * GetSafeTimeToLanding():0.##}s");
                }
            }
        }

        private float GetPreviewInputRatio()
        {
            if (!Application.isPlaying)
            {
                return 0.5f;
            }

            return GetCurrentInputRatio();
        }

        private void DrawLaunchRangeGizmos(Vector3 launchStartPosition, Vector3 launchEndPosition)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(launchStartPosition, 0.25f);
            DrawGizmoLabel(launchStartPosition + Vector3.up * 0.45f, "発射範囲 Start");

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(launchEndPosition, 0.25f);
            DrawGizmoLabel(launchEndPosition + Vector3.up * 0.45f, "発射範囲 End");

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(launchStartPosition, launchEndPosition);
        }

        private void DrawMouseInputRangeGizmos(Vector3 mouseInputLeftPosition, Vector3 mouseInputRightPosition, Vector3 mouseInputPosition)
        {
            Gizmos.color = Color.gray;
            Gizmos.DrawSphere(mouseInputLeftPosition, 0.25f);
            Gizmos.DrawSphere(mouseInputRightPosition, 0.25f);
            Gizmos.DrawLine(mouseInputLeftPosition, mouseInputRightPosition);

            DrawGizmoLabel(mouseInputLeftPosition + Vector3.up * 0.45f, "マウス入力 Left");
            DrawGizmoLabel(mouseInputRightPosition + Vector3.up * 0.45f, "マウス入力 Right");

            Gizmos.color = Color.white;
            Gizmos.DrawSphere(mouseInputPosition, 0.2f);
            DrawWireDisc(mouseInputPosition, Vector3.up, 0.45f, Color.white);
        }

        private void DrawLaunchPreviewGizmos(Vector3 launchPosition, Vector3 mouseInputPosition, float inputRatio)
        {
            var launchDirection = GetLaunchDirection();

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(mouseInputPosition, launchPosition);

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(launchPosition, 0.35f);
            Gizmos.DrawRay(launchPosition, launchDirection * 3f);
            DrawWireDisc(launchPosition, launchDirection.normalized, 0.55f, Color.green);
            DrawGizmoLabel(launchPosition + Vector3.up * 0.8f, $"発射位置\nInput: {inputRatio:0.00}");
        }

        private void DrawSettingsLabel(Vector3 launchPosition, float inputRatio)
        {
            var labelPosition = launchPosition + Vector3.up * 1.6f;
            var text = $"Mode: {_launchMotionType}\nInput: {inputRatio:0.00}";

            switch (_launchMotionType)
            {
                case LaunchMotionType.BallisticToLandingPoint:
                    text += $"\nLanding: Mouse Input\nTime: {GetSafeTimeToLanding():0.##}s";
                    break;

                case LaunchMotionType.CurveToLandingPoint:
                    text += $"\nLanding: Mouse Input\nTime: {GetSafeTimeToLanding():0.##}s\nHeight: {_flightHeight:0.##}";
                    break;

                case LaunchMotionType.Impulse:
                default:
                    text += $"\nImpulse: {_launchImpulse:0.##}";
                    break;
            }

            DrawGizmoLabel(labelPosition, text);
        }

        private void DrawWireDisc(Vector3 center, Vector3 normal, float radius, Color color)
        {
            Handles.color = color;
            Handles.DrawWireDisc(center, normal.sqrMagnitude <= Mathf.Epsilon ? Vector3.up : normal.normalized, radius);
        }

        private void DrawGizmoLabel(Vector3 position, string text)
        {
            Handles.Label(position, text, EditorStyles.boldLabel);
        }
#endif
    }
}
