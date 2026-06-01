using UnityEngine;

public class SlotManager : MonoBehaviour
{
    [SerializeField] SlotTestRend slotTestRend; // 結果表示用のクラス（後できっちりしたやつに変える）
    [SerializeField] float winProbability = 0.05f; //当たりの確率
    [SerializeField] ReelManager reelManager; //リールの管理クラス
    bool isWin; //当たりかどうか
    int selectedNumber = 1; // ルートキャラに対応した数字(後で別のクラスから参照するように変更)


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

    private void Update()
    {
        // スペースキーでスロット開始（テスト用）
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SlotStart();
        }
        // Nキーで100回スロット開始（テスト用）
        if (Input.GetKeyDown(KeyCode.N))
        {
            for (int i = 0; i < 100; i++)
            {
                SlotStart();
            }
        }

        // リール制御テストで7.2,7の結果でリールを回して止める
        if (Input.GetKeyDown(KeyCode.T))
        {
            int[] testReels = new int[] { 7, 2, 7 };
            slotTestRend.SlotRend(testReels, EffectRank.Jackpot, EffectType.Freeze);
            reelManager.StartReels();
            reelManager.StartStopReels(testReels);
        }
    }

    public void SlotStart()
    {
        int resultNumber;
        float r = Random.value;

        if (r < winProbability)
        {
            isWin = true;

            r = Random.value;
            if (r < 0.40f) resultNumber = selectedNumber;
            else if (r < 0.96f) resultNumber = GetRandomOtherNumber();
            else resultNumber = 7;
        }
        else
        {
            isWin = false;
            resultNumber = -1;
        }

        EffectGenerate();

       effect = SelectEffect(rank);

        float expectedRate = GetEffectWinRate(effect);

        // 当たりの場合でフリーズ演出の際は7で確定させる
        if (isWin && effect == EffectType.Freeze)
        {
            resultNumber = 7;
        }

        int[] reels = GenerateReelResult(resultNumber);

        //Debug.Log($"【結果】当たり: {isWin}");
        //Debug.Log($"【演出ランク】{rank}");
        //Debug.Log($"【演出】{effect}");
        //Debug.Log($"【信頼度】{expectedRate * 100}%");
        //Debug.Log($"【リール】{reels[0]}, {reels[1]}, {reels[2]}");

        //1行で出力
        Debug.Log($"【結果】当たり: {isWin} | 【演出ランク】{rank} | 【演出】{effect} | 【信頼度】{expectedRate * 100}% | 【リール】{reels[0]}, {reels[1]}, {reels[2]}");
        slotTestRend.SlotRend(reels, rank, effect);
        reelManager.StartReels();
        reelManager.StartStopReels(reels);
    }

    int[] GenerateReelResult(int resultNumber)
    {
        int[] reels = new int[3];

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

            // weakreach以上の演出だった場合二つ揃える。
            if (effect >= EffectType.SetCharacterCutin)
            {
                // リーチ風（2つ揃い）
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
            // ハズレのときはweak,normal,strong,verystrongのどれか（weakが多め）
            //rank = Random.value < 0.7f ? EffectRank.Weak : EffectRank.Normal;
            rank = Random.value < 0.7f ? EffectRank.Weak : (Random.value < 0.85f ? EffectRank.Normal : (Random.value < 0.95f ? EffectRank.Strong : EffectRank.VeryStrong));
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
                if (r < 0.7f) return EffectType.None;
                if (r < 0.85f) return EffectType.NormalTalk;
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
}
