using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using EMR.Core;

public class SceneChanger : MonoBehaviour
{
    private GamePause gamePause;
    [SerializeField] ReserveManager reserveManager;
    [SerializeField] GameObject MedalRoot;
    [SerializeField] PanelAniZoom panelAniZoom;
    [SerializeField] GameObject menuCanvas;
    [SerializeField] GameObject BlackOutImage;

    [SerializeField] WinManager winManager;

    private void Start()
    {
        gamePause = GameState.Instance.GamePause;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            StartScenario("Sakaguchi_TestStoryScene");
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            EndScenario("Sakaguchi_TestStoryScene");
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            StartCoroutine(StartMenu());
        }
    }

    public void StartScenario(string sceneName)
    {
        StartCoroutine(StartScenarioCoroutine(sceneName));
    }

    [SerializeField] SlotManager slotManager;
    // [SerializeField] WinManager winManager; ← 不要になるので削除可

    IEnumerator StartMenu()
    {
        // 切れ目で止まるよう要求
        reserveManager.pauseRequested = true;

        // 切れ目に入るまで待つ
        yield return new WaitUntil(() =>
            !reserveManager.isProcessing ||
            reserveManager.isBetweenReserves
        );

        // 切れ目に入ったのを確認してからポーズ
        gamePause.ChangePause(true);

        menuCanvas.SetActive(true);
        BlackOutImage.SetActive(true);
        panelAniZoom.MenuPanelChange();

        yield return new WaitForSeconds(1);
        MedalRoot.SetActive(false);
    }

    public void StartShop()
    {
        StartCoroutine(StartShopCoroutine());
    }

    public IEnumerator StartShopCoroutine()
    {
        // 切れ目で止まるよう要求
        reserveManager.pauseRequested = true;

        // 切れ目に入るまで待つ
        yield return new WaitUntil(() =>
            !reserveManager.isProcessing ||
            reserveManager.isBetweenReserves
        );

        // 切れ目に入ったのを確認してからポーズ
        gamePause.ChangePause(true);

        MedalRoot.SetActive(false);
    }

    IEnumerator StartScenarioCoroutine(string sceneName)
    {
        reserveManager.pauseRequested = true;

        yield return new WaitUntil(() =>
            !reserveManager.isProcessing ||
            reserveManager.isBetweenReserves
        );

        gamePause.ChangePause(true);

        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        MedalRoot.SetActive(false);
    }

    // メニュー・シナリオ終了時にpauseRequestedを解除する
    public void EndMenu()
    {
        reserveManager.pauseRequested = false;
        MedalRoot.SetActive(true);
        gamePause.ChangePause(false);
    }

    IEnumerator EndScenarioCoroutine(string sceneName)
    {
        yield return SceneManager.UnloadSceneAsync(sceneName);
        reserveManager.pauseRequested = false;
        MedalRoot.SetActive(true);
        gamePause.ChangePause(false);
    }
    public void EndScenario(string sceneName)
    {
        StartCoroutine(EndScenarioCoroutine(sceneName));
    }

}