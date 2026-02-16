using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using TacticalDroneCommander.Infrastructure;
using Gameplay;

namespace TacticalDroneCommander.Core
{
    public class GameBootstrapper : MonoBehaviour
    {
        [Inject] private ISaveLoadService _saveLoadService;
        [Inject] private IGameStateMachine _gameStateMachine;
        [Inject] private IWaveManager _waveManager;
        [Inject] private IPoolInitializer _poolInitializer;
        [Inject] private IBaseSpawner _baseSpawner;
        [Inject] private IPlayerDroneManager _playerDroneManager;
        
        private async void Start()
        {
            await InitializeGame();
        }

        private async UniTask InitializeGame()
        {
            Debug.Log("GameBootstrapper: Starting game initialization...");
            
            _waveManager.Initialize();
            _playerDroneManager.Initialize();
            
            _saveLoadService.Load();
            Debug.Log("GameBootstrapper: Save data loaded");
            
            await UniTask.Yield();
            Debug.Log("GameBootstrapper: Asset provider ready");

            await _poolInitializer.InitializeAllPools();
            
            await _baseSpawner.SpawnBase();
            Debug.Log("GameBootstrapper: Base spawned");
            
            _playerDroneManager.SpawnInitialDrone();
            Debug.Log("GameBootstrapper: Initial player drone spawned");
            
            _gameStateMachine.SwitchState(GameState.Pregame);
            
            Debug.Log("GameBootstrapper: Game initialization complete!");
        }

        private void Update()
        {
            _gameStateMachine?.Update();
        }

        private void OnApplicationQuit()
        {
            _gameStateMachine?.Dispose();
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
