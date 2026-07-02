using UnityEngine;

[CreateAssetMenu(menuName = "Item")]
public class ItemData : ScriptableObject
{
    public  ItemType itemType;
    public string itemName;     // アイテムの名前
    public string description;  // アイテムの説明
    public Sprite icon;         // アイテムのアイコン
    public ItemEffect effect;   // アイテムの効果を定義するScriptableObject
    public bool isConsumable;   // 消費アイテムかどうか
}