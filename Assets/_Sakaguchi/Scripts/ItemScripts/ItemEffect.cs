using UnityEngine;

/// <summary>
/// アイテムの効果を定義する抽象クラス
/// </summary>
public abstract class ItemEffect : ScriptableObject
{
    /// <summary>
    /// インベントリの内容が変わった時
    /// </summary>
    /// <param name="context"></param>
    public virtual void OnInventoryChanged(ItemEffectContext context) { }

    /// <summary>
    /// メダルを発射した時
    /// </summary>
    /// <param name="context"></param>
    public virtual void OnMedalShot(ItemEffectContext context) { }
    /// <summary>
    /// メダルが着地した時
    /// </summary>
    /// <param name="context"></param>
    /// <param name="medal"></param>
    public virtual void OnMedalLanded(ItemEffectContext context, GameObject medal) { }
    /// <summary>
    /// メダルが横穴に落ちたとき
    /// </summary>
    /// <param name="context"></param>
    public virtual void OnMedalLost(ItemEffectContext context) { }

    /// <summary>
    /// ラウンドがスタート時
    /// </summary>
    /// <param name="context"></param>
    public virtual void OnRoundStart(ItemEffectContext context) { }
    /// <summary>
    /// ラウンドが終わった時
    /// </summary>
    /// <param name="context"></param>
    public virtual void OnRoundEnd(ItemEffectContext context) { }

    /// <summary>
    /// スロットが回った時
    /// </summary>
    /// <param name="context"></param>
    public virtual void OnSlotRoll(ItemEffectContext context) { }
    /// <summary>
    /// スロットで当たった際
    /// </summary>
    /// <param name="context"></param>
    public virtual void OnSlotWin(ItemEffectContext context) { }

    /// <summary>
    /// 消費アイテム用トリガー
    /// </summary>
    /// <param name="context"></param>
    public virtual void OnConsumptionItem(ItemEffectContext context) { }
}
