using UnityEngine;
using System.Collections.Generic;

// 所持アイテムのリストを管理するクラス。
// 「イベントが起きたら効果を呼ぶ」処理は ItemEffectDispatcher が担当するので、
// ここでは追加/削除と、変更通知だけを行う。
public class PlayerInventory : MonoBehaviour
{
    [SerializeField] ItemData testdata;

    public List<ItemData> Items { get; } = new List<ItemData>();

    private void Start()
    {
        // テストで開始時にtestdataをインベントリに追加してみる
        AddItem(testdata);
    }

    /// <summary>
    /// 指定したアイテムを追加する
    /// </summary>
    /// <param name="item"></param>
    public void AddItem(ItemData item)
    {
        Items.Add(item);
        ItemTriggerEvents.OnInventoryChanged?.Invoke();
    }

    /// <summary>
    ///  指定したアイテムを削除する
    /// </summary>
    /// <param name="item"></param>
    public void RemoveItem(ItemData item)
    {
        if (Items.Contains(item))
        {
            Items.Remove(item);
            ItemTriggerEvents.OnInventoryChanged?.Invoke();
        }
    }
}
