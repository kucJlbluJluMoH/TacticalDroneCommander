using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using TacticalDroneCommander.Infrastructure;

namespace TacticalDroneCommander.Core
{
    public class GameBootstrapper : MonoBehaviour
    {
        [Inject] private IAssetProvider _assetProvider;
        [Inject] private ISaveLoadService _saveLoadService;
        [Inject] private IGameStateMachine _gameStateMachine;
        
        private async void Start()
        {
            await InitializeGame();
        }

        private async UniTask InitializeGame()
        {
            Debug.Log("GameBootstrapper: Starting game initialization...");
            
            _saveLoadService.Load();
            Debug.Log("GameBootstrapper: Save data loaded");
            
            await UniTask.Yield();
            Debug.Log("GameBootstrapper: Asset provider ready");
            
            _gameStateMachine.SwitchState(GameState.Pregame);
            
            Debug.Log("GameBootstrapper: Game initialization complete!");
        }

        private void OnApplicationQuit()
        {
            _saveLoadService.Save();
            Debug.Log("GameBootstrapper: Game saved on exit");
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                _saveLoadService.Save();
                Debug.Log("GameBootstrapper: Game saved on pause");
            }
        }
    }
}
