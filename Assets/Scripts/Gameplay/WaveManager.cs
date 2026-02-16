using System.Linq;
using UnityEngine;
using TacticalDroneCommander.Core;
using Entities;

namespace Gameplay
{
    public interface IWaveManager
    {
        void StartWave(int waveNumber = 0);
        void Initialize();
    }
    
    public class WaveManager: IWaveManager
    {
        private readonly GameConfig _config;
        private readonly IEntitiesManager _entitiesManager;
        private readonly IEnemySpawner _enemySpawner;
        private readonly IGameStateMachine _stateMachine;
        private int _currentWave;
        
        public WaveManager(
            GameConfig config, 
            IEntitiesManager entitiesManager,
            IEnemySpawner enemySpawner,IGameStateMachine stateMachine)
        {
            _config = config;
            _entitiesManager = entitiesManager;
            _enemySpawner = enemySpawner;
            _stateMachine = stateMachine;
        }
        
        public void Initialize()
        {
            _entitiesManager.OnEntitiesChanged += CheckWaveCompletion;
            _stateMachine.OnStateChanged += OnGameStateChanged;
            Debug.Log("WaveManager initialized and subscribed to state changes.");
        }

        private void OnGameStateChanged(GameState newState)
        {
            if (newState == GameState.Wave)
            {
                StartWave();
            }
        }
        
        public void StartWave(int waveNumber = 0)
        {
            if(waveNumber!=0)
                _currentWave = waveNumber-1;
            _currentWave++;
            int enemyCount = CalculateEnemyCount();
            
            Entity baseEntity = _entitiesManager.GetEntity("base");
            Vector3 targetPosition = baseEntity != null 
                ? baseEntity.GetTransform().position 
                : _config.BaseCoordinates;
            
            Debug.Log($"WaveManager: Starting wave {_currentWave} with {enemyCount} enemies");
            
            _enemySpawner.SpawnEnemies(enemyCount, targetPosition);
        }
        private void CheckWaveCompletion()
        {
            if (_stateMachine.CurrentState != GameState.Wave)
            {
                return;
            }
            var enemies = _entitiesManager.GetEntitiesOfType<EnemyEntity>();
            if (!enemies.Any())
            {
                OnWaveCompleted();
            }
        }

        private async void OnWaveCompleted()
        {
            Debug.Log($"WaveManager: Wave {_currentWave} completed!");
            _stateMachine.SwitchState(GameState.Postwave);

            await System.Threading.Tasks.Task.Delay((int)(_config.TimeBetweenWaves*1000));

            if (_stateMachine.CurrentState == GameState.Postwave)
            {
                StartWave();
            }
        }
        
        private int CalculateEnemyCount()
        {
            return Mathf.RoundToInt(_config.BaseEnemiesPerWave * Mathf.Pow(_config.EnemiesCountMultiplier, _currentWave - 1));
        }
    }
}
