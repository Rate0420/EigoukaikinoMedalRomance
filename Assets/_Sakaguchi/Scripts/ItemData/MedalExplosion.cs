using UnityEngine;

[CreateAssetMenu(menuName = "ItemEffects/MedalExprosion")]
public class MedalExplosion : ItemEffect
{
    public float explosionForce = 1000f; // ”š”­‚ÌˆĞ—Í
    public float explosionRadius = 5f;   // ”š”­‚Ì”¼Œa
    public float upwardsModifier = 2f;   // ã•ûŒü‚Ö‚Ì•â³’l
    public float explosionChance = 0.05f; // ”š”­‚ÌŠm—¦

    public override void OnMedalLanded(BuffItemContext context, GameObject medal)
    {
        // 5%‚ÌŠm—¦‚Å”š”­‚·‚éˆ—‚ğ‚±‚±‚ÉÀ‘•
        float chance = Random.Range(0f, 1f);
        Debug.Log(chance);
        if (chance <= explosionChance)
        {
            context.StartMedalExplosion(medal);
        }
    }
}
