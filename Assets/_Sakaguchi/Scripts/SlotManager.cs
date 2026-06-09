using UnityEngine;
using System.Collections;

public class SlotManager : MonoBehaviour
{
    [SerializeField] SlotTestRend slotTestRend; // 結果表示用のクラス（後できっちりしたやつに変える）
    [SerializeField] float winProbability = 0.1f; //当たりの確率
    [SerializeField] ReelManager reelManager; //リールの管理クラス
    [SerializeField] float slotEndDelay = 1.0f; //スロットが止まってから次の抽選までの遅延時間
    [SerializeField] EffectManager effectManager; // 演出管理クラス
    bool isWin; //当たりかどうか
    bool isReach;
    int selectedNumber = 1; // ルートキャラに対応した数字(後で別のクラスから参照するように変更)
    public bool testflg;

    /*
     
    ・Weak（ほぼ外れ）
    通常セリフ
    何もなし
    弱リーチ（たまに）

    ・Normal
    リーチ
    キャラクターリーチ（弱）
    設定キャラセリフ

    ・Strong
    設定キャラリーチ
    キャラ群（低頻度）

    ・VeryStrong
    高確キャラリーチ
    高確設定キャラリーチ
    キャラ群確定

    ・Jackpot
    フリーズ

     */

    // 演出の強さを表すランク
    public enum EffectRank
    {
        Weak,
        Normal,
        Strong,
        VeryStrong,
        Jackpot
    }

    EffectRank rank;

    public enum EffectType
    {
        None,
        NormalTalk,
        SetCharacterTalk,
        CharacterCutin,
        SetCharacterCutin,
        CharacterReach,
        SetCharacterReach,
        CharacterGroup,
        HighChanceReach,
        HighChanceSetCharacterReach,
        Freeze
    }

    EffectType effect;

    int[] GenerateReelResult(int resultNumber)
    {
        int[] reels = new int[3];

        isReach = false;

        if (resultNumber == -1)
        {
            // 外れでも一定確率でリーチ
            if (Random.value < 0.25f) // ←ここ調整ポイント
            {
                isReach = true;
            }
        }

        if (resultNumber != -1)
        {
            // 当たり
            reels[0] = resultNumber;
            reels[1] = resultNumber;
            reels[2] = resultNumber;
        }
        else
        {
            // ハズレ（ここで演出パターン作る）
            int pattern = Random.Range(0, 3);

            // バラバラ(リーチではない)
            reels[0] = Random.Range(1, 10);
            reels[1] = Random.Range(1, 10);
            reels[2] = Random.Range(1, 10);
            //　reels[2]がreels[0]と同じにならないようにする
            while (reels[2] == reels[0])
            {
                reels[2] = Random.Range(1, 10);
            }

            if (isReach)
            {
                int n = Random.Range(1, 10);
                reels[0] = n;
                reels[2] = n;

                reels[1] = Random.Range(1, 10);
                while (reels[1] == n)
                {
                    reels[1] = Random.Range(1, 10);
                }
            }
        }

        return reels;
    }

    void EffectGenerate()
    {
        if (!isWin)
        {
            float r = Random.value;

            if (r < 0.6f) rank = EffectRank.Weak;
            else if (r < 0.85f) rank = EffectRank.Normal;
            else if (r < 0.97f) rank = EffectRank.Strong;
            else rank = EffectRank.VeryStrong; // ←ハズレでも来る
        }
        else
        {
            float r = Random.value;

            if(r < 0.05f) rank = EffectRank.Weak;
            else if (r < 0.5f) rank = EffectRank.Normal;
            else if (r < 0.8f) rank = EffectRank.Strong;
            else if (r < 0.95f) rank = EffectRank.VeryStrong;
            else rank = EffectRank.Jackpot;
        }
    }

