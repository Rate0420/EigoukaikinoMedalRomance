using UnityEngine;
using System;

/// <summary>
/// 
/// </summary>
public static class ItemTriggerEvents
{
    /// <summary>
    /// インベントリに変更があった時に呼ばれるイベント
    /// </summary>
    public static Action OnInventoryChanged;

    /// <summary>
    /// メダル発射時に呼ばれるイベント
    /// </summary>
    public static Action OnMedalShot;

    /// <summary>
    /// 
    /// </summary>
    public static Action<GameObject> OnMedalLanded;

    /// <summary>
    /// 
    /// </summary>
    public static Action OnMedalLost;

    /// <summary>
    /// ラウンド開始時に呼ばれるイベント
    /// </summary>
    public static Action OnRoundStart;

    /// <summary>
    /// ラウンド終了時に呼ばれるイベント
    /// </summary>
    public static Action OnRoundEnd;

    /// <summary>
    /// 
    /// </summary>
    public static Action OnSlotRoll;

    /// <summary>
    /// 
    /// </summary>
    public static Action OnSlotWin;

    /// <summary>
    /// 消費アイテム選択時
    /// </summary>
    public static Action OnConsumptionItem;
}