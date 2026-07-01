using System.Collections;
using TMPro;
using UnityEngine;

public class S_WriteText : MonoBehaviour
{
    [SerializeField] private S_StoryData storyData;
    [SerializeField] private S_SetStoryUI setStoryUI;
    [SerializeField] private S_ChooseManager chooseManager;

    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private GameObject nextArrowImage;
    [SerializeField] private GameObject fastUI;
    [SerializeField] private Animator fastAnim;

    [SerializeField] private float textSpeed = 0.1f;
    [SerializeField] private float fastTextSpeed = 0.04f;

    public int Index { get; private set; } = 0;

    private bool isFast;
    private bool isDrawing;
    private bool isSceneChanged;

    // true の間は入力を無視する（Start直後・JumpTo直後の誤発火防止）
    private bool inputBlocked = true;

    // ----------------------------------------------------------------

    private void Start()
    {
        storyData = S_DontDestroyStory.instance.story;
        if (fastAnim == null && fastUI != null)
            fastAnim = fastUI.GetComponent<Animator>();

        setStoryUI.Apply(Index);
        StartCoroutine(StartSequence());
    }

    private IEnumerator StartSequence()
    {
        yield return null;          // 1フレーム待つ
        inputBlocked = false;
        BeginEntry(Index);
    }

    private void Update()
    {
        if (inputBlocked) return;
        if (chooseManager.IsShowingChoices) return;

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            OnAdvanceInput();

        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.LeftShift))
        {
            isFast = !isFast;
            textSpeed = isFast ? fastTextSpeed : 0.1f;
            if (fastAnim != null) fastAnim.SetBool("ScaleBool", isFast);
        }
    }

    // ----------------------------------------------------------------

    private void OnAdvanceInput()
    {
        if (chooseManager.IsShowingChoices) return;

        if (isDrawing)
        {
            // 書いている最中 → 全文一括表示
            StopAllCoroutines();
            var currentEntry = storyData.Get(Index);
            messageText.text = currentEntry.text;
            isDrawing = false;

            // nextIndex が指定されていれば自動ジャンプ
            if (currentEntry.HasJump)
            {
                Index = currentEntry.nextIndex;
                HideNextArrow();
                BeginEntry(Index);
            }
            else
            {
                Index++;
                ShowNextArrow();
            }
            return;
        }

        if (Index < storyData.Length)
        {
            HideNextArrow();
            BeginEntry(Index);
            return;
        }

        // 全エントリ終了 → シーン遷移
        if (fastAnim != null) fastAnim.SetBool("ScaleBool", false);
        string nextScene = storyData.Get(storyData.Length - 1).nextScene;
        if (!string.IsNullOrEmpty(nextScene) && !isSceneChanged)
        {
            isSceneChanged = true;
            FadeSceneChanger.ChangeScene(nextScene);
        }
    }

    private void BeginEntry(int entryIndex)
    {
        StopAllCoroutines();
        isDrawing = false;
        messageText.text = "";
        StartCoroutine(CorDrawText(entryIndex));
    }

    /// <summary> 選択肢決定後にジャンプ先Indexから再開する（ChooseManagerから呼ぶ）</summary>
    public void JumpTo(int targetIndex)
    {
        Index = targetIndex;
        HideNextArrow();
        // ボタン押下クリックが同フレームのUpdateに届くのを防ぐため
        // 1フレームだけ入力をブロックしてからBeginEntryを呼ぶ
        StartCoroutine(JumpSequence());
    }

    private IEnumerator JumpSequence()
    {
        inputBlocked = true;
        yield return null;          // 1フレーム待つ（クリックイベントをやり過ごす）
        inputBlocked = false;
        BeginEntry(Index);
    }

    // ----------------------------------------------------------------

    private IEnumerator CorDrawText(int entryIndex)
    {
        isDrawing = true;

        var entry = storyData.Get(entryIndex);
        setStoryUI.Apply(entryIndex);

        if (entry.HasChoices)
        {
            messageText.text = entry.text;
            chooseManager.ShowChoices(entryIndex);  // IsShowingChoices=true を先に立てる
            isDrawing = false;
            yield break;
        }

        string message = entry.text;
        float time = 0f;
        while (true)
        {
            yield return null;
            time += Time.deltaTime;
            int length = Mathf.FloorToInt(time / textSpeed);
            if (length >= message.Length) break;
            messageText.text = message.Substring(0, length);
        }

        messageText.text = message;
        yield return null;

        isDrawing = false;

        // nextIndex が指定されていれば自動ジャンプ（次へボタンを押さずに飛ぶ）
        // nextIndex = -1 なら通常通り Index+1 へ進んで矢印を出して待機
        if (entry.HasJump)
        {
            // 自動ジャンプは入力でなくコードから呼ぶため inputBlocked は不要
            Index = entry.nextIndex;
            HideNextArrow();
            BeginEntry(Index);
        }
        else
        {
            Index++;
            ShowNextArrow();
        }
    }

    private void ShowNextArrow() { if (nextArrowImage != null) nextArrowImage.SetActive(true); }
    private void HideNextArrow() { if (nextArrowImage != null) nextArrowImage.SetActive(false); }
}
