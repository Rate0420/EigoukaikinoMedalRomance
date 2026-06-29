using UnityEngine;
using System.Collections;
using static ReserveManager;
using Unity.VisualScripting;

public class EffectManager : MonoBehaviour
{
    public SequencePlayer cutinPlayer;
    public TalkManager talkManager;
    [SerializeField] string[] cutinPath;
    public int selectedNumber = 0; // ルートキャラに対応した数字(後で別のクラスから参照するように変更)
    [SerializeField]string selectedCharacter;
    public float reachtime;
    [SerializeField] VideoEffectPlayer videoEffectPlayer;
    public VideoEffectPlayer preEffectPlayer;

    enum selectedCharacterEnum
    {
        陽向,
        加流,
        小夜,
        澪音,
        リーゼロッテ,
        リリス,
        冥衣,
        ヴェルミリオン
    }

    public IEnumerator PlayEffect(SlotManager.EffectType effect)
    {
        Debug.Log($"[演出] 効果: {effect}");
        switch (effect)
        {
            case SlotManager.EffectType.NormalTalk:
                yield return StartCoroutine(PlayTalk("キャラセリフ", GetRandomOtherNumber()));
                break;
            case SlotManager.EffectType.SetCharacterTalk:
                yield return StartCoroutine(PlayTalk("設定キャラセリフ",selectedNumber));
                break;
            case SlotManager.EffectType.CharacterCutin:
                yield return StartCoroutine(PlayCutin("キャラカットイン",GetRandomOtherNumber()));
                break;
            case SlotManager.EffectType.SetCharacterCutin:
                yield return StartCoroutine(PlayCutin("設定キャラカットイン",selectedNumber));
                break;
            case SlotManager.EffectType.CharacterGroup:
                yield return StartCoroutine(PlayCharacteroGroup());
                break;
            case SlotManager.EffectType.Freeze:
                yield return StartCoroutine(PlayFreez());
                break;
            default:
                yield return null;
                break;
        }
    }

    public IEnumerator PlayFreez()
    {
        Debug.Log($"[演出] フリーズ");
        yield return StartCoroutine(videoEffectPlayer.PlayVideoCoroutine(16, 0.5f));
    }

    public IEnumerator PlayCharacteroGroup()
    {
        Debug.Log($"[演出] キャラ群予告");
        // 再生は裏で走らせる（最後まで流れる）
        StartCoroutine(videoEffectPlayer.PlayVideoNoFadeCoroutine(17, 1f));
        yield return StartCoroutine(videoEffectPlayer.WaitEarlyEndCoroutine(3f));
    }

    // ReelManagerからyield returnできるように変更
    public IEnumerator PlayReach()
    {
        StartCoroutine(videoEffectPlayer.PlayVideoNoFadeCoroutine(18, 1f));
        yield return StartCoroutine(videoEffectPlayer.WaitEarlyEndCoroutine(1f));
    }

    IEnumerator PlayTalk(string text,int num)
    {
        Debug.Log($"[演出] 会話: {text}");

        // selectedNumberをenumに変換してキャラ名を取得
        selectedCharacterEnum characterEnum = (selectedCharacterEnum)num;
        selectedCharacter = characterEnum.ToString();
        int talknum = talkManager.searchTalkBoxNum(selectedCharacter);
        Debug.Log("キャラクター" + selectedCharacter);
        yield return new WaitForSeconds(talkManager.SetTalkBox(talknum));
    }

    IEnumerator PlayCutin(string name,int num)
    {
        Debug.Log($"[演出] カットイン: {name}");
        yield return new WaitForSeconds(cutinPlayer.Play(cutinPath[num]));
        cutinPlayer.Stop();
    }

    int GetRandomOtherNumber()
    {
        while (true)
        {
            int num = Random.Range(0, cutinPath.Length);
            if (num != selectedNumber) return num;
        }
    }

    public void PlayPreEffect()
    {
        int r = Random.Range(0, preEffectPlayer.videoPaths.Length);
        switch(r)
        {
            case 0:
                Debug.Log($"[演出] 先読み: 白ほうき星");
                StartCoroutine(preEffectPlayer.PlayVideoNoFadeCoroutine(r, 1f));
                break;
            case 1:
                Debug.Log($"[演出] 先読み: 雪結晶");
                StartCoroutine(preEffectPlayer.PlayVideoFadeInOut(r,1f,1,1));
                break;
            case 2:
                Debug.Log($"[演出] 先読み: レンズフレア");
                StartCoroutine(preEffectPlayer.PlayVideoNoFadeCoroutine(r, 1f));
                break;
            case 3:
                Debug.Log($"[演出] 先読み: 光の風的な奴");
                StartCoroutine(preEffectPlayer.PlayVideoNoFadeCoroutine(r, 1f));
                break;
        }

        
    }

}