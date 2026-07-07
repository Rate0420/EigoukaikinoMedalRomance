using UnityEngine;
using TMPro;

public class MedalManager : MonoBehaviour
{
    public static MedalManager Instance;

    public int medals = 200;

    public TMP_Text medalText;

    void Awake()
    {
        Instance = this;
    }

    public void AddMedals(int amount)
    {
        medals += amount;

        UpdateUI();

        RefreshAllShopButtons();
    }

    public bool SpendMedals(int amount)
    {
        if (medals < amount)
            return false;

        medals -= amount;

        UpdateUI();

        RefreshAllShopButtons();

        return true;
    }

    void UpdateUI()
    {
        medalText.text = "ŠŽƒƒ_ƒ‹ : " + medals;
    }

    void RefreshAllShopButtons()
    {
        foreach (BuffShopManager shop in FindObjectsByType<BuffShopManager>(FindObjectsSortMode.None))
        {
            shop.RefreshUI();
        }

        foreach (CostShopMananager shop in FindObjectsByType<CostShopMananager>(FindObjectsSortMode.None))
        {
            shop.RefreshUI();
        }
    }
}