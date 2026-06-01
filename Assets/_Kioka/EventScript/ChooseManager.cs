using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ChooseManager : MonoBehaviour
{
    [SerializeField] private WriteText writeText;    // WriteTextスクリプト
    [SerializeField] private GameObject chooseBtns;  // 選択肢のボタンまとめたもの
    [SerializeField] private Button button;          // ボタン

    [SerializeField] private int[] number;           // 選択肢イベント実行時のインデックス番号
    HashSet<int> usedIndexes = new HashSet<int>();   // 使われたindexを入れる

    [SerializeField] private TalkEvent talkEvent;    // スクリプタブルオブジェクト

    public bool isEvent;                             // 選択肢イベントが発生しているか判別

    void Start()
    {
        number = talkEvent.number;
        chooseBtns.SetActive(false);
        button.Select();
    }

    /// <summary>
    /// 選択肢イベントを実行するか判断する
    /// </summary>
    void CheckChoose()
    {
        // indexとnumberの番号が同じで、まだ処理していない場合だけ表示
        // Containsで同じ数字が含まれているか判別
        if (number.Contains(writeText.index) && !usedIndexes.Contains(writeText.index))
        {
            chooseBtns.SetActive(true);  // 選択肢ボタン表示
            isEvent = true;
        }
    }

    /// < summary >
    /// 選択肢ボタン押下時の処理
    /// </ summary >
    public void OnChooseButton()
    {
        chooseBtns.SetActive(false);  // 選択肢ボタン非表示
        isEvent = false;
        button.Select();              // ボタン選択

        usedIndexes.Add(writeText.index);  // このindexは処理済みにする
    }

    void Update()
    {
        CheckChoose();
    }
}
