using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ReserveManager : MonoBehaviour
{
    [SerializeField] int maxReserve = 5;
    Queue<ReserveData> reserves = new Queue<ReserveData>();

    [SerializeField] SlotManager slotManager;
    [SerializeField] TextMeshProUGUI reserveCountText;

    bool isProcessing = false;

    private void Update()
    {
        // デバッグ用：スペースキーで保留追加
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AddReserve();
        }
    }

    // 外部から呼ぶ
    public void AddReserve()
    {
        if (reserves.Count >= maxReserve)
        {
            Debug.Log("保留MAX");
            return;
        }

        // 抽選をここでやる（重要）
        ReserveData data = slotManager.GenerateReserveData();

        reserves.Enqueue(data);
        Debug.Log($"保留追加 現在:{reserves.Count}");
        reserveCountText.text = $"保留: {reserves.Count}";

        // 動いてなければ回す
        if (!isProcessing)
        {
            StartCoroutine(ProcessReserve());
        }
    }

    System.Collections.IEnumerator ProcessReserve()
    {
        isProcessing = true;

        while (reserves.Count > 0)
        {
            ReserveData data = reserves.Dequeue();
            reserveCountText.text = $"保留: {reserves.Count}";

            yield return slotManager.PlaySlot(data);
        }

        isProcessing = false;
    }

}