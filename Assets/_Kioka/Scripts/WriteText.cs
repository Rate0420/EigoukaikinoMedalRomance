using System.Collections;
using TMPro;
using UnityEngine;

public class WriteText : MonoBehaviour
{
    [SerializeField] private StoryData storyData;           // スクリプタブルオブジェクト
    [SerializeField] private SetStoryUI setStoryUI;         // SetStoryUIスクリプト
    [SerializeField] private ChooseManager chooseManager;   // ChooseManagerスクリプト

    [SerializeField] private TextMeshProUGUI massageText;   // メッセージテキスト
    [SerializeField] private GameObject image;              // Aボタンの画像
    [SerializeField] private GameObject fastUI;             // 早送りの画像、テキスト
    [SerializeField] private string[] scene;                // シーン名
    [SerializeField] private Animator anim;                 // アニメーター

    [SerializeField] private float textSpeed = 0.1f;        // テキストの速さ

    bool isSceneChange;                                     // シーン遷移の判別
    public int index = 0;

    private bool isFast;        // 早送りかどうかの判別
    private bool isDrawing;     // メッセージを書いているどうかの判別(連打防止)

    private void Start()
    {
        storyData = DontDestroyStory.instance.story;
        anim = fastUI.GetComponent<Animator>();     // 早送りの画像、テキストからアニメーターを取得
        scene = storyData.sceneName;
        setStoryUI.ChangeNameAndSprite();
        DrawText();
    }

    private void Update()
    {
        // 選択肢イベントが発生していないときのみ文字送り
        if(chooseManager.isEvent == false)
        {
            //
            // ここのFire1,Fire2を変える とりあえず||使ってキーマウに対応させる
            //
            // テキストを表示　左クリックorSpaceキー
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            {
                DrawText();
                image.SetActive(false);     // Aボタンの画像を非表示にする
            }
            // 早送り右クリックor左シフトキー
            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.LeftShift))
            {
                if (!isFast)
                {
                    isFast = true;      // 早送り
                    textSpeed = 0.04f;
                    anim.SetBool("ScaleBool", true);    // アニメーション再生
                }
                else
                {
                    isFast = false;     // 元の速さに戻す
                    textSpeed = 0.1f;
                    anim.SetBool("ScaleBool", false);   // アニメーション停止
                }
            }
        }
    }

    /// <summary>
    /// メッセージを表示する
    /// </summary>
    public void DrawText()
    {
        if (isDrawing) 
        {
            // 文字を出している最中に連打された場合は、文字をすべて表示する
            StopAllCoroutines();    // コルーチンを停止
            massageText.text = storyData.text[index];   // すべての文字を表示
            index++;

            image.SetActive(true);
            isDrawing = false;
        }

        else if (index < storyData.text.Length)
        {
            StopAllCoroutines();        // 連打対策
            massageText.text = "";      // 初期化
            StartCoroutine(CorDrawText(storyData.text[index]));
        }
        // ストーリー終了時
        else if (index >= storyData.text.Length)
        {
            anim.SetBool("ScaleBool", false);   // アニメーション停止

            //
            // ここのFire1を変える
            //
            // 右クリックorスペースキーでシーン遷移
            if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) && !isSceneChange)
            {
                FadeSceneChanger.ChangeScene(scene[0]);
                isSceneChange = true;
            }
        }
    }

    /// <summary>
    /// 1文字ずつ表示するためのコルーチン
    /// </summary>
    private IEnumerator CorDrawText(string massage)
    {
        if (isDrawing) yield break;
        isDrawing = true;
        setStoryUI.ChangeNameAndSprite();
        float time = 0f;
        while (true)
        {
            yield return null;
            time += Time.deltaTime;
            int length = Mathf.FloorToInt(time / textSpeed);
            if (length > massage.Length) break;
            massageText.text = massage.Substring(0, length);
        }

        massageText.text = massage;
        yield return null;

        index++; 

        image.SetActive(true);
        isDrawing = false;
    }
}
