using UnityEngine;
using System.Collections;

public class PanelAniZoom : MonoBehaviour
{
    // ƒƒjƒ…[‚Ìƒpƒlƒ‹Ø‚è‘Ö‚¦ƒAƒjƒ[ƒVƒ‡ƒ“

    [SerializeField] private GameObject menuPanel;  // ƒƒjƒ…[ƒpƒlƒ‹

    [SerializeField] private RectTransform panel;   // ˆÃ“]‚P
    [SerializeField] private RectTransform image;   // ˆÃ“]‚Q(ƒLƒƒƒ‰)

    // ˆÃ“]—p
    [SerializeField] private Vector2 zoomInPos = new Vector2(0 , 0);
    [SerializeField] private Vector2 zoomOutPos = Vector2.zero;
    // ˆÃ“]—p(ƒLƒƒƒ‰)
    [SerializeField] private Vector2 _zoomInPos = new Vector2(0, 0);
    [SerializeField] private Vector2 _zoomOutPos = Vector2.zero;

    [SerializeField] private float duration;    // ˆÃ“]‚P‚ÌˆÃ“]ŠÔ
    [SerializeField] private float _duration;   // ˆÃ“]‚Q‚ÌˆÃ“]ŠÔ

    [SerializeField] GameObject[] bOImage;      // ˆÃ“]—p‰æ‘œ

    [SerializeField] private float wSF; // ƒV[ƒ“Ø‚è‘Ö‚¦‚ÌŠÔŠu

    private RectTransform targetPanel;  // Œ»İˆÃ“]‚³‚¹‚Ä‚¢‚é‚à‚Ì
    private float targerDuration;       // Œ»İİ’è‚³‚ê‚Ä‚¢‚éˆÃ“]ŠÔ

    /// <summary>
    /// ƒƒjƒ…[ƒpƒlƒ‹‚Ì•\¦Ø‘Ö
    /// </summary>
    public void MenuPanelChange()
    {
        bOImage[0].SetActive(true);
        bOImage[1].SetActive(true);
        targetPanel = panel;
        targerDuration = duration;
        StartCoroutine(CloseAnimation());
    }

    /// <summary>
    /// ˆÃ“]¨ˆÃ“]‰ğœ
    /// </summary>
    private IEnumerator CloseAnimation()
    {
        yield return ScaleAnimation(zoomInPos, zoomOutPos);

        // ˆÃ“]ŠJn
        targetPanel = image;
        targerDuration = _duration;
        yield return ScaleAnimation(_zoomOutPos, _zoomInPos);
        menuPanel.SetActive(!menuPanel.activeSelf);
        
        yield return new WaitForSeconds(wSF);
        // ˆÃ“]‰ğœ
        yield return ScaleAnimation(_zoomInPos, _zoomOutPos);
        targetPanel = panel;
        targerDuration = duration;
        yield return ScaleAnimation(zoomOutPos, zoomInPos);
        bOImage[0].SetActive(false);
        bOImage[1].SetActive(false);
    }

    private IEnumerator ScaleAnimation(Vector3 start, Vector3 end)
    {
        float time = 0;

        while (time < targerDuration)
        {
            time += Time.deltaTime;
            float t = time / targerDuration;

            targetPanel.localScale = Vector3.Lerp(start, end, t);

            yield return null;
        }

        targetPanel.localScale = end;
    }
}
