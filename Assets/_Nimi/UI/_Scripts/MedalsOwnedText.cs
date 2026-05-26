using TMPro;
using UnityEngine;
using EMR.Core;

namespace EMR.Medal.UI
{
    // 所持メダルを表示するためのクラス
    public class MedalsOwnedText : MonoBehaviour
    {
        [SerializeField] TMP_Text _medalsOwnedText; // 所持メダル数を表示するテキスト

        private void OnEnable()
        {
            GameState.Instance.OwnedModel.OnCountChanged += SetMedalsOwnedCount;
        }

        private void OnDisable()
        {
            GameState.Instance.OwnedModel.OnCountChanged -= SetMedalsOwnedCount;
        }

        public void SetMedalsOwnedCount(int medalOwnedCount)
        {
            // テキストに所持メダル数を表示
            _medalsOwnedText.text = $"{medalOwnedCount.ToString("D5")}<size=30>枚";
        }
    }
}