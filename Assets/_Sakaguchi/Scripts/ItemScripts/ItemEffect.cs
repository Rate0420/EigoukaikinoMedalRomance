using UnityEngine;

// アイテムの効果を定義する抽象クラス

public abstract class ItemEffect : ScriptableObject
{
    public virtual void OnInventoryChanged(BuffItemContext context) { }

    public virtual void OnMedalShot(BuffItemContext context) { }
    public virtual void OnMedalLanded(BuffItemContext context,GameObject medal) { }
    public virtual void OnMedalLost(BuffItemContext context) { }

    public virtual void OnRoundStart(BuffItemContext context) { }
    public virtual void OnRoundEnd(BuffItemContext context) { }

    public virtual void OnSlotRoll(BuffItemContext context) { }
    public virtual void OnSlotWin(BuffItemContext context) { }

    public virtual void OnConsumptionItem(BuffItemContext context) { }
}