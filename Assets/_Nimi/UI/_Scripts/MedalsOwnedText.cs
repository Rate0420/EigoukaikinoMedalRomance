using TMPro;
using UnityEngine;

namespace EMR.Medal.UI
{
    // 所持メダルを表示するためのクラス
    public class MedalsOwnedText : MonoBehaviour
    {
        [SerializeField] TMP_Text _medalsOwnedText; // 所持メダル数を表示するテキスト

        public void SetMedalsOwnedCount(int medalOwnedCount)
        {
            // テキストに所持メダル数を表示
            _medalsOwnedText.text = $"{medalOwnedCount.ToString("D5")}<size=30>枚";
        }
    }
}