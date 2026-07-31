using UnityEngine;

[CreateAssetMenu(menuName = "Item")]
public class ItemData : ScriptableObject
{
    /// <summary>
    /// アイテムの種類
    /// </summary>
    public ItemType itemType;

    /// <summary>
    /// アイテム名
    /// </summary>
    public string itemName;

    /// <summary>
    /// 詳細画面などに表示する説明文
    /// </summary>
    [TextArea(3, 5)]
    public string description;

    /// <summary>
    /// 購入に必要なメダル数
    /// </summary>
    public int[] cost;

    /// <summary>
    /// 詳細画面に表示するアイコン(Sprite)
    /// </summary>
    public Sprite icon;

    /// <summary>
    /// 購入時にInventoryへ追加し、ゲーム中に効果を発揮する
    /// </summary>
    public ItemEffect effect;

    /// <summary>
    /// 消費アイテムかどうか
    /// </summary>
    public bool isConsumable;

    /// <summary>
    /// アイテムレベル
    /// </summary>
    public int level;
}