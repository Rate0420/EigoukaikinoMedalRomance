using System;

namespace EMR.Round
{
    /// <summary>
    /// ラウンドを管理するクラス
    /// </summary>
    public class RoundManager 
    {
        /// <summary>
        /// 現在のラウンド数。
        /// </summary>
        public int CurrentRound { get; private set; }

        /// <summary>
        /// ラウンド数が変更されたときに呼ばれるイベント。
        /// </summary>
        public event Action<int> OnRoundChanged;


        public RoundManager (int startRound = 0)
        {
            CurrentRound = Math.Max(0, startRound);
        }

        /// <summary>
        /// 次のラウンドへ進める。
        /// </summary>
        public void NextRound() => SetRound(CurrentRound + 1);

        /// <summary>
        /// 前のラウンドへ戻す。
        /// </summary>
        public void PrevRound() => SetRound(CurrentRound - 1);

        /// <summary>
        /// 指定したラウンド数に設定する。
        /// </summary>
        public void SetRound(int value)
        {
            int newRound = Math.Max(0, value);

            if (CurrentRound == newRound)
                return;

            CurrentRound = newRound;
            OnRoundChanged?.Invoke(CurrentRound);
        }
    }
}