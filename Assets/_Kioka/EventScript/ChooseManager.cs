using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChooseManager : MonoBehaviour
{
    [SerializeField] private TalkEvent talkEvent;           // TalkEventスクリプタブルオブジェクト
    [SerializeField] private WriteText writeText;           // WriteTextスクリプト
    [SerializeField] private SendChooseData sendChooseData; // SendChooseDataスクリプト

    [SerializeField] private CanvasGroup twoChooseBtns;     // 2個の選択肢のボタンまとめたもの
    [SerializeField] private CanvasGroup threeChooseBtns;   // 3個の選択肢のボタンまとめたもの
    [SerializeField] private GameObject twoFirstBtn;        // 最初に選択状態にするボタン
    [SerializeField] private GameObject threeFirstBtn;      // 最初に選択状態にするボタン

    private int[] number;                                   // 選択肢イベント実行時のストーリーインデックス番号
    private int eventIndex;                                 // 何番目の選択肢イベントか
    private HashSet<int> usedIndexes = new HashSet<int>();  // 使われたindexを入れる

    public bool isEvent = false;                            // 選択肢イベントが発生しているか判別

    void Start()
    {
        number = talkEvent.number;

        // 選択肢ボタン非表示
        twoChooseBtns.alpha = 0;     
        threeChooseBtns.alpha = 0;   
    }

    /// <summary>
    /// 選択肢イベントを実行するか判断する
    /// </summary>
    void CheckChoose()
    {
        // この処理を1回だけ行う
        if (isEvent) return;

        // indexとnumberの番号が同じで、まだ処理していない場合だけ表示
        // Containsで同じ数字が含まれているか判別
        if (number.Contains(writeText.index) && !usedIndexes.Contains(writeText.index))
        {
            isEvent = true;                                                 // 選択肢イベント実行
            int count = talkEvent.events[eventIndex].choices.Length;        // 選択肢が何個あるか数える

            // 選択肢2個
            if (count == 2)
            {
                twoChooseBtns.alpha = 1;                                    // 2個の選択肢ボタン表示
                EventSystem.current.SetSelectedGameObject(twoFirstBtn);     // 選択状態のボタンを設定
            }
            // 選択肢3個
            else if (count == 3)
            {
                threeChooseBtns.alpha = 1;                                  // 3個の選択肢ボタン表示
                EventSystem.current.SetSelectedGameObject(threeFirstBtn);   // 選択状態のボタンを設定
            }
        }
    }

    /// < summary >
    /// 選択肢ボタン押下時の処理
    /// </ summary >
    public void OnChooseButton(int chooseIndex)
    {
        isEvent = false;    // 選択肢イベント終了

        // 選択肢ボタン非表示
        twoChooseBtns.alpha = 0;                        
        threeChooseBtns.alpha = 0;                      

        // スクリプタブルオブジェクトから選択結果を取得
        var choose = talkEvent.events[eventIndex].choices[chooseIndex];
        sendChooseData.SendData(choose.routeString, choose.chooseString);   // 選択データを入れる

        usedIndexes.Add(writeText.index);   // このindexは処理済みにする

        eventIndex++;
    }

    void Update()
    {
        CheckChoose();
    }
}
