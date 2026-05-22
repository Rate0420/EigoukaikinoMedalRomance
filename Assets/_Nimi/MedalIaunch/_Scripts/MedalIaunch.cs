using UnityEngine;
using EMR.Utility;

namespace EMR.Medal.Launch
{
    /// <summary>
    /// メダルを発射するクラス
    /// </summary>
    public class MedalIaunch : MonoBehaviour
    {
        [Header("メダルの設定")]
        [SerializeField] private Rigidbody _medalPrefab; // 発射するメダルのPrefab

        [Header("入力の設定")]
        [SerializeField] private Camera _inputCamera; // マウス位置の計算に使用するカメラ

        [Header("発射位置の設定")]
        [SerializeField] private Transform _launchTransform; // 発射位置の基準となるTransform
        [SerializeField] private Vector3 _launchOffset;      // 発射位置のオフセット
        [SerializeField] private Vector3 _launchRotation;    // 発射位置・方向に加える回転

        [Header("発射の設定")]
        [SerializeField] private bool _isFollowingRotation = false; // 発射方向を回転に追尾させるか
        [SerializeField] private MoveDirection _launchDirection = MoveDirection.PositiveZ; // 基準となる発射方向
        [SerializeField, Min(0)] private float _launchForce = 10f; // 発射の強さ

        [Header("発射位置の制限")]
        [SerializeField] private Vector3 _positionA = Vector3.left * 45f;  // 発射可能範囲の開始位置
        [SerializeField] private Vector3 _positionB = Vector3.right * 45f; // 発射可能範囲の終了位置

        [Header("デバッグ設定")]
        [SerializeField] private bool _isDrawGizmos = true; // Gizmosを表示するか

        private Vector3 _mousePosition = Vector3.zero;  // 現在のマウス位置
        private Vector3 _launchPosition = Vector3.zero; // 実際にメダルを発射する位置

        private void Reset()
        {
            // コンポーネント追加時、自身のTransformを基準にする
            _launchTransform = transform;
        }

        private void Awake()
        {
            // 未設定の場合は自身のTransformを発射基準にする
            if (!_launchTransform)
            {
                _launchTransform = transform;
            }
        }

        private void Update()
        {
            // 毎フレーム、マウス位置に応じた発射位置を更新する
            UpdateLaunchPosition();

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
            if (!_medalPrefab)
            {
                Debug.LogWarning($"{nameof(MedalIaunch)}: Medal Prefab is not assigned.", this);
                return;
            }

            // 計算済みの発射位置と回転でメダルを生成する
            var medal = Instantiate(_medalPrefab, _launchPosition, GetLaunchRotation());

            // 発射方向に力を加える
            medal.AddForce(GetLaunchDirection() * _launchForce, ForceMode.Impulse);
        }

        /// <summary>
        /// マウス位置から発射位置を更新する
        /// </summary>
        private void UpdateLaunchPosition()
        {
            _mousePosition = Input.mousePosition;
            _launchPosition = CalculateLaunchPosition(_mousePosition);
        }

        /// <summary>
        /// スクリーン座標から、発射可能範囲上の位置を計算する
        /// </summary>
        private Vector3 CalculateLaunchPosition(Vector3 screenPosition)
        {
            GetLaunchLine(out var positionA, out var positionB);

            var camera = GetInputCamera();

            // カメラが無い場合は中央から発射する
            if (!camera)
            {
                return Vector3.Lerp(positionA, positionB, 0.5f);
            }

            var segment = positionB - positionA;

            // AとBが同じ位置の場合はAを返す
            if (segment.sqrMagnitude <= Mathf.Epsilon)
            {
                return positionA;
            }

            var center = Vector3.Lerp(positionA, positionB, 0.5f);

            // 発射範囲の中央を通る、カメラに向いた平面を作る
            var plane = new Plane(-camera.transform.forward, center);

            // マウス位置からワールド空間へRayを飛ばす
            var ray = camera.ScreenPointToRay(screenPosition);

            // Rayが平面に当たらない場合は中央から発射する
            if (!plane.Raycast(ray, out var distance))
            {
                return center;
            }

            var hitPoint = ray.GetPoint(distance);

            // 当たった位置を、A-Bの線分上の割合に変換する
            var t = Vector3.Dot(hitPoint - positionA, segment) / segment.sqrMagnitude;

            // 0から1に制限して、発射可能範囲の外へ出ないようにする
            return Vector3.Lerp(positionA, positionB, Mathf.Clamp01(t));
        }

        /// <summary>
        /// 発射可能範囲のA地点とB地点を取得する
        /// </summary>
        private void GetLaunchLine(out Vector3 positionA, out Vector3 positionB)
        {
            var launchTransform = GetLaunchTransform();
            var rotation = Quaternion.Euler(_launchRotation);

            // _positionA/Bを_launchTransform基準のローカル位置として扱い、回転も反映する
            positionA = launchTransform.TransformPoint(_launchOffset + rotation * _positionA);
            positionB = launchTransform.TransformPoint(_launchOffset + rotation * _positionB);
        }

        /// <summary>
        /// メダル生成時の回転を取得する
        /// </summary>
        private Quaternion GetLaunchRotation()
        {
            return GetLaunchTransform().rotation * Quaternion.Euler(_launchRotation);
        }

        /// <summary>
        /// 発射方向を取得する
        /// </summary>
        private Vector3 GetLaunchDirection()
        {
            var direction = DirectionExtensions.ToVector3(_launchDirection).normalized;

            // 回転に追尾しない場合は、MoveDirectionのワールド方向をそのまま使う
            if (!_isFollowingRotation)
            {
                return direction;
            }

            // 回転に追尾する場合は、発射基準の回転と_launchRotationを反映する
            return GetLaunchRotation() * direction;
        }

        /// <summary>
        /// 発射基準のTransformを取得する
        /// </summary>
        private Transform GetLaunchTransform()
        {
            return _launchTransform ? _launchTransform : transform;
        }

        /// <summary>
        /// 入力計算に使うカメラを取得する
        /// </summary>
        private Camera GetInputCamera()
        {
            return _inputCamera ? _inputCamera : Camera.main;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (!_isDrawGizmos) return;

            // 発射可能範囲を取得する
            GetLaunchLine(out var positionA, out var positionB);

            // 再生中は実際の発射位置、編集中は中央位置を表示する
            var launchPosition = Application.isPlaying
                ? _launchPosition
                : Vector3.Lerp(positionA, positionB, 0.5f);

            var launchDirection = GetLaunchDirection();

            // 開始位置を黄色で表示
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(positionA, 0.25f);

            // 終了位置を赤色で表示
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(positionB, 0.25f);

            // 発射可能範囲を青線で表示
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(positionA, positionB);

            // 現在の発射位置と発射方向を緑色で表示
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(launchPosition, 0.35f);
            Gizmos.DrawRay(launchPosition, launchDirection * 3f);
        }
#endif
    }
}