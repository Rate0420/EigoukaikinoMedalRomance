using UnityEngine;

public class BuffItemContext : MonoBehaviour
{
    // バフアイテムの基礎クラス。
    // ここに必用な情報等を定義する。

    public TestScripts_Sakaguchi test;

    // 消費アイテム処理クラス
    public VerticalShaking_process shakingProcess;
    public MedalExplosion_process medalExplosionProcess;


    public void Gold()
    { 
        Debug.Log("Gold");
    }
    public void MedalExplosion()
    {
        Debug.Log("MedalExplosion");
    }

    public void StartVerticalShaking(float duration)
    {
        shakingProcess.StartShake(duration);
    }

    public void StartMedalExplosion(GameObject medal)
    {
        Debug.Log("StartMedalExplosion");
        medalExplosionProcess.BlowAway(medal);
    }
}