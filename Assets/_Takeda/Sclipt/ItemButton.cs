using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemButton : MonoBehaviour
{
    [HideInInspector]
    public ItemData itemData;

    public BuffShopManager shopManager;

    Button button;

    public Transform iconRoot;

    public TMP_Text nameText;
    public TMP_Text costText;

    GameObject currentIcon;

    void Awake()
    {
        button = GetComponent<Button>();

        if (button != null)
        {
            button.onClick.AddListener(Select);
        }
    }

    public void SetItem(ItemData item)
    {
        itemData = item;

        nameText.text = item.itemName;
        costText.text = item.cost + "枚";

        // 前のアイコン削除
        if (currentIcon != null)
            Destroy(currentIcon);

        // 新しいアイコン生成
        if (item.buttonIconPrefab != null)
        {
            currentIcon = Instantiate(item.buttonIconPrefab, iconRoot, false);

            RectTransform rect = currentIcon.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchoredPosition = Vector2.zero;
                rect.localScale = Vector3.one;
            }
        }
    }

    public void Select()
    {
        if (itemData != null)
            shopManager.SelectItem(itemData);
    }
}