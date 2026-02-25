using System.Collections.Generic;
using UnityEngine;
using TacticalDroneCommander.Core;
using TacticalDroneCommander.Core.Events;
using TacticalDroneCommander.Infrastructure;
using TacticalDroneCommander.Systems;
using Entities;
using Controllers;
using UI;

namespace Gameplay
{
    public interface IEnemySpawner
    {
        EnemyEntity SpawnEnemy(Vector3 spawnPosition, Vector3 targetPosition);
        List<EnemyEntity> SpawnEnemies(int count, Vector3 targetPosition);
        void DespawnAllEnemies();
    }
    
    public class EnemySpawner : IEnemySpawner
    {
        private readonly GameConfig _config;
        private readonly IEntitiesManager _entitiesManager;
        private readonly IPoolService _poolService;
        private readonly IAssetProvider _assetProvider;
        private readonly IUpgradeSpawner _upgradeSpawner;
        private readonly IHealthBarService _healthBarService;
        
        private readonly ICombatSystem _combatSystem;
        private readonly IMovementSystem _movementSystem;
        private readonly ITargetingSystem _targetingSystem;
        private readonly IEventBus _eventBus;
        
        private int _enemyCounter;
        
        public EnemySpawner(
            GameConfig config, 
            IEntitiesManager entitiesManager,
            IPoolService poolService,
            IAssetProvider assetProvider,
            IUpgradeSpawner upgradeSpawner,
            IHealthBarService healthBarService,
            ICombatSystem combatSystem,
            IMovementSystem movementSystem,
            ITargetingSystem targetingSystem,
            IEventBus eventBus)
        {
            _config = config;
            _entitiesManager = entitiesManager;
            _poolService = poolService;
            _assetProvider = assetProvider;
            _upgradeSpawner = upgradeSpawner;
            _healthBarService = healthBarService;
            _combatSystem = combatSystem;
            _movementSystem = movementSystem;
            _targetingSystem = targetingSystem;
            _eventBus = eventBus;
            _enemyCounter = 0;
        }
        
        public EnemyEntity SpawnEnemy(Vector3 spawnPosition, Vector3 targetPosition)
        {
            GameObject enemyObject = _poolService.Get("Enemy", spawnPosition, Quaternion.identity);
            
            if (enemyObject == null)
            {
                Debug.LogError("EnemySpawner: Failed to get enemy from pool!");
                return null;
            }
            
            string enemyId = $"enemy_{_enemyCounter++}";
            EnemyEntity enemy = new EnemyEntity(enemyId, enemyObject, _config);
            
            EnemyController controller = enemyObject.GetComponent<EnemyController>();
            
            controller.Initialize(
                enemy,
                _poolService,
                _entitiesManager,
                _config,
                _combatSystem,
                _movementSystem,
                _targetingSystem,
                _eventBus,
                _upgradeSpawner);
            
            _entitiesManager.RegisterEntity(enemy);
            
            _ = _healthBarService.CreateHealthBarForEntity(enemy);
            
            _eventBus.Publish(new EntitySpawnedEvent(enemy, spawnPosition));
            
            Debug.Log($"EnemySpawner: Spawned {enemyId} at {spawnPosition}");
            
            return enemy;
        }
        
        public List<EnemyEntity> SpawnEnemies(int count, Vector3 targetPosition)
        {
            List<EnemyEntity> spawnedEnemies = new List<EnemyEntity>();
            
            for (int i = 0; i < count; i++)
            {
                Vector3 spawnPosition = GenerateSpawnPosition(targetPosition);
                
                EnemyEntity enemy = SpawnEnemy(spawnPosition, targetPosition);
                if (enemy != null)
                {
                    spawnedEnemies.Add(enemy);
                }
            }
            
            Debug.Log($"EnemySpawner: Spawned {spawnedEnemies.Count} enemies");
            return spawnedEnemies;
        }
        
        private Vector3 GenerateSpawnPosition(Vector3 centerPosition)
        {
            float spawnRadius = 15f;//todo
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            
            Vector3 offset = new Vector3(
                Mathf.Cos(angle) * spawnRadius,
                0f,
                Mathf.Sin(angle) * spawnRadius
            );
            
            return centerPosition + offset;
        }

        public void DespawnAllEnemies()
        {
            var enemies = new List<EnemyEntity>(_entitiesManager.GetEntitiesOfType<EnemyEntity>());
            foreach (var enemy in enemies)
            {
                _healthBarService.RemoveHealthBar(enemy);
                var controller = enemy.GetGameObject()?.GetComponent<EnemyController>();
                controller?.Despawn();
                _entitiesManager.UnregisterEntity(enemy);
            }

            Debug.Log($"EnemySpawner: Despawned {enemies.Count} enemies.");
        }
    }
}

