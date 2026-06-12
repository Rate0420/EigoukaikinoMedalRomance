using UnityEngine;
using System.Collections;
using static ReserveManager;

public class SlotManager : MonoBehaviour
{
    [SerializeField] float winProbability = 0.5f; //当たりの確率
    [SerializeField] ReelManager reelManager; //リールの管理クラス
    [SerializeField] float slotEndDelay = 1.0f; //スロットが止まってから次の抽選までの遅延時間
    [SerializeField] EffectManager effectManager; // 演出管理クラス
    [SerializeField] ReserveManager reserveManager; // 保留管理クラス
    bool isWin; //当たりかどうか
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

    int[] GenerateReelResult(int resultNumber,bool reach)
    {
        int[] reels = new int[3];

        if (resultNumber == -1)
        {
            // 外れでも1/4、もしくはキャラクターリーチ以上の演出ならリーチにする
            if (Random.value < 0.25f || effect >= EffectType.CharacterReach)
            {
                reach = true;
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

            if (reach)
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
            resultNumber = GenerateResultNumber(); // 当たり
            data.isReach = true; // ★確定でリーチ
        }
        else
        {
            resultNumber = -1;

            // ★ここでリーチを決める（先に！）
            data.isReach = Random.value < 0.25f;
        }

        data.resultNumber = resultNumber;

        // ↓ここで演出
        isWin = resultNumber != -1;
        EffectGenerate();
        data.rank = rank;
        data.effect = SelectEffect(rank);

        // ★強演出なら強制リーチ
        if (data.effect >= EffectType.CharacterReach)
        {
            data.isReach = true;
        }

        data.visual = DecideReserveVisual(data);

        return data;
    }
    ReserveVisualType DecideReserveVisual(ReserveData data)
    {
        // 面倒だがswitchでランク別にどの保留色になるかを決める
        switch(data.rank)
        {
            case EffectRank.Weak:
                // weakは7割ノーマル、3割青。
                return Random.value < 0.7f ? ReserveVisualType.Normal : ReserveVisualType.Blue;
            case EffectRank.Normal:
                // 2割ノーマル、6割青、2割緑
                return Random.value < 0.2f ? ReserveVisualType.Normal : (Random.value < 0.75f ? ReserveVisualType.Blue : ReserveVisualType.Green);
            case EffectRank.Strong:
                //　5割緑、5割赤
                return Random.value < 0.5f ? ReserveVisualType.Green : ReserveVisualType.Red;
            case EffectRank.VeryStrong:
                return ReserveVisualType.Red;
            case EffectRank.Jackpot:
                return ReserveVisualType.Gold;
            default:
                return ReserveVisualType.Normal;
        }
    }

    bool DecidePreEffect(ReserveData data)
    {
        if (data.rank >= EffectRank.Strong)
        {
            return Random.value < 0.7f;
        }

        if (data.rank == EffectRank.Normal)
        {
            return Random.value < 0.3f;
        }

        return false;
    }

    public IEnumerator PlaySlot(ReserveData data)
    {
        // テストフラグが立っている場合は、常に会話演出にする
        if (testflg)
        {
            Coroutine preEffectCoroutine = null;
            if (reserveManager.HasPreTarget())
            {
                effectManager.PlayPreEffect();
            }

            data.effect = EffectType.NormalTalk;
            data.rank = EffectRank.Weak;
            Debug.Log("ランク:"+data.rank +"演出:"+data.effect); 
            int[] reel = GenerateReelResult(data.resultNumber,data.isReach); // 3を当たりとして生成
            reelManager.StartReels();
            Coroutine efc = StartCoroutine(effectManager.PlayEffect(data.effect));
            // efcが終わるまで待つ
            yield return efc;
            reelManager.StartStopReels(reel);

            yield return new WaitUntil(() =>
    !reelManager.leftReel.IsSpinning &&
    !reelManager.centerReel.IsSpinning &&
    !reelManager.rightReel.IsSpinning
);

            yield return new WaitForSeconds(slotEndDelay);

            if (preEffectCoroutine != null)
            {
                StopCoroutine(preEffectCoroutine);
            }
        }

        else
        {
            Coroutine preEffectCoroutine = null;

            // 先読み対象があるか確認
            if ( reserveManager.HasPreTarget())
            {
                Debug.Log("先読み演出");
                effectManager.PlayPreEffect();
            }

            Coroutine effectCoroutine = null;
            int[] reels = GenerateReelResult(data.resultNumber,data.isReach);
            if(data.effect == EffectType.Freeze) reels = GenerateReelResult(7, true); // フリーズなら必ず7にする
            Debug.Log("ランク:" + data.rank + "演出:" + data.effect);
            reelManager.StartReels();
            if (data.effect != EffectType.None)
            {
                effectCoroutine = StartCoroutine(effectManager.PlayEffect(data.effect));
            }

            if (effectCoroutine != null)
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

            if (preEffectCoroutine != null)
            {
                StopCoroutine(preEffectCoroutine);
            }
        }
    }

    int GenerateResultNumber()
    {
        int resultnum;
        float r = Random.value;
        //ルートキャラの数字 = 40% 
        //非ルートキャラ = 8 %×7人 = 56 
        //7(JPCC) = 4 %
        if (r < 0.4f) resultnum = selectedNumber;
        else if (r < 0.96f) resultnum = GetRandomOtherNumber();
        else resultnum = 7;

        return resultnum;
    }
}

