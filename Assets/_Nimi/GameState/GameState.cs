using UnityEngine;
using EMR.Medal;
using EMR.Round;

namespace EMR.Core
{
    public class GameState : MonoBehaviour
    {
        /// <summary>
        /// インスタンス
        /// </summary>
        public static GameState Instance { get; private set; }

        /// <summary>
        /// 所有するメダルの情報を取得します。
        /// </summary>
        public MedalsOwnedModel OwnedModel { get; private set; }

        /// <summary>
        /// メダルの払い戻しの通知システム
        /// </summary>
        public MedalRefundNotifier RefundNotifier { get; private set; }

        /// <summary>
        /// ラウンド進行管理システム
        /// </summary>
        public RoundManager RoundManager { get; private set; }

        /// <summary>
        /// ラウンド内で消費したメダル管理クラス
        /// </summary>
        public RoundAdvanceService RoundService { get; private set; }

        /// <summary>
        /// ポーズ管理用システム
        /// </summary>
        public GamePause GamePause { get; private set; }


        private void Awake()
        {
            // 既にインスタンスが存在する場合は、このオブジェクトを破棄します。
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);


            OwnedModel = new MedalsOwnedModel(30);
            RefundNotifier = new MedalRefundNotifier();
            RoundManager = new RoundManager();
            RoundService = new RoundAdvanceService();
            GamePause = new GamePause();
        }
    }
}