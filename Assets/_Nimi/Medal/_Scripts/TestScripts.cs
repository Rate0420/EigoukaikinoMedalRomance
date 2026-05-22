using UnityEngine;

public class TestScripts : MonoBehaviour
{
    [Header("メダルの設定")]
    [SerializeField] private Rigidbody _medalPrefab;

    [Header("発射位置の設定")]
    [SerializeField] private Transform _launchTransform;
    [SerializeField] private Vector3 _launchOffset;
    [SerializeField] private Vector3 _launchRotation;

    [Header("発射の設定")]
    [SerializeField] private Vector3 _launchDirection = Vector3.forward; // 発射方向
    [SerializeField, Min(0)] private float _launchForce = 10f;

    [Header("発射位置の制限")]
    [SerializeField] private Vector3 _positionA;
    [SerializeField] private Vector3 _positionB;

    [Header("デバッグ設定")]
    [SerializeField] private bool _isDrawGizmos = true;


    private Vector3 _mousePosition = Vector3.zero;

#if UNITY_EDITOR

    private void OnDrawGizmosSelected()
    {
        // Gizmosを描画するか、発射位置が設定されているかを確認
        if (!_isDrawGizmos || !_launchTransform) return;

        // 発射位置の制限を描画
        var positionA = _launchTransform.TransformPoint(_launchOffset) + _positionA;
        var positionB = _launchTransform.TransformPoint(_launchOffset) + _positionB;

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(positionA, 0.15f);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(positionB, 0.15f);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(positionA, positionB);
    }
#endif
}