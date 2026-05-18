using UnityEngine;

namespace EMR.Madal.Iaunch
{
    /// <summary>
    /// メダルを発射するクラス
    /// </summary>
    public class MedalIaunch : MonoBehaviour
    {
        [SerializeField] private Rigidbody _medalPrefab; // 発射するメダルのプレハブ

        [Header("発射位置の設定")]
        [SerializeField] private Transform _iaunchTransform; // 発射位置のTransform
        [SerializeField] private Vector3 _iaunchOffset;      // 発射位置のオフセット

        [Header("発射位置の制限")]
        [SerializeField] private Vector3 _iaunchMinPosition; // 発射位置の最小値
        [SerializeField] private Vector3 _iaunchMaxPosition; // 発射位置の最大値

        [Header("発射力の設定")]
        [SerializeField] private float _launchForce;      // 発射力
        [SerializeField] private Vector3 _launchRotation; // 発射時の回転

        [Header("Gizmos")]
        [SerializeField] private bool _isShowGizmos; // Gizmosを表示するかどうか


        private Vector3 _mousePosition;  // マウスの位置
        private Vector3 _iaunchPosition; // 発射位置


        private void Update()
        {
            // マウスの位置を取得
            _mousePosition = Input.mousePosition;
            // 発射位置を計算
            _iaunchPosition = _iaunchTransform.position + _iaunchOffset;
            //// 発射位置の制限
            //_iaunchPosition.x = Mathf.Clamp(_iaunchPosition.x, _iaunchMinPosition.x, _iaunchMaxPosition.x);
            //_iaunchPosition.y = Mathf.Clamp(_iaunchPosition.y, _iaunchMinPosition.y, _iaunchMaxPosition.y);
            //_iaunchPosition.z = Mathf.Clamp(_iaunchPosition.z, _iaunchMinPosition.z, _iaunchMaxPosition.z);

            // マウスクリックでメダルを発射
            if (Input.GetMouseButtonDown(0))
            {
                LaunchMedal();
            }
        }

        /// <summary>
        /// メダルを発射する
        /// </summary>
        private void LaunchMedal()
        {
            // メダルをインスタンス化
            Rigidbody medal = Instantiate(_medalPrefab, _iaunchPosition, Quaternion.identity);
            // 発射力を与える
            medal.AddForce(medal.transform.forward * _launchForce, ForceMode.Impulse);
        }

        private void OnDrawGizmos()
        {

            if (_isShowGizmos)
            {
                var iaunchPosition = _iaunchTransform.position + _iaunchOffset;

                // 発射位置をGizmosで表示
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(iaunchPosition, 0.1f);
                // 発射位置の回転をGizmosで表示
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(iaunchPosition, Vector3.forward);
            }
        }
    }
}