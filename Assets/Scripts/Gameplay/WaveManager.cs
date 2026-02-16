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
        
        private int _currentWave;
        
        public WaveManager(
            GameConfig config, 
            IEntitiesManager entitiesManager,
            IEnemySpawner enemySpawner)
        {
            _config = config;
            _entitiesManager = entitiesManager;
            _enemySpawner = enemySpawner;
        }
        
        public void Initialize()
        {
            Debug.Log("WaveManager initialized.");
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
        
        private int CalculateEnemyCount()
        {
            return Mathf.RoundToInt(_config.BaseEnemiesPerWave * Mathf.Pow(_config.EnemiesCountMultiplier, _currentWave - 1));
        }
    }
}
