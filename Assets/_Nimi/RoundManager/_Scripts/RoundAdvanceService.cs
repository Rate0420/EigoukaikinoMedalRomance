using System;

namespace EMR.Round
{
    /// <summary>
    /// ラウンド内でのメダル消費数を管理し、
    /// 必要数に達した場合にラウンドを進行させるサービス。
    /// </summary>
    public class RoundAdvanceService
    {
        /// <summary>
        /// ラウンドを進めるために必要なメダル数。
        /// </summary>
        public int MedalCost { get; private set; } = -1;

        /// <summary>
        /// 現在のラウンドで消費したメダル数。
        /// </summary>
        public int CurrentRoundConsumedMedalCount { get; private set; }

        /// <summary>
        /// ラウンド進行までに残っている必要メダル数。
        /// </summary>
        public int RemainingMedalCount =>
            Math.Max(0, MedalCost - CurrentRoundConsumedMedalCount);

        /// <summary>
        /// メダルが不足してラウンドを進められなかったときに呼ばれるイベント。
        /// 引数は不足しているメダル数。
        /// </summary>
        public event Action<int> OnMedalShortage;

        /// <summary>
        /// ラウンドが進行したときに呼ばれるイベント。
        /// 引数は進行後のラウンド数。
        /// </summary>
        public event Action<int> OnRoundAdvanced;



        /// <summary>
        /// ラウンド進行に必要なメダル数を設定する。
        /// </summary>
        /// <param name="medalCost">必要なメダル数</param>
        public void SetMedalCost(int medalCost)
        {
            MedalCost = Math.Max(0, medalCost);
        }


        /// <summary>
        /// 現在のラウンドで消費したメダル数を加算する。
        /// </summary>
        /// <param name="count">加算する消費メダル数</param>
        public void AddConsumedMedals(int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(count),
                    "加算するメダル数は0以上にしてください。");
            }

            // 必要数を超えてカウントしない。
            CurrentRoundConsumedMedalCount = Math.Min(
                MedalCost,
                CurrentRoundConsumedMedalCount + count);
        }
    }
}