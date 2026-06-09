using UnityEngine;
using System.Collections;

public class ReelManager : MonoBehaviour
{
    [SerializeField] ReelController leftReel;
    [SerializeField] ReelController centerReel;
    [SerializeField] ReelController rightReel;

    [SerializeField] float reachtime;

    Coroutine stopCoroutine;
    bool isRunning = false;

    public void StartReels()
    {
        // 前のCoroutineを確実に止める
        if (stopCoroutine != null)
        {
            StopCoroutine(stopCoroutine);
            stopCoroutine = null;
        }

        isRunning = true;
        leftReel.StartSpin();
        centerReel.StartSpin();
        rightReel.StartSpin();
    }

    public void StartStopReels(int[] result)
    {
        if (!isRunning) return;
        stopCoroutine = StartCoroutine(StopReelsCoroutine(result));
    }

    IEnumerator StopReelsCoroutine(int[] result)
    {
        bool isReach = result[0] == result[2];

        // 最低でも1秒は回す
        yield return new WaitForSeconds(1.0f);

        leftReel.StopSpin(result[0]);
        yield return new WaitUntil(() => !leftReel.IsSpinning);

        yield return new WaitForSeconds(0.3f);

        rightReel.StopSpin(result[2]);

        float centerDecel = isReach ? 2.5f : 1f;
        yield return new WaitUntil(() => !rightReel.IsSpinning);

        if (isReach)
        {
            yield return new WaitForSeconds(reachtime);

        }
        else
        {
            yield return new WaitForSeconds(0.3f);
        }

        centerReel.StopSpin(result[1], centerDecel);
        yield return new WaitUntil(() => !centerReel.IsSpinning);

        isRunning = false;
    }
}