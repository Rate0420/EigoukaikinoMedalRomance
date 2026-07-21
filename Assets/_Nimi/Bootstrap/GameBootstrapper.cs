using UnityEngine;
using EMR.Core;
using EMR.Round;

namespace EMR.Bootstrap
{
    /// <summary>
    /// 起動時にゲーム全体の依存関係を組み立てる。
    /// Bootstrap シーンにだけ配置する。
    /// </summary>
    public class GameBootstrapper : MonoBehaviour
    {
        [SerializeField]
        private RoundProgressionSettings _roundProgressionSettings;

        [SerializeField]
        private GameState _gameStatePrefab;

        private void Awake()
        {
            if (GameState.Instance != null)
            {
                return;
            }

            if (_roundProgressionSettings == null)
            {
                Debug.LogError(
                    "RoundProgressionSettings が未設定です。",
                    this);
                return;
            }

            if (_gameStatePrefab == null)
            {
                Debug.LogError(
                    "GameState Prefab が未設定です。",
                    this);
                return;
            }

            GameState gameState = Instantiate(_gameStatePrefab);

            gameState.Initialize(_roundProgressionSettings);
        }
    }
}