using UnityEngine;

[CreateAssetMenu(menuName = "ItemEffects/DualShot")]
public class DualShot : ItemEffect
{
    public override void OnInventoryChanged(ItemEffectContext context)
    {
        Debug.Log("2"); // プレイヤーの発射数を2にする
    }
}
