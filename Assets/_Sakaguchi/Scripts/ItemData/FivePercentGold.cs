using UnityEngine;

[CreateAssetMenu(menuName = "ItemEffects/FivePercentGold")]
public class FivePercentGold : ItemEffect
{
    public override void OnMedalShot(BuffItemContext context)
    {
        // 5%‚ÌŠm—¦‚ÅƒS[ƒ‹ƒh‚ğŠl“¾‚·‚éˆ—‚ğ‚±‚±‚ÉÀ‘•
        float chance = Random.Range(0f, 1f);
        Debug.Log(chance);
        if (chance <= 0.05f)
        {

            context.Gold();
        }
    }
}