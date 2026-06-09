using UnityEngine;

public class SendChooseData : MonoBehaviour
{
    /// <summary>
    /// 選択情報を渡す
    /// </summary>
    public void SendData(string route, string choose)
    {
        Debug.Log($"選択データ: {route}  キャラ好感度： {choose}");
    }
}
