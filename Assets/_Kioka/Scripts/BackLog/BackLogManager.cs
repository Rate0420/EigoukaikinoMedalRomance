using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;

public class BackLogManager : MonoBehaviour
{
    [SerializeField] private ChooseManager chooseManager;   // ChooseManagerスクリプト

    // オブジェクト、ボタン
    [SerializeField] private GameObject backLog;    // 会話履歴オブジェクト
    [SerializeField] private GameObject backLogBtn; // 会話履歴表示ボタン
    [SerializeField] private GameObject closeBtn;   // 会話履歴閉じるボタン
    [SerializeField] private GameObject textBtn;    // テキストボタン(ストーリー中もBackLogBtnにNavigationできるように)
    [SerializeField] private Button twoBtn;         // 2個の選択肢ボタン
    [SerializeField] private Button threeBtn;       // 3個の選択肢ボタン

    public bool isBackLog;  // バックログが表示されているか判定
    public bool isClick;    // 閉じるボタンを押したか判定

    void Start()
    {
        backLog.SetActive(false);   // バックログ非表示
        EventSystem.current.SetSelectedGameObject(textBtn); // テキストボタンを最初に選択
    }

    /// <summary>
    /// ボタン押下時の処理
    /// </summary>
    /// <param name="btnNum"></param>
    public void OnBackLogBtn(int btnNum)
    {
        // バックログボタン
        if (btnNum == 0)
        {
            isBackLog = true;
            backLog.SetActive(true);    // バックログ表示
            EventSystem.current.SetSelectedGameObject(closeBtn);    // 閉じるボタンを最初に選択
        }
        // 閉じるボタン
        else if (btnNum == 1)
        {
            isBackLog = false;
            isClick = true;
            backLog.SetActive(false);   // バックログ非表示
            EventSystem.current.SetSelectedGameObject(textBtn);  // テキストボタンを最初に選択

            // 選択肢が出ているとき
            if (chooseManager.isEvent == true)
            {
                EventSystem.current.SetSelectedGameObject(backLogBtn);  // バックログボタンを最初に選択
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z)) OnBackLogBtn(0);

        if (Input.GetKeyDown(KeyCode.X))OnBackLogBtn(1);

        // Buttonコンポーネントを取得
        Selectable backLogSele = backLogBtn.GetComponent<Selectable>();
        // Navigation構造体を取得
        Navigation backLogNav = backLogSele.navigation;

        // バックログボタンからの移動先ボタンを設定
        if (chooseManager.isTwoBtn == true) backLogNav.selectOnLeft = twoBtn;
        else if (chooseManager.isTwoBtn == false) backLogNav.selectOnLeft = threeBtn;

        // 変更をコンポーネントに反映
        backLogSele.navigation = backLogNav;
    }
}
