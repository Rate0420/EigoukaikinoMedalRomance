using System;

namespace EMR.Medal
{
    /// <summary>
    /// 現在所持しているメダルの数を管理するクラス
    /// </summary>
    public class MedalsOwnedModel
    {
        /// <summary>
        /// 現在所持しているメダル数
        /// </summary>
        public int Count { get; private set; }


        /// <summary>
        /// 所持メダル数が変更されたときに呼び出されるイベント
        /// </summary>
        public event Action<int> OnCountChanged;


        /// <summary>
        /// 現在のメダル数から+1するメソッド
        /// </summary>
        public void AddMedal()
        {
            AddMedal(1);
        }


        /// <summary>
        /// 現在のメダル数から指定した数だけ増やすメソッド
        /// </summary>
        /// <param name="count">増やす数</param>
        /// <exception cref="ArgumentException">増やす数は負の数にすることはできません</exception>
        public void AddMedal(int count)
        {
            if (count < 0) throw new ArgumentException("増やす数は負の数にすることはできません", nameof(count));

            Count += count;
            OnCountChanged?.Invoke(Count);
        }


        /// <summary>
        /// 現在のメダル数から-1するメソッド
        /// </summary>
        public void RemoveMedal()
        {
            RemoveMedal(1);
        }


        /// <summary>
        /// 現在のメダル数から指定した数だけ減らすメソッド
        /// </summary>
        /// <param name="count">減らす数</param>
        /// <exception cref="ArgumentException">減らす数は負の数にすることはできません</exception>
        public void RemoveMedal(int count)
        {
            if (count < 0) throw new ArgumentException("減らす数は負の数にすることはできません", nameof(count));

            Count = Math.Max(0, Count - count);
            OnCountChanged?.Invoke(Count);
        }
    }
}