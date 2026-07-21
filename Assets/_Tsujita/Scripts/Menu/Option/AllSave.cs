using UnityEngine;
using System.Collections;

public class AllSave : MonoBehaviour
{
    [SerializeField] private MenuManager menuManager;
    [SerializeField] private GameObject saveText;

    [SerializeField] private float fdx;

    public void Seve()
    {
        // ñ¢é¿ëï
        // Ç±Ç±Ç≈ÉZÅ[Éu
        StartCoroutine(SaveButton());
    }

    private IEnumerator SaveButton()
    {
        saveText.SetActive(true);
        yield return new WaitForSeconds(fdx);
        saveText.SetActive(false);
        menuManager.OnMeunButtons(0);
        yield return null;
    }
}
