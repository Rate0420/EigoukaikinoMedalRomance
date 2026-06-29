using UnityEngine;

public class ReelController : MonoBehaviour
{
    [SerializeField] RectTransform content;
    [SerializeField] float symbolHeight = 1280f;
    [SerializeField] float speed = 5f;
    [SerializeField] int symbolCount = 9;
    [SerializeField] float decelerationRate = 3f; // 減速の強さ

    [SerializeField]float stopDecelerationMultiplier = 1f;

    // パブリックプロパティを追加するだけ
    public bool IsSpinning => isSpinning;

    [SerializeField] float currentIndex = 0f;
    bool isSpinning = false;
    bool isStopping = false;
    int targetIndex = 0;
    float currentSpeed;

    void ApplyPosition()
    {
        float corrected = (symbolCount - (currentIndex % symbolCount)) % symbolCount;
        content.anchoredPosition = new Vector2(
            content.anchoredPosition.x,
            -corrected * symbolHeight
        );
    }

    public void StartSpin()
    {
        isSpinning = true;
        isStopping = false;
        currentSpeed = speed;
        // currentIndex = 0f; ← 削除（前の位置からそのまま継続）
    }

    public void StopSpin(int index, float decelMultiplier = 1f)
    {
        targetIndex = (symbolCount + 1) - index;
        isStopping = true;
        stopDecelerationMultiplier = decelMultiplier;
    }

    public void ForceStop(int index)
    {
        targetIndex = (symbolCount + 1) - index;
        currentIndex = targetIndex;
        isSpinning = false;
        isStopping = false;
        currentSpeed = 0f;
        ApplyPosition();
    }

    void Update()
    {
        if (!isSpinning)
        {
            ApplyPosition();
            return;
        }

        if (!isStopping)
        {
            // 逆方向（上から下）に回転：currentIndexを減らす
            currentIndex -= currentSpeed * Time.deltaTime;
            if (currentIndex < 0) currentIndex += symbolCount;
        }
        else
        {
            // 現在位置からtargetIndexまでの残り距離（逆方向＝マイナス方向）
            float remaining = currentIndex - targetIndex;

            // 必ず逆方向（マイナス）で止まるよう正規化
            if (remaining <= 0) remaining += symbolCount;

            // 残り距離に応じて減速
            float decelDistance = 2f * stopDecelerationMultiplier;
            if (remaining < decelDistance)
            {
                currentSpeed = Mathf.Lerp(0.5f, speed, remaining / decelDistance);
            }

            float step = currentSpeed * Time.deltaTime;

            if (remaining <= step)
            {
                currentIndex = targetIndex;
                isSpinning = false;
                isStopping = false;
                currentSpeed = 0f;
            }
            else
            {
                currentIndex -= step;
                if (currentIndex < 0) currentIndex += symbolCount;
            }
        }

        ApplyPosition();
    }
}
