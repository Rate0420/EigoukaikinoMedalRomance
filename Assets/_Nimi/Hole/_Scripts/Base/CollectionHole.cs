using NaughtyAttributes;
using System;
using UnityEngine;

namespace EMR.Medal.Hole
{
    // メダルを判定するクラス
    public class CollectionHole : MonoBehaviour
    {
        [SerializeField, Tag] string _medalTag = "Medal"; // メダルのタグ
        [SerializeField] bool _isJackSpot = false; // ジャックスポットかどうか
        [SerializeField] bool _isCount = false; // カウントするかどうか


        /// <summary>
        /// メダルを判定したときに発行されるイベント
        /// </summary>
        public event Action<Medal> OnMedalCollected;

        /// <summary>
        /// メダルを判定したときに、メダルと当たったワールド座標を渡すイベント
        /// </summary>
        public event Action<Medal, Vector3> OnMedalCollectedAt;

        /// <summary>
        /// メダルが落ちた位置
        /// </summary>
        public Vector3 HitPosition { get; private set; }


        public void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(_medalTag))
            {
                var medal = other.GetComponentInParent<Medal>();
                if (medal != null)
                {
                    HitPosition = other.ClosestPoint(transform.position);
                    medal.Collect();

                    if (_isCount)
                    {
                        OnMedalCollected?.Invoke(medal);
                        OnMedalCollectedAt?.Invoke(medal, HitPosition);
                    }
                }
                else
                {
                    Debug.LogWarning("メダルタグが付いているオブジェクトにMedalコンポーネントが見つかりませんでした", other.gameObject);
                }
            }
        }
    }
}