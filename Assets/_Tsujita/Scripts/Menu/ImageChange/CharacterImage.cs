using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.iOS;
using UnityEngine.UI;

public class CharacterImage : MonoBehaviour
{
    // メイン画像の切り替え全般
    [SerializeField] private CharacterDatabase database;
    [SerializeField] protected MenuManager menuManager;

    [SerializeField] private Image targetImage;

    [SerializeField] private AudioSource voiceAudio;

    [SerializeField] private TextMeshProUGUI targetTMP;

    private int oldIndex;
    private CharacterData character;

    private void Awake()
    {
        voiceAudio = GetComponent<AudioSource>();
    }

    public void Route()
    {
        // 現在ルートのキャラ取得
        character =
            database.GetCharacter(
                MenuManager.Instance.currentRoute
            );
    }

    /// <summary>
    /// メイン画像の切り替え
    /// </summary>
    public void MainImageChange()
    {
        if(!menuManager.isMenuFlg)
        {
            if (character == null)
            {
                Debug.LogError("キャラがいないよ");
                return;
            }

            if (character.sprites.Length == 0)
            {
                Debug.LogError("画像がないよ");
                return;
            }


            // ランダム抽選
            int randomIndex;
            do
            {
                randomIndex =
                    Random.Range(0, character.sprites.Length);

            } while (randomIndex == oldIndex);


            // 今回番号保存
            oldIndex = randomIndex;


            // 画像反映
            targetImage.sprite =
                character.sprites[randomIndex];

            // ボイス反映
            if (character.voice.Length > randomIndex)
            {
                voiceAudio.PlayOneShot(
                    character.voice[randomIndex]
                );
            }

            // テキスト反映
            if (character.menuTexts.Length > randomIndex)
            {
                targetTMP.text =
                    character.menuTexts[randomIndex];
            }
        }
    }
}