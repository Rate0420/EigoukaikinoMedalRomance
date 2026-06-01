using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] BuffItemContext context;

    [SerializeField] ItemData testdata;

    // テストで開始時にtestdataをインベントリに追加してみる
    private void Start()
    {
        AddItem(testdata);
    }

    // ItemDataクラスを持てるインベントリというリストのクラス

    public List<ItemData> inventory = new List<ItemData>();

    // アイテムをインベントリに追加するメソッド
    public void AddItem(ItemData item)
    {
        inventory.Add(item);
        ItemTriggerEvents.OnInventoryChanged?.Invoke();
    }

    // アイテムをインベントリから削除するメソッド
    public void RemoveItem(ItemData item)
    {
        if (inventory.Contains(item))
        {
            inventory.Remove(item);
            ItemTriggerEvents.OnInventoryChanged?.Invoke();
        }
    }



    void OnInventoryChanged()
    {
        foreach (ItemData invItem in inventory)
        {
            invItem.effect?.OnInventoryChanged(context);
        }
    }

    void OnMedalShot()
    {
        foreach (ItemData item in inventory)
        {
            if (item.effect != null)
            {
                item.effect.OnMedalShot(context); // プレイヤーに効果を適用
            }
            else
            {
            }
        }
    }

    void OnMedalLanded(GameObject medal)
    {
        foreach (ItemData item in inventory)
        {
            item.effect?.OnMedalLanded(context, medal);
        }
    }

    void OnMedalLost()
    {
        foreach (ItemData item in inventory)
        {
            if (item.effect != null)
            {
                item.effect.OnMedalLost(context); // プレイヤーに効果を適用
            }
            else
            {
            }
        }
    }

    void OnRoundStart()
    {
        foreach (ItemData item in inventory)
        {
            if (item.effect != null)
            {
                item.effect.OnRoundStart(context); // プレイヤーに効果を適用
            }
            else
            {
            }
        }
    }

    void OnRoundEnd()
    {
        foreach (ItemData item in inventory)
        {
            if (item.effect != null)
            {
                item.effect.OnRoundEnd(context); // プレイヤーに効果を適用
            }
            else
            {
            }
        }
    }

    void OnSlotRoll()
    {
        foreach (ItemData item in inventory)
        {
            if (item.effect != null)
            {
                item.effect.OnSlotRoll(context); // プレイヤーに効果を適用
            }
            else
            {
            }
        }
    }

    void OnSlotWin()
    {
        foreach (ItemData item in inventory)
        {
            if (item.effect != null)
            {
                item.effect.OnSlotWin(context); // プレイヤーに効果を適用
            }
            else
            {
            }
        }
    }

    private void OnConsumptionItem()
    {
        foreach (ItemData item in inventory)
        {
            if (item.effect != null)
            {
                item.effect.OnConsumptionItem(context); // プレイヤーに効果を適用
            }
            else
            {
            }
        }
    }

    private void Update()
    {
        // なんかメダル発射の所にPlayerInventoryを入れれないのでここで左クリック検知で全てのアイテムのOnMedalShotを呼び出す
        if (Input.GetMouseButtonDown(0))
        {
            ItemTriggerEvents.OnMedalShot?.Invoke();
        }

        // テストで右クリックでOnConsumptionItemを呼び出す
        if (Input.GetMouseButtonDown(1))
        {
            ItemTriggerEvents.OnConsumptionItem?.Invoke();
        }
    }

    void OnEnable()
    {
        ItemTriggerEvents.OnInventoryChanged += OnInventoryChanged;
        ItemTriggerEvents.OnMedalShot += OnMedalShot;
        ItemTriggerEvents.OnMedalLanded += OnMedalLanded;
        ItemTriggerEvents.OnMedalLost += OnMedalLost;
        ItemTriggerEvents.OnRoundStart += OnRoundStart;
        ItemTriggerEvents.OnRoundEnd += OnRoundEnd;
        ItemTriggerEvents.OnSlotRoll += OnSlotRoll;
        ItemTriggerEvents.OnSlotWin += OnSlotWin;
        ItemTriggerEvents.OnConsumptionItem += OnConsumptionItem;
    }

    void OnDisable()
    {
        ItemTriggerEvents.OnInventoryChanged -= OnInventoryChanged;
        ItemTriggerEvents.OnMedalShot -= OnMedalShot;
        ItemTriggerEvents.OnMedalLanded -= OnMedalLanded;
        ItemTriggerEvents.OnMedalLost -= OnMedalLost;
        ItemTriggerEvents.OnRoundStart -= OnRoundStart;
        ItemTriggerEvents.OnRoundEnd -= OnRoundEnd;
        ItemTriggerEvents.OnSlotRoll -= OnSlotRoll;
        ItemTriggerEvents.OnSlotWin -= OnSlotWin;
        ItemTriggerEvents.OnConsumptionItem -= OnConsumptionItem;
    }
}
