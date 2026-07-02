using UnityEngine;

public class Test_T : MonoBehaviour
{
    // 仮実装
    public PanelAniZoom panelAniZoom;
    public GameObject c_Button;

    // メニュー画面の切り替え用で使う
    public void C_Button()
    {
        panelAniZoom.MenuPanelChange();
    }
}
