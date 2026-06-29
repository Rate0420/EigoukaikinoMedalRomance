using UnityEngine;

/// <summary>
/// キャラクターごとの好感度を管理するシングルトン。
/// ChooseManager から AddAffinity() を呼んで変化させる。
/// SendChooseData の役割を引き継いだもの。
/// </summary>
public class S_AffinityManager : MonoBehaviour
{
    public static S_AffinityManager Instance { get; private set; }

    [SerializeField] private int affinity = 0;
    public int Affinity => affinity;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddAffinity(int delta)
    {
        affinity += delta;
        Debug.Log($"[AffinityManager] 好感度: {affinity} ({(delta >= 0 ? "+" : "")}{delta})");
    }

    public void ResetAffinity() => affinity = 0;
}
