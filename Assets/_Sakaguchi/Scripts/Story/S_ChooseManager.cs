using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// StoryData の ChoiceEntry に基づき選択肢UIを表示し、結果を WriteText と AffinityManager に渡す。
/// </summary>
public class S_ChooseManager : MonoBehaviour
{
    [SerializeField] private S_StoryData storyData;
    [SerializeField] private S_WriteText writeText;
    [SerializeField] private S_AffinityManager affinityManager;

    [Header("2択UI")]
    [SerializeField] private CanvasGroup twoChoiceGroup;
    [SerializeField] private List<Button> twoChoiceBtns;

    [Header("3択UI")]
    [SerializeField] private CanvasGroup threeChoiceGroup;
    [SerializeField] private List<Button> threeChoiceBtns;

    public bool IsShowingChoices { get; private set; } = false;

    // ボタン押下時に参照するエントリIndex
    // ShowChoices()で設定し、OnChoiceButtonPressed()の先頭で使う
    // currentEntryIndex = -1 のリセットは JumpTo完了後ではなく
    // OnChoiceButtonPressed の「最初」に一時退避して使うことで競合を解消する
    private int currentEntryIndex = -1;

    // ----------------------------------------------------------------

    private void Start()
    {
        storyData = S_DontDestroyStory.instance.story;
        HideAll();
    }

    // ----------------------------------------------------------------

    /// <summary> 指定エントリの選択肢を表示する（WriteText から呼ばれる）</summary>
    public void ShowChoices(int entryIndex)
    {
        var entry = storyData.Get(entryIndex);
        if (!entry.HasChoices) return;

        // entryIndexを先に確定させてからUI構築
        currentEntryIndex = entryIndex;
        IsShowingChoices = true;

        int count = entry.choices.Count;
        if (count == 2)
        {
            SetupButtons(twoChoiceBtns, entry.choices);
            Show(twoChoiceGroup, twoChoiceBtns[0].gameObject);
        }
        else if (count == 3)
        {
            SetupButtons(threeChoiceBtns, entry.choices);
            Show(threeChoiceGroup, threeChoiceBtns[0].gameObject);
        }
        else
        {
            Debug.LogWarning($"[ChooseManager] 選択肢数 {count} は未対応です（2か3にしてください）");
        }
    }

    /// <summary> ボタン押下時の処理（引数は0始まりのIndex）</summary>
    public void OnChoiceButtonPressed(int choiceIndex)
    {
        // ★ 先に退避してからリセット → JumpTo内でShowChoicesが呼ばれても競合しない
        int resolvedEntry = currentEntryIndex;
        currentEntryIndex = -1;

        if (resolvedEntry < 0) return;

        var choices = storyData.Get(resolvedEntry).choices;
        if (choiceIndex < 0 || choiceIndex >= choices.Count) return;

        var choice = choices[choiceIndex];

        affinityManager?.AddAffinity(choice.affinityDelta);

        HideAll();
        IsShowingChoices = false;

        // ジャンプ先が指定されていればそこへ、なければ次のエントリへ
        int nextIndex = choice.jumpToIndex >= 0 ? choice.jumpToIndex : resolvedEntry + 1;

        // WriteText.JumpTo → BeginEntry → CorDrawText の順で実行される
        // この時点で currentEntryIndex は -1 なので、内部でShowChoicesが呼ばれても
        // currentEntryIndex は ShowChoices内で正しく上書きされる
        writeText.JumpTo(nextIndex);
    }

    // ----------------------------------------------------------------

    private void SetupButtons(List<Button> buttons, List<S_StoryData.ChoiceEntry> choices)
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            if (i >= choices.Count) continue;
            var label = buttons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (label != null) label.text = choices[i].label;

            int captured = i;
            buttons[i].onClick.RemoveAllListeners();
            buttons[i].onClick.AddListener(() => OnChoiceButtonPressed(captured));
        }
    }

    private void Show(CanvasGroup group, GameObject firstSelected)
    {
        group.alpha = 1f;
        group.interactable = true;
        group.blocksRaycasts = true;
        EventSystem.current.SetSelectedGameObject(firstSelected);
    }

    private void HideAll()
    {
        Hide(twoChoiceGroup);
        Hide(threeChoiceGroup);
    }

    private void Hide(CanvasGroup group)
    {
        if (group == null) return;
        group.alpha = 0f;
        group.interactable = false;
        group.blocksRaycasts = false;
    }
}