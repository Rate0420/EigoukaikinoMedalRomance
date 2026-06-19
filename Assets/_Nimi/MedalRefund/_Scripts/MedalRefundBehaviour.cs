using UnityEngine;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using EMR.Core;
using EMR.Utility;


namespace EMR.Medal.Refund
{
    /// <summary>
    /// <see cref="GameState.OnRefundRequested"/> を受け取り、
    /// 実際に所持メダルへ枚数を加算するコンポーネント。
    /// </summary>
    public class MedalRefundBehaviour : MonoBehaviour
    {
        [SerializeField] MedalRefundSpawner[] _spawner;
        [SerializeField] ProbabilitySelector<GameObject> _enemySelector;

        // 払い戻しの通知
        private MedalRefundNotifier _refundNotifier;


        private void Start()
        {
            _refundNotifier = GameState.Instance.RefundNotifier;

            // 払い戻しが要求されたときのイベントを登録
            _refundNotifier.OnRefundRequested += HandleRefundRequested;

            _refundNotifier.RequestRefund(100);
        }

        private void OnDisable()
        {
            if (_refundNotifier != null)
            {
                // イベントを解除
                _refundNotifier.OnRefundRequested -= HandleRefundRequested;
            }
        }

        private void HandleRefundRequested(int refundAmount)
        {
            ProcessRefundAsync(refundAmount).Forget();
        }

        /// <summary>
        /// 払い戻し要求を受け取り、メダルを排出する
        /// </summary>
        /// <param name="amount">払い戻すメダルの枚数</param>
        private async UniTask ProcessRefundAsync(int refundAmount)
        {
            Debug.Log($"[{nameof(MedalRefundBehaviour)}] 払い戻し: {refundAmount} 枚 ");

            while (_refundNotifier.RefundAmount > 0)
            {

                // スポナーをランダム選択
                MedalRefundSpawner spawner = _spawner[Random.Range(0, _spawner.Length)];

                // 確率でPrefabを選択
                GameObject prefab = _enemySelector.GetRandom();

                // 生成
                spawner.SpawnMedal(prefab);

                // 払い戻し枚数を減らす
                _refundNotifier.RemoveRefund(1);

                // 連続生成を少し遅らせる
                await UniTask.Delay(System.TimeSpan.FromSeconds(0.25f));
            }
        }
    }
}