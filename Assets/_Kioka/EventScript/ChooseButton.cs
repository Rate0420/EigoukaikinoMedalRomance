using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ChooseButton : MonoBehaviour
{
    [SerializeField] private ChooseManager chooseManager;  // ChooseManagerスクリプト
    [SerializeField] private TalkEvent talkEvent;          // TalkEventスクリプト

    [SerializeField] private GameObject chooseBtns;        // 選択肢のボタンまとめたもの

    /// <summary>
    /// 選択肢のボタンを押したときの処理
    /// </summary>
    // 選択肢1
    public void OnClickButton1()
    {
        chooseManager.OnChooseButton();
        
    }
    // 選択肢2
    public void OnClickButton2()
    {
        chooseManager.OnChooseButton();
    }
    // 選択肢3
    public void OnClickButton3()
    {
        chooseManager.OnChooseButton();
    }
}
