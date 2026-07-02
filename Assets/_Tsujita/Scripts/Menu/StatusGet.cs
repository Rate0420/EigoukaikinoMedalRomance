using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusGet : MonoBehaviour
{
    [SerializeField] private CharacterDatabase characterDatabase;
    [SerializeField] private ItemDataBase itemDatabase;
    [SerializeField] private MenuManager menuManager;
    [SerializeField] private CharacterData[] datas;
    private CharacterData characterData;

    // 各キャラの好感度
    [SerializeField] private TextMeshProUGUI[] likeabilityTexts;     // 好感度一覧

    // メイン画面左のステータス一覧
    [SerializeField] private TextMeshProUGUI statusLikeability;     // ルートキャラ用好感度
    [SerializeField] private int nowStory;  // ストーリー進行度
    [SerializeField] private int miniStory; // ミニイベ進行度
    [SerializeField] private int nowMedal;  // 所持メダル
    [SerializeField] private TextMeshProUGUI nowStoryText;  // ストーリー進行度
    [SerializeField] private TextMeshProUGUI miniStoryText; // ミニイベ進行度
    [SerializeField] private TextMeshProUGUI nowMedalText;  // 所持メダル

    // バフ関連
    [SerializeField] private ItemData[] buffStats = new ItemData[3];    // セットされているバフ
    [SerializeField] private TextMeshProUGUI[] buffStatsTexts;          // バフ用テキスト
    [SerializeField] private TextMeshProUGUI[] buffNameTexts;           // バフ名テキスト
    [SerializeField] private GameObject buffPanel;                      // 確認パネル
    private int nowNo = -1;

    [SerializeField] private Image cutinSprite; // ルートキャラの画像

    /// <summary>
    /// 現在のステータスを反映
    /// </summary>
    public void SetStatus()
    {
        // ステータスタブへの反映
        // 各キャラの好感度
        for (int i = 0; i < datas.Length; i++)
        {
            likeabilityTexts[i].text = datas[i].likeability.ToString() + " / 100";
        }

        // バフ番号の取得
        // 効果内容の反映

        // 現在ルートのキャラ取得
        characterData =
            characterDatabase.GetCharacter(
                MenuManager.Instance.currentRoute
            );

        cutinSprite.sprite = characterData.cutinSprite;

        // ステータス取得
        nowStory = 1;
        miniStory = 1;
        nowMedal = 999999;

        // 画面左のステータス画面に反映
        nowStoryText.text = nowStory.ToString() + "/7";
        miniStoryText.text = miniStory.ToString() + "/8";
        statusLikeability.text = characterData.likeability.ToString();
        nowMedalText.text = nowMedal.ToString() + "枚";

        UpdateBuffUI();
    }

    /// <summary>
    /// バフUI更新
    /// </summary>
    private void UpdateBuffUI()
    {
        for (int i = 0; i < buffStats.Length; i++)
        {
            if (buffStats[i] != null)
            {
                buffNameTexts[i].text = buffStats[i].itemName;
                buffStatsTexts[i].text = buffStats[i].description;
            }
            else
            {
                buffNameTexts[i].text = "未設定";
                buffStatsTexts[i].text = "";
            }
        }
    }

    /// <summary>
    /// バフ追加
    /// </summary>
    public bool AddBuff(ItemType itemType)
    {
        ItemData item = itemDatabase.GetCharacter(itemType);

        if (item == null)
        {
            Debug.LogWarning("アイテムが見つかりません");
            return false;
        }

        // 空いているスロットを探す
        for (int i = 0; i < buffStats.Length; i++)
        {
            if (buffStats[i] == null)
            {
                buffStats[i] = item;

                UpdateBuffUI();

                Debug.Log(item.itemName + " を追加");
                return true;
            }
        }

        Debug.Log("バフ枠がいっぱいです");
        return false;
    }

    /// <summary>
    /// 指定スロットのバフ削除
    /// </summary>
    public void BuffDelete(int slotNo)
    {
        nowNo = slotNo;
        buffPanel.SetActive(true);
    }

    public void DeletChoice(int buttonNo)
    {
        switch(buttonNo)
        {
            case 0:
                if (nowNo < 0 || nowNo >= buffStats.Length)
                    return;

                buffStats[nowNo] = null;

                nowNo = -1;
                UpdateBuffUI();
                buffPanel.SetActive(false);
                break;
            case 1:
                nowNo = -1;
                buffPanel.SetActive(false);
                break;
        }
    }
}
