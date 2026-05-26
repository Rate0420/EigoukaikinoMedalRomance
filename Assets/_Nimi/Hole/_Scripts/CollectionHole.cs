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
        public event Action<Medal> OnMedalCollected;

        public void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(_medalTag))
            {
                var medal = other.GetComponentInParent<Medal>();
                if (medal != null)
                {
                    medal.Collect();
                    // イベントを発行
                    OnMedalCollected?.Invoke(medal);
                }
                else
                {
                    Debug.LogWarning("メダルタグが付いているオブジェクトにMedalコンポーネントが見つかりませんでした", other.gameObject);
                }
            }
        }
    }
}