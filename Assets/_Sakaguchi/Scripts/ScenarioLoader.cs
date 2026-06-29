using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ScenarioLoader : MonoBehaviour
{
    [SerializeField] GamePause gamePause;
    [SerializeField] ReserveManager reserveManager;
    [SerializeField] GameObject MedalRoot;

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
    }

    public void StartScenario(string sceneName)
    {
        StartCoroutine(StartScenarioCoroutine(sceneName));
    }

    IEnumerator StartScenarioCoroutine(string sceneName)
    {
        // ① ポーズ要求
        gamePause.ChangePause(true);

        // ② 保留消化終了待ち
        yield return new WaitUntil(() => !reserveManager.isProcessing);

        // ③ シーン追加
        yield return SceneManager.LoadSceneAsync(
            sceneName,
            LoadSceneMode.Additive);
        
        MedalRoot.SetActive(false);
    }

    public void EndScenario(string sceneName)
    {
        StartCoroutine(EndScenarioCoroutine(sceneName));
    }

    IEnumerator EndScenarioCoroutine(string sceneName)
    {
        yield return SceneManager.UnloadSceneAsync(sceneName);
        MedalRoot.SetActive(true);
        gamePause.ChangePause(false);
    }
}