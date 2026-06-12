using UnityEngine;
using System.Collections;
using UnityEngine.Video;

public class VideoEffectPlayer : MonoBehaviour
{
    VideoPlayer videoPlayer;
    Renderer videoRenderer;
    [SerializeField] float endlength = 0.5f; // 終了前にフェードアウトする時間

    [SerializeField] string[] videoPaths;

    Material mat;
    Color color;

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoRenderer = GetComponent<Renderer>();

        mat = videoRenderer.material;
        color = mat.color;
    }

    // 外部からコルーチンで呼び出す用
    public IEnumerator PlayVideoCoroutine(int id, float playbackSpeed = 1f)
    {
        if (id < 0 || id >= videoPaths.Length)
        {
            Debug.LogError("Invalid video ID");
            yield break;
        }

        bool isPrepared = false;
        bool isFinished = false;

        videoPlayer.url = videoPaths[id];
        videoPlayer.playbackSpeed = playbackSpeed;

        videoRenderer.enabled = false;

        // 準備完了イベント
        videoPlayer.prepareCompleted += (vp) =>
        {
            isPrepared = true;
        };

        // 再生終了イベント
        videoPlayer.loopPointReached += (vp) =>
        {
            isFinished = true;
        };

        videoPlayer.Prepare();

        // 準備完了後
        yield return new WaitUntil(() => isPrepared);

        // ★ここでリセット
        color.a = 1f;
        mat.color = color;

        videoRenderer.enabled = true;
        videoPlayer.Play();

        // 再生終了待ち
        yield return new WaitUntil(() => isFinished);
        yield return new WaitUntil(() => !videoPlayer.isPlaying);

        // フェードアウト
        yield return StartCoroutine(FadeOut(1.0f)); // 1秒でフェード

        videoRenderer.enabled = false;

    }

    public IEnumerator PlayVideoNoFadeCoroutine(int id, float playbackSpeed = 1f)
    {
        if (id < 0 || id >= videoPaths.Length)
        {
            Debug.LogError("Invalid video ID");
            yield break;
        }

        bool isPrepared = false;

        videoPlayer.url = videoPaths[id];
        videoPlayer.playbackSpeed = playbackSpeed;

        videoRenderer.enabled = false;

        videoPlayer.prepareCompleted += (vp) =>
        {
            isPrepared = true;
        };

        videoPlayer.Prepare();

        yield return new WaitUntil(() => isPrepared);

        color.a = 1f;
        mat.color = color;

        videoRenderer.enabled = true;
        videoPlayer.Play();

        // こっちは最後まで再生
        yield return new WaitUntil(() => !videoPlayer.isPlaying);

        videoRenderer.enabled = false;
    }

    public IEnumerator WaitEarlyEndCoroutine(float earlyEndTime)
    {
        // length取れるまで待つ（重要）
        yield return new WaitUntil(() => videoPlayer.isPrepared);

        double endTime = videoPlayer.length - earlyEndTime;
        if (endTime < 0) endTime = 0;

        yield return new WaitUntil(() => videoPlayer.time >= endTime);
    }

    IEnumerator FadeOut(float duration)
    {


        float startAlpha = color.a;
        float time = 0f;

        while (time < duration)
        {
            float t = time / duration;
            color.a = Mathf.Lerp(startAlpha, 0f, t);
            mat.color = color;

            time += Time.deltaTime;
            yield return null;
        }

    }
}