using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ストーリーシーン全体のデータを管理するスクリプタブルオブジェクト。
/// テキスト・立ち絵・背景・音声・選択肢をすべて1エントリ単位で保持する。
/// </summary>
[CreateAssetMenu(fileName = "S_StoryData", menuName = "Game/S_StoryData")]
public class S_StoryData : ScriptableObject
{
    [Header("ストーリーエントリ一覧")]
    public List<StoryEntry> entries;

    public int Length => entries != null ? entries.Count : 0;
    public StoryEntry Get(int i) => entries[i];

    // ============================================================
    //  StoryEntry : 1セリフ分のすべてのデータ
    // ============================================================
    [System.Serializable]
    public class StoryEntry
    {
        [Header("テキスト")]
        public string text;

        [Header("名前（\"none\" で非表示）")]
        public string charName;

        // ---- 立ち絵 ----
        [Header("キャラ1")]
        public Sprite char1Sprite;
        [Tooltip("キャラ1のX位置オフセット（px）")]
        public float char1OffsetX;
        [Tooltip("キャラ1のY位置オフセット（px）")]
        public float char1OffsetY;

        [Header("キャラ2")]
        public Sprite char2Sprite;
        [Tooltip("キャラ2のX位置オフセット（px）")]
        public float char2OffsetX;
        [Tooltip("キャラ2のY位置オフセット（px）")]
        public float char2OffsetY;

        // ---- 背景 ----
        [Header("背景")]
        public Sprite bgSprite;

        // ---- 音 ----
        [Header("BGM（null で変更なし）")]
        public AudioClip bgm;
        [Header("SE")]
        public AudioClip se;
        [Header("ボイス")]
        public AudioClip voice;

        // ---- 画面効果 ----
        [Header("画面効果ID（0 = なし）")]
        public int scEffect;

        // ---- シーン遷移 ----
        [Header("次シーン名（空 = 遷移なし）")]
        public string nextScene;

        // ---- 選択肢 ----
        [Header("選択肢（空リスト = 通常テキスト）")]
        public List<ChoiceEntry> choices;

        /// <summary> このエントリが選択肢を持つか </summary>
        public bool HasChoices => choices != null && choices.Count >= 2;
    }

    // ============================================================
    //  ChoiceEntry : 1選択肢分のデータ
    // ============================================================
    [System.Serializable]
    public class ChoiceEntry
    {
        [Header("ボタンに表示するテキスト")]
        public string label;

        [Header("好感度変化量（正 = 増加 / 負 = 減少）")]
        public int affinityDelta;

        [Header("選択後にジャンプするエントリIndex（-1 = 次のエントリへ）")]
        public int jumpToIndex = -1;
    }
}
