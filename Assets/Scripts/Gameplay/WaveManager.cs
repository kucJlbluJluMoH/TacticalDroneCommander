using System.Linq;
using UnityEngine;
using TacticalDroneCommander.Core;
using TacticalDroneCommander.Core.Events;
using Entities;

namespace Gameplay
{
    public interface IWaveManager
    {
        void StartWave(int waveNumber = 0);
        void Initialize();
        int CurrentWave { get; }
    }
    
    public class WaveManager: IWaveManager
    {
        private readonly GameConfig _config;
        private readonly IEntitiesManager _entitiesManager;
        private readonly IEnemySpawner _enemySpawner;
        private readonly IEventBus _eventBus;
        private int _currentWave;
        private GameState _currentGameState;
        
        public int CurrentWave => _currentWave;
        
        public WaveManager(
            GameConfig config, 
            IEntitiesManager entitiesManager,
            IEnemySpawner enemySpawner,
            IEventBus eventBus)
        {
            _config = config;
            _entitiesManager = entitiesManager;
            _enemySpawner = enemySpawner;
            _eventBus = eventBus;
        }
        
        public void Initialize()
        {
            _entitiesManager.OnEntitiesChanged += CheckWaveCompletion;
            
            _eventBus.Subscribe<EntityDiedEvent>(OnEntityDied);
            _eventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
            
            Debug.Log("WaveManager initialized and subscribed to events.");
        }

        private void OnEntityDied(EntityDiedEvent evt)
        {
            if (evt.Entity is BaseEntity)
            {
                Debug.Log("WaveManager: Base died! Publishing GameOverEvent with defeat.");
                _eventBus.Publish(new GameOverEvent(false, _currentWave));
            }
        }

        private void OnGameStateChanged(GameStateChangedEvent evt)
        {
            _currentGameState = evt.NewState;
            
            if (evt.NewState == GameState.Wave)
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
            
            _eventBus.Publish(new WaveStartedEvent(_currentWave, enemyCount));
        }
        
        private void CheckWaveCompletion()
        {
            if (_currentGameState != GameState.Wave)
            {
                return;
            }
            var enemies = _entitiesManager.GetEntitiesOfType<EnemyEntity>();
            if (!enemies.Any())
            {
                OnWaveCompleted();
            }
        }

        private void OnWaveCompleted()
        {
            Debug.Log($"WaveManager: Wave {_currentWave} completed!");
            if (_currentWave >= _config.MaxWaves)
            {
                _eventBus.Publish(new GameOverEvent(true,_currentWave));
                return;
            }
            _eventBus.Publish(new WaveCompletedEvent(_currentWave));
        }
        
        private int CalculateEnemyCount()
        {
            return Mathf.RoundToInt(_config.BaseEnemiesPerWave * Mathf.Pow(_config.EnemiesCountMultiplier, _currentWave - 1));
        }
    }
}
