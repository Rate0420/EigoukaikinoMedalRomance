using System;
using EMR.Medal;
using UnityEngine;

namespace EMR.Hole
{
    /// <summary>
    /// 穴へのアイテム落下を管理するクラス
    /// </summary>
    public class HoleDropDetector
    {
        /// <summary>
        /// 回収可能なアイテムが穴に落ちたときに発火するイベント
        /// </summary>
        public event Action<ICollectable> CollectableDroppedIntoHole;

        /// <summary>
        /// アイテムが穴に落ちたことを通知する。
        /// </summary>
        /// <param name="collectable">穴に落ちたアイテム</param>
        public void NotifyCollectableDropped(ICollectable collectable)
        {
            if (collectable == null)
            {
                throw new ArgumentNullException(nameof(collectable));
            }

            Debug.Log("落下");

            CollectableDroppedIntoHole?.Invoke(collectable);
        }
    }
}