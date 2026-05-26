using NaughtyAttributes;
using System;
using UnityEngine;

namespace EMR.Medal.Hole
{
    // メダルを判定するクラス
    public class CollectionHole : MonoBehaviour
    {
        [SerializeField, Tag] string _medalTag = "Medal"; // メダルのタグ

        /// <summary>
        /// メダルを判定したときに発行されるイベント
        /// </summary>
        public event Action OnMedalCollected;

        public void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(_medalTag))
            {
                // イベントを発行
                OnMedalCollected?.Invoke();
            }
            else
            {
                Debug.Log("タグが一致していない対象が入った", other.gameObject);
            }
        }
    }
}