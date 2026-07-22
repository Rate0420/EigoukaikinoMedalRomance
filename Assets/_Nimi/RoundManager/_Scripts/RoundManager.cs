using System;
using UnityEngine;

namespace EMR.Round
{
    /// <summary>
    /// 現在のラウンド番号を管理するクラス。
    /// ラウンド変更時にはイベントを通知する。
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


        public RoundManager (int startRound = 1)
        {
            SetRound(startRound);
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

            Debug.Log($"ラウンド進行 {CurrentRound}");
        }
    }
}