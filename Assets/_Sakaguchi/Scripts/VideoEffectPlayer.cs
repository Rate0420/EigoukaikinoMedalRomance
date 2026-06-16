using UnityEngine;
using System.Collections;
using UnityEngine.Video;

public class VideoEffectPlayer : MonoBehaviour
{
    VideoPlayer videoPlayer;
    Renderer videoRenderer;
    [SerializeField] float endlength = 0.5f; // 終了前にフェードアウトする時間

    public string[] videoPaths;

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

    public IEnumerator PlayVideoFadeInOut(int id, float playbackSpeed = 1f, float fadeintime = 0f, float fadeouttime = 0f)
    {
        if (id < 0 || id >= videoPaths.Length)
        {
            Debug.LogError("Invalid video ID");
            yield break;
        }

        videoPlayer.url = videoPaths[id];
        videoPlayer.playbackSpeed = playbackSpeed;
        videoRenderer.enabled = false;

        bool isPrepared = false;

        void OnPrepared(VideoPlayer vp)
        {
            isPrepared = true;
        }

        videoPlayer.prepareCompleted += OnPrepared;
        videoPlayer.Prepare();

        yield return new WaitUntil(() => isPrepared);

        videoPlayer.prepareCompleted -= OnPrepared; // ★超重要（イベント解除）

        // 最初から透明にする
        color.a = 0f;
        mat.color = color;

        videoRenderer.enabled = true;

        videoPlayer.Play();

        // ★フェードイン（0ならスキップ）
        if (fadeintime > 0f)
            yield return StartCoroutine(FadeIn(fadeintime));
        else
        {
            color.a = 1f;
            mat.color = color;
        }

        // ★length安定待ち（1フレーム）
        yield return null;

        double endTime = videoPlayer.length - fadeouttime;
        if (endTime < 0) endTime = 0;

        yield return new WaitUntil(() => videoPlayer.time >= endTime);

        // ★フェードアウト
        if (fadeouttime > 0f)
            yield return StartCoroutine(FadeOut(fadeouttime));
        else
        {
            color.a = 0f;
            mat.color = color;
        }

        videoPlayer.Stop();
        videoRenderer.enabled = false;
    }
    IEnumerator FadeIn(float time)
    {
        float t = 0f;

        while (t < time)
        {
            t += Time.deltaTime;
            float a = Mathf.Clamp01(t / time);
            color.a = a;
            mat.color = color;
            yield return null;
        }

        color.a = 1f;
        mat.color = color;
    }

    IEnumerator FadeOut(float time)
    {
        float t = 0f;

        while (t < time)
        {
            t += Time.deltaTime;
            float a = 1f - Mathf.Clamp01(t / time);
            color.a = a;
            mat.color = color;
            yield return null;
        }

        color.a = 0f;
        mat.color = color;
    }
}