using System;
using UnityEngine;
using EMR.Medal;
using EMR.Round;

namespace EMR.Core
{
    public class GameState : MonoBehaviour
    {
        public static GameState Instance { get; private set; }

        public MedalsOwnedModel OwnedModel { get; private set; }
        public MedalRefundNotifier RefundNotifier { get; private set; }
        public RoundManager RoundManager { get; private set; }
        public RoundAdvanceService RoundService { get; private set; }
        public GamePause GamePause { get; private set; }

        public RoundProgressionSettings RoundSettings { get; private set; }

        public bool IsInitialized { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// ゲーム全体のサービスを初期化する。
        /// Bootstrapper から一度だけ呼ぶ。
        /// </summary>
        public void Initialize(RoundProgressionSettings roundSettings)
        {
            if (IsInitialized)
            {
                throw new InvalidOperationException(
                    "GameState はすでに初期化されています。");
            }

            if (roundSettings == null)
            {
                throw new ArgumentNullException(nameof(roundSettings));
            }

            RoundSettings = roundSettings;

            OwnedModel = new MedalsOwnedModel(30);
            RefundNotifier = new MedalRefundNotifier();

            // RoundProgressionSettings は1始まりなので、開始値も1にそろえる。
            RoundManager = new RoundManager(startRound: 1);
            RoundService = new RoundAdvanceService();

            GamePause = new GamePause();

            RoundManager.OnRoundChanged += ApplyRoundRequirement;

            ApplyRoundRequirement(RoundManager.CurrentRound);

            IsInitialized = true;
        }

        private void ApplyRoundRequirement(int roundNumber)
        {
            if (roundNumber > RoundSettings.RoundCount)
            {
                Debug.Log($"全{RoundSettings.RoundCount}ラウンドをクリアしました。");
                return;
            }

            RoundRequirement requirement =
                RoundSettings.GetRequirement(roundNumber);

            RoundService.SetMedalCost(requirement.RequiredMedalCount);

            Debug.Log(
                $"Round {roundNumber} 開始: " +
                $"必要メダル={requirement.RequiredMedalCount}, " +
                $"必要ボール={requirement.RequiredBallCount}");
        }

        private void OnDestroy()
        {
            if (Instance != this)
            {
                return;
            }

            if (RoundManager != null)
            {
                RoundManager.OnRoundChanged -= ApplyRoundRequirement;
            }

            Instance = null;
        }
    }
}