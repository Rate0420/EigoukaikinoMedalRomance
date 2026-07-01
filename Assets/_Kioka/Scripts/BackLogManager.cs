using UnityEngine;
using UnityEngine.EventSystems;

public class BackLogManager : MonoBehaviour
{
    [SerializeField] private ChooseManager chooseManager;   // ChooseManagerスクリプト

    [SerializeField] private GameObject backLog;
    [SerializeField] private GameObject backLogBtn;
    [SerializeField] private GameObject closeBtn;
    [SerializeField] private GameObject textBtn;

    public bool isBackLog;  // バックログが表示されているか判定
    public bool isClick;    // 閉じるボタンを押したか判定

    void Start()
    {
        backLog.SetActive(false);   // バックログ非表示
        EventSystem.current.SetSelectedGameObject(textBtn); // テキストボタンを最初に選択
        isBackLog = false;
        isClick = false;
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

    void Update()
    {
    }
}
