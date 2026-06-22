using UnityEngine;
using TMPro;
using System.Collections;

public class WinManager : MonoBehaviour
{
    [SerializeField] GameObject winUI;
    [SerializeField] TextMeshPro payoutText;
    [SerializeField] VideoEffectPlayer videoPlayer;

    public IEnumerator PlayWin(int resultNumber)
    {
        // ① 演出動画
        yield return videoPlayer.PlayVideoNoFadeCoroutine(0, 1.0f);

        // ② 払い出し枚数決定
        int payout = GetPayout(resultNumber);

        // ③ UI表示
        winUI.SetActive(true);
        payoutText.text = $"{payout}枚";

        // ④ 外部払い出し（ここは仮）
        yield return StartCoroutine(WaitPayout(payout));

        // ⑤ UI消す
        winUI.SetActive(false);
    }

    int GetPayout(int number)
    {
        // 偶数・奇数で分岐
        if (number % 2 == 0)
            return 50;
        else
            return 30;
    }

    IEnumerator WaitPayout(int amount)
    {
        // 仮：3秒待つ（あとで差し替え）
        yield return new WaitForSeconds(3f);
    }
}