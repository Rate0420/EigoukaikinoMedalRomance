using UnityEngine;
using TMPro;
using System.Collections;
using EMR.Medal.Refund;
using EMR.Core;

public class WinManager : MonoBehaviour
{
    [SerializeField] GameObject winUI;
    [SerializeField] TextMeshPro payoutText;
    [SerializeField] VideoEffectPlayer videoPlayer;

    [SerializeField] MedalRefundBehaviour refundBehaviour;


    public IEnumerator PlayWin(int resultNumber)
    {
        // ‡@ ‰‰o“®‰æ
        yield return videoPlayer.PlayVideoNoFadeCoroutine(0, 1.0f);

        // ‡A •¥‚¢o‚µ–‡”Œˆ’è
        int payout = GetPayout(resultNumber);

        // ‡B UI•\¦
        winUI.SetActive(true);
        payoutText.text = $"{payout}–‡";

        bool isFinished = false;

        System.Action<int> spawnHandler = null;
        System.Action finishHandler = null;

        // Update the remaining payout count per spawned medal.
        spawnHandler = (remaining) =>
        {
            payoutText.text = $"{remaining}–‡";
        };

        
        finishHandler = () => isFinished = true;

        if (refundBehaviour == null)
        {
            Debug.LogError($"{nameof(WinManager)}: {nameof(refundBehaviour)} is not assigned.");
            winUI.SetActive(false);
            yield break;
        }

        refundBehaviour.OnMedalSpawned += spawnHandler;
        refundBehaviour.OnRefundFinished += finishHandler;

        GameState.Instance.RefundNotifier.RequestRefund(payout);

        float timeout = Mathf.Max(10f, payout * 0.5f + 5f);
        float elapsed = 0f;
        while (!isFinished && elapsed < timeout)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        refundBehaviour.OnMedalSpawned -= spawnHandler;
        refundBehaviour.OnRefundFinished -= finishHandler;

        if (!isFinished)
        {
            Debug.LogWarning($"{nameof(WinManager)}: Refund did not finish within {timeout:0.0} seconds. Resuming slot processing.");
        }

        winUI.SetActive(false);

    }

    int GetPayout(int number)
    {
        // ‹ô”EŠï”‚Å•ªŠò
        if (number == 7) return 100;
        if (number % 2 == 0)
            return 30;
        else
            return 50;
    }
}
