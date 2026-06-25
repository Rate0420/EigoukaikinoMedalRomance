using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    public bool IsScenarioPlaying { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.U))
        { 
            StartScenario();
        }
        if(Input.GetKeyUp(KeyCode.I)) { EndScenario(); }

        if (Input.GetKeyDown(KeyCode.S))
        {
            SceneManager.LoadScene(
                "Sakaguchi_TestStoryScene",
                LoadSceneMode.Additive);

            Debug.Log("シナリオ開始");
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            SceneManager.UnloadSceneAsync(
                "Sakaguchi_TestStoryScene");

            Debug.Log("シナリオ終了");
        }
    }

    public void StartScenario()
    {
        foreach (var p in FindObjectsOfType<MonoBehaviour>()
            .OfType<IGamePausable>())
        {
            p.Pause();
        }
    }

    public void EndScenario()
    {
        foreach (var p in FindObjectsOfType<MonoBehaviour>()
            .OfType<IGamePausable>())
        {
            p.Resume();
        }
    }

}