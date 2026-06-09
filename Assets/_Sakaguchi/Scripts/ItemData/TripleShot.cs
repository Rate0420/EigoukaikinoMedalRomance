using UnityEngine;

[CreateAssetMenu(menuName = "ItemEffects/TripleShot")]
public class TripleShot : ItemEffect
{
    public override void OnInventoryChanged(BuffItemContext context)
    {
        context.test.shootCount = 3; // ƒvƒŒƒCƒ„[‚ÌshootCount‚ğ3‚É‚·‚é
    }
}