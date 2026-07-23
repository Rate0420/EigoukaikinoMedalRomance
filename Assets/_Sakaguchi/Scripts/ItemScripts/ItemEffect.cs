using UnityEngine;

// アイテムの効果を定義する抽象クラス
public abstract class ItemEffect : ScriptableObject
{
    // インベントリの内容が変わった時
    public virtual void OnInventoryChanged(ItemEffectContext context) { }

    // メダルを発射した時
    public virtual void OnMedalShot(ItemEffectContext context) { }
    // メダルが着地した時
    public virtual void OnMedalLanded(ItemEffectContext context, GameObject medal) { }
    // メダルが横穴に落ちたとき
    public virtual void OnMedalLost(ItemEffectContext context) { }

    // ラウンドがスタート時
    public virtual void OnRoundStart(ItemEffectContext context) { }
    // ラウンドが終わった時
    public virtual void OnRoundEnd(ItemEffectContext context) { }

    // スロットが回った時
    public virtual void OnSlotRoll(ItemEffectContext context) { }
    // スロットで当たった際
    public virtual void OnSlotWin(ItemEffectContext context) { }

    // 消費アイテム用トリガー
    public virtual void OnConsumptionItem(ItemEffectContext context) { }
}
