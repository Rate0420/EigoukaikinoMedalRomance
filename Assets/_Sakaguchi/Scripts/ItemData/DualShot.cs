using UnityEngine;

[CreateAssetMenu(menuName = "ItemEffects/DualShot")]
public class DualShot : ItemEffect
{
    public override void OnInventoryChanged(BuffItemContext context)
    {
        context.test.shootCount = 2; // ƒvƒŒƒCƒ„[‚ÌshootCount‚ğ1‘‚â‚·
    }
}