    EffectType SelectEffect(EffectRank rank)
    {
        float r = Random.value;

        switch (rank)
        {
            case EffectRank.Weak:
                if (r < 0.4f) return EffectType.None;
                if (r < 0.7f) return EffectType.NormalTalk;
                return EffectType.SetCharacterTalk;

            case EffectRank.Normal:
                if (r < 0.4f) return EffectType.CharacterCutin;
                if (r < 0.7f) return EffectType.SetCharacterCutin;
                return EffectType.CharacterReach;

            case EffectRank.Strong:
                if (r < 0.5f) return EffectType.SetCharacterReach;
                if (r < 0.8f) return EffectType.CharacterGroup;
                return EffectType.HighChanceReach;

            case EffectRank.VeryStrong:
                if (r < 0.5f) return EffectType.HighChanceReach;
                return EffectType.HighChanceSetCharacterReach;

            case EffectRank.Jackpot:
                return EffectType.Freeze;
        }

        return EffectType.None;
    }

    float GetEffectWinRate(EffectType effect)
    {
        switch (effect)
        {
            case EffectType.None: return 0.05f;
            case EffectType.NormalTalk: return 0.10f;
            case EffectType.SetCharacterTalk: return 0.15f;

            case EffectType.CharacterCutin: return 0.30f;
            case EffectType.SetCharacterCutin: return 0.40f;
            case EffectType.CharacterReach: return 0.35f;

            case EffectType.SetCharacterReach: return 0.60f;
            case EffectType.CharacterGroup: return 0.70f;

            case EffectType.HighChanceReach: return 0.85f;
            case EffectType.HighChanceSetCharacterReach: return 0.90f;

            case EffectType.Freeze: return 1.0f;
        }

        return 0f;
    }

    int GetRandomOtherNumber()
    {
        // 選択した数字以外の数字をランダムに返す（例えば1の場合は2,3,4,5,6,8,9の中から）
        int[] otherNumbers = new int[] { 1, 2, 3, 4, 5, 6, 8, 9 };
        // selectedNumberを除外
        otherNumbers = System.Array.FindAll(otherNumbers, n => n != selectedNumber);
        int index = Random.Range(0, otherNumbers.Length);
        return otherNumbers[index];
    }

    public ReserveData GenerateReserveData()
    {
        ReserveData data = new ReserveData();

        int resultNumber;
        float r = Random.value;

        if (r < winProbability)
        {
            r = Random.value;

            if (r < 0.40f) resultNumber = selectedNumber;
            else if (r < 0.96f) resultNumber = GetRandomOtherNumber();
            else resultNumber = 7;
        }
        else
        {
            resultNumber = -1;
        }

        data.resultNumber = resultNumber;

        // 演出
        isWin = resultNumber != -1;
        EffectGenerate();
        data.rank = rank;
        data.effect = SelectEffect(rank);

        return data;
    }

    public IEnumerator PlaySlot(ReserveData data)
    {
        // テストフラグが立っている場合は、常にキャラクターグループ演出にする
        if (testflg)
        {
            data.effect = EffectType.CharacterGroup;
            data.rank = EffectRank.Strong;
            int[] reel = GenerateReelResult(3); // 3を当たりとして生成
            reelManager.StartReels();
            Coroutine efc = StartCoroutine(effectManager.PlayEffect(data.effect));
            // efcが終わるまで待つ
            yield return efc;
            Debug.Log($"[SlotManager] PlaySlot: 演出開始 {data.effect}");
            reelManager.StartStopReels(reel);

            yield return new WaitUntil(() =>
    !reelManager.leftReel.IsSpinning &&
    !reelManager.centerReel.IsSpinning &&
    !reelManager.rightReel.IsSpinning
);

            yield return new WaitForSeconds(slotEndDelay);
        }

        else
        {
            Coroutine effectCoroutine = null;
            int[] reels = GenerateReelResult(data.resultNumber);

            reelManager.StartReels();
            Debug.Log($"[SlotManager] PlaySlot: 演出開始 {data.effect}");
            if (data.effect != EffectType.None)
            {
                effectCoroutine = StartCoroutine(effectManager.PlayEffect(data.effect));
            }

            yield return effectCoroutine;

            if (data.effect != EffectType.None)
            {
                yield return effectCoroutine;
            }

            reelManager.StartStopReels(reels);

            // リール終了待ち（ここ大事）
            yield return new WaitUntil(() =>
                !reelManager.leftReel.IsSpinning &&
                !reelManager.centerReel.IsSpinning &&
                !reelManager.rightReel.IsSpinning
            );

            yield return new WaitForSeconds(slotEndDelay);
        }
    }
}

