using UnityEngine;

// ItemTriggerEventsを購読し、発火したタイミングで
// PlayerInventory内の全アイテムのItemEffectを呼び出すクラス。
//
// 役割分担:
// ・「何を持っているか」        → PlayerInventory
// ・「イベントが起きたら何をするか」 → こちら(ItemEffectDispatcher)
public class ItemEffectDispatcher : MonoBehaviour
{
    [SerializeField] PlayerInventory inventory;
    [SerializeField] ItemEffectContext context;

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

    void OnInventoryChanged()
    {
        foreach (var item in inventory.Items)
        {
            item.effect?.OnInventoryChanged(context);
        }
    }

    void OnMedalShot()
    {
        foreach (var item in inventory.Items)
        {
            item.effect?.OnMedalShot(context);
        }
    }

    void OnMedalLanded(GameObject medal)
    {
        foreach (var item in inventory.Items)
        {
            item.effect?.OnMedalLanded(context, medal);
        }
    }

    void OnMedalLost()
    {
        foreach (var item in inventory.Items)
        {
            item.effect?.OnMedalLost(context);
        }
    }

    void OnRoundStart()
    {
        foreach (var item in inventory.Items)
        {
            item.effect?.OnRoundStart(context);
        }
    }

    void OnRoundEnd()
    {
        foreach (var item in inventory.Items)
        {
            item.effect?.OnRoundEnd(context);
        }
    }

    void OnSlotRoll()
    {
        foreach (var item in inventory.Items)
        {
            item.effect?.OnSlotRoll(context);
        }
    }

    void OnSlotWin()
    {
        foreach (var item in inventory.Items)
        {
            item.effect?.OnSlotWin(context);
        }
    }

    void OnConsumptionItem()
    {
        foreach (var item in inventory.Items)
        {
            item.effect?.OnConsumptionItem(context);
        }
    }
}
