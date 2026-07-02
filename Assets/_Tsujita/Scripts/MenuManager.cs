using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;
    public CharactorType currentRoute;

    [SerializeField] CharacterDatabase database;
    [SerializeField] CharacterImage characterImage;
    [SerializeField] StatusGet statusGet;

    [SerializeField] private GameObject[] menuButtons;  // ボタン
    [SerializeField] private GameObject[] menuPanels;   // パネル

    [SerializeField] private Image buckButton;  // 戻るボタン
    [SerializeField] private GameObject sceneChangeImage;
    [SerializeField] private GameObject sceneCharaImage;

    private Color DefaultColor =        // ボタンの初期色
        new Color32(255, 255, 255, 255);
    private Color SelectColor =         // ボタン選択中色
        new Color32(255, 45, 235, 255);

    public int nowPanleNo = -1;     // 現在開いているパネル番号

    public bool isPanelFlg;
    public bool isMenuFlg;  // メニューパネル表示フラグ
    public bool isLBFlg;    // 追加パネル表示用フラグ

    private PanelAni[] panelAnis;     // パネルアニメーション用
    private PanelAniZoom zoomPanelAni;  // 拡大縮小用アニメーション
    [SerializeField] private GameObject buckButton2;

    private void Awake()
    {
        Instance = this;
        panelAnis = new PanelAni[menuPanels.Length];

        for (int i = 0; i < menuPanels.Length; i++)
            panelAnis[i] =
                menuPanels[i].GetComponent<PanelAni>();

        zoomPanelAni = sceneChangeImage.GetComponent<PanelAniZoom>();
        sceneChangeImage.SetActive(false);
        isPanelFlg = false;
    }

    private void Start()
    {
        MenuStart();
    }

    /// <summary>
    /// メニューパネルを開くと実行
    /// </summary>
    public void MenuStart()
    {
        buckButton2.SetActive(false);
        isMenuFlg = false;
        isLBFlg = false;
        characterImage.Route();
        statusGet.SetStatus();
        characterImage.MainImageChange();
    }

    /// <summary>
    /// 初期ボタンのパネル切り替え
    /// </summary>
    public void OnMeunButtons(int buttonNo)
    {   
        switch (buttonNo)
        {
            case 0: // メニューパネル非表示
                PanelReset(buttonNo);
                break;
            case 1: // ステータスパネル
                PanelSet(buttonNo);
                break;
            case 2: // キャラ解説パネル
                PanelSet(buttonNo);
                break;
            case 3: // セーブパネル
                PanelSet(buttonNo);
                break;
            case 4: // 音量設定パネル
                PanelSet(buttonNo);
                break;
            case 5: // 好感度一覧
                PanelSet(buttonNo);
                break;
            case 99:
                PanelSet(buttonNo);
                break;
        }

        // 色変更
        if(buttonNo > 0)
            BuckButtonChange();
    }

    /// <summary>
    /// パネルの非表示
    /// </summary>
    private void PanelReset(int buttonNo)
    {
        if (isLBFlg)    // 好感度パネル
        {
            panelAnis[5].Close();
            isLBFlg = false;
        }
        else if (isMenuFlg) // パネル全般の切り替え
        {
            panelAnis[nowPanleNo].Close();
            nowPanleNo = -1;

            buckButton.color = DefaultColor;
            buckButton2.SetActive(false);
            isMenuFlg = false;
        }
        else    // メニューパネルの非表示
        {
            sceneChangeImage.SetActive(true);
            zoomPanelAni.MenuPanelChange();
            isPanelFlg = false;
        }
    }

    /// <summary>
    /// 初期ボタンのパネル関連
    /// </summary>
    /// <param name="buttonNo"></param>
    private void PanelSet(int buttonNo)
    {
        if (buttonNo == 99)
        {
            isPanelFlg = true;
            buttonNo = 6;
        }

        if (!isMenuFlg)
        {
            nowPanleNo = buttonNo;
            panelAnis[buttonNo].Open();
            isMenuFlg = true;
        }

        if (buttonNo == 5)
        {
            panelAnis[buttonNo].Open();
            isLBFlg = true;
        }
    }

    // 戻るボタンの色変更
    private void BuckButtonChange()
    {
        buckButton.color = SelectColor;
        buckButton2.SetActive(true);
    }
}
