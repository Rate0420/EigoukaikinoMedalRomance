using UnityEngine;

[CreateAssetMenu(menuName = "ItemEffects/MedalExprosion")]
public class MedalExplosion : ItemEffect
{
    public override void OnMedalLanded(BuffItemContext context)
    {
        // 5%‚ÌŠm—¦‚Å”š”­‚·‚éˆ—‚ğ‚±‚±‚ÉÀ‘•
        float chance = Random.Range(0f, 1f);
        Debug.Log(chance);
        if (chance <= 0.05f)
        {
            context.MedalExplosion();
        }
    }
}
