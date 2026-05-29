using UnityEngine;

namespace EMR.Medal
{
    public class Medal : MonoBehaviour
    {
        [SerializeField] MedalInfo _info;

        /// <summary>
        /// メダルの情報を取得します。
        /// </summary>
        public MedalInfo Info => _info;

        /// <summary>
        /// メダルのカウント数を取得します。
        /// </summary>
        public int Count => _info.Count;

        /// <summary>
        /// メダルを獲得したときのメソッド
        /// </summary>
        public void Collect()
        {
            Destroy(gameObject);
        }
    }
}