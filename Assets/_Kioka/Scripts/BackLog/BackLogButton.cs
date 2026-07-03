using UnityEngine;

public class BackLogButton : MonoBehaviour
{
    [SerializeField] private BackLogManager backLogManager;

    /// <summary>
    /// ƒ{ƒ^ƒ“‚ğ‰Ÿ‚µ‚½‚Æ‚«‚Ìˆ—
    /// </summary>
    /// <param name="btnNum"></param>
    public void OnClickButton(int btnNum)
    {
        backLogManager.OnBackLogBtn(btnNum);
    }
}
