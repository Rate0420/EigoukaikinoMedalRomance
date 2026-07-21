using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EMR.Core;

public class BuffShopManager : MonoBehaviour
{

    [Header("リロールコスト")]
    public int rerollCost = 50;

    [Header("全アイテム")]
    [SerializeField] private ItemData[] allBuffs;    // バフアイテム用データ
    [SerializeField] private ItemData[] allItems;    // 消費アイテム用データ

    [Header("現在表示中")]
    public ItemData[] currentBuffs = new ItemData[4];
    public ItemData[] currentItems = new ItemData[4];

    [Header("アイテムボタン")]
    public ItemButton[] buffButtons;
    public ItemButton[] itemButtons;

    [Header("詳細パネル")]
    public GameObject detailPanel;

    public Image detailIcon;
    public TMP_Text detailName;
    public TMP_Text detailDesc;
    public TMP_Text costText;

    [Header("アイテム種類")]
    public TMP_Text itemTypeText;

    [Header("所持メダル表示")]
    public TMP_Text medalText;

    [Header("購入ボタン")]
    public Button buyButton;

    private ItemData currentItem;

    [SerializeField] private StatusGet statusGet;
    [SerializeField] private TextMeshProUGUI levelText;
    ItemType itemType;

    void Start()
    {
        // 最初は詳細を隠す
        detailPanel.SetActive(false);

        // 購入ボタン無効
        buyButton.interactable = false;

        // 初回ショップ生成
        RerollFree();

        for (int i = 0; i < buffButtons.Length; i++)
        {
            buffButtons[i].OnButtonClicked += SelectItem;
        }

        for (int i = 0; i < itemButtons.Length; i++)
        {
            itemButtons[i].OnButtonClicked += SelectItem;
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < buffButtons.Length; i++)
        {
            buffButtons[i].OnButtonClicked -= SelectItem;
        }

        for (int i = 0; i < itemButtons.Length; i++)
        {
            itemButtons[i].OnButtonClicked -= SelectItem;
        }
    }

    private void OnEnable()
    {
        for (int i = 0; i < buffButtons.Length; i++)
        {
            buffButtons[i].OnButtonClicked += SelectItem;
        }

        for (int i = 0; i < itemButtons.Length; i++)
        {
            itemButtons[i].OnButtonClicked += SelectItem;
        }
    }

    //------------------------------------
    // アイテム選択
    //------------------------------------
    public void SelectItem(ItemData item)
    {
        currentItem = item;

        detailPanel.SetActive(true);

        detailIcon.sprite = item.icon;
        detailName.text = item.itemName;
        detailDesc.text = item.description;
        costText.text = item.cost + "枚";
        levelText.text = "Lv." + item.level; 

        if (item.isConsumable)
        {
            itemTypeText.text = "消費アイテム";
        }
        else
        {
            itemTypeText.text = "バフアイテム";
        }

        UpdateBuyButton();
    }

    //------------------------------------
    // 購入
    //------------------------------------
    public void BuyItem()
    {
        if (currentItem == null)
            return;

        if (GameState.Instance.OwnedModel.Count < 0)
        {
            Debug.Log("メダル不足");
            return;
        }

        // バフアイテム
        if (!currentItem.isConsumable)
        {
            itemType = currentItem.itemType;

            if (!statusGet.AddBuff(itemType))
                return;

            for (int i = 0; i < currentBuffs.Length; i++)
            {
                if (currentBuffs[i] == currentItem && !statusGet.isBuff)
                {
                    buffButtons[i].gameObject.SetActive(false);
                    break;
                }
            }

        }
        // 消費アイテム
        else 
        {
            for (int i = 0; i < currentItems.Length; i++)
            {
                if (currentItems[i] == currentItem)
                {
                    itemButtons[i].gameObject.SetActive(false);
                    break;
                }
            }
            Debug.Log("消費");
        }

        // メダルの支払い
        GameState.Instance.OwnedModel.RemoveMedal(currentItem.cost);

        currentItem = null;

        detailPanel.SetActive(false);

        buyButton.interactable = false;
    }
    //------------------------------------
    // 購入ボタン更新
    //------------------------------------
    void UpdateBuyButton()
    {
        if (currentItem == null)
        {
            buyButton.interactable = false;
            return;
        }

        buyButton.interactable =
            GameState.Instance.OwnedModel.Count >= currentItem.cost;
    }
    //------------------------------------
    // リロール
    //------------------------------------
    public void Reroll()
    {
        if (GameState.Instance.OwnedModel.Count < 50)
        {
            Debug.Log("メダル不足");
            return;
        }

        // メダルの支払い
        GameState.Instance.OwnedModel.RemoveMedal(rerollCost);

        RerollFree();

        currentItem = null;

        detailPanel.SetActive(false);

        buyButton.interactable = false;
    }

    //------------------------------------
    // 重複なしでショップ生成
    //------------------------------------
    void RerollFree()
    {
        ResetButton();

        // バフアイテムの生成
        List<ItemData> pool = new List<ItemData>(allBuffs);
        for (int i = 0; i < buffButtons.Length; i++)
        {
            if (pool.Count == 0)
                break;

            int randomIndex = Random.Range(0, pool.Count);
            
            currentBuffs[i] = pool[randomIndex];

            buffButtons[i].SetItem(currentBuffs[i]);
            pool.RemoveAt(randomIndex);
        }

        // 消費アイテムの生成
        List<ItemData> pool2 = new List<ItemData>(allItems);
        for (int i = 0; i < itemButtons.Length; i++)
        {
            if (pool2.Count == 0)
                break;

            int randomIndex = Random.Range(0, pool2.Count);

            currentItems[i] = pool2[randomIndex];

            itemButtons[i].SetItem(currentItems[i]);

            pool2.RemoveAt(randomIndex);
        }
    }
    public void RefreshUI()
    {
        UpdateBuyButton();
    }

    /// <summary>
    /// リロール時に、非表示にしたボタンを表示する
    /// </summary>
    private void ResetButton()
    {
        for(int i = 0; i < buffButtons.Length; i++)
        {
            buffButtons[i].gameObject.SetActive(true);
            itemButtons[i].gameObject.SetActive(true);
        }
    }
}