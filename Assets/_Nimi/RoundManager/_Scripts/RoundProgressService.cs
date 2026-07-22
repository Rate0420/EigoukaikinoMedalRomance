using System;
using UnityEngine;

namespace EMR.Round
{
    /// <summary>
    /// ラウンド内でのメダル消費数を管理し、
    /// 必要数に達した場合にラウンドを進行させるサービス。
    /// </summary>
    public class RoundProgressService
    {
        /// <summary>
        /// ラウンド管理
        /// </summary>
        private readonly RoundManager _roundManager;

        /// <summary>
        /// ラウンド進行に必要なボール数
        /// </summary>
        public int RequiredBallCount { get; private set; }

        /// <summary>
        /// ラウンド進行に必要なメダル数
        /// </summary>
        public int RequiredMedalCount { get; private set; }

        /// <summary>
        /// 現在のラウンドで落下したボール数
        /// </summary>
        public int DroppedBallCount { get; private set; }



        /// <summary>
        /// 現在ラウンドで消費したメダル数
        /// </summary>
        public int ConsumedMedals { get; private set; }

        /// <summary>
        /// ラウンド進行までに必要な残りボール数
        /// </summary>
        public int RemainingBalls =>
            Math.Max(0, RequiredBallCount - DroppedBallCount);

        /// <summary>
        /// ラウンド進行までに必要な残りメダル数
        /// </summary>
        public int RemainingMedals =>
            Math.Max(0, RequiredMedalCount - ConsumedMedals);


        /// <summary>
        /// ボールを取得したとき
        /// </summary>
        public event Action<int> OnBallsDropped;

        /// <summary>
        /// メダルを消費した時
        /// </summary>
        public event Action<int> OnMedalsConsumed;

        /// <summary>
        /// ラウンドが進んだ時
        /// </summary>
        public event Action<int> OnRoundAdvanced;



        public RoundProgressService(RoundManager manager)
        {
            _roundManager = manager;
        }

        /// <summary>
        /// ラウンド進行条件を更新する
        /// </summary>
        public void SetRequirement(RoundRequirement requirement)
        {
            RequiredBallCount = requirement.RequiredBallCount;
            RequiredMedalCount = requirement.RequiredMedalCount;
            Debug.Log($"NextRound {_roundManager.CurrentRound+1} -> Ball:{RequiredBallCount}, Medal:{RequiredMedalCount}");
        }

        /// <summary>
        /// メダルを消費する。
        /// </summary>
        public void ConsumeMedals(int amount)
        {
            if (amount <= 0)
                return;

            ConsumedMedals += amount;

            OnMedalsConsumed?.Invoke(amount);

            CheckRoundAdvance();
        }


        /// <summary>
        /// ボールを取得した数を加算する。
        /// </summary>
        public void AddDroppedBalls(int amount)
        {
            if (amount <= 0)
                return;

            DroppedBallCount += amount;

            CheckRoundAdvance();
        }

        /// <summary>
        /// ラウンド進行条件を満たしているか確認する。
        /// </summary>
        private void CheckRoundAdvance()
        {
            if (ConsumedMedals < RequiredMedalCount) return;

            if (DroppedBallCount < RequiredBallCount) return;

            _roundManager.NextRound();

            // 次ラウンド用にリセット
            ConsumedMedals = 0;
            DroppedBallCount = 0;

            OnRoundAdvanced?.Invoke(_roundManager.CurrentRound);
        }
    }
}