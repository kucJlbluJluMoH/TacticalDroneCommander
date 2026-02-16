using System.Collections.Generic;
using UnityEngine;
using TacticalDroneCommander.Core;
using TacticalDroneCommander.Infrastructure;
using Entities;
using Controllers;

namespace Gameplay
{
    public interface IEnemySpawner
    {
        EnemyEntity SpawnEnemy(Vector3 spawnPosition, Vector3 targetPosition);
        List<EnemyEntity> SpawnEnemies(int count, Vector3 targetPosition);
    }
    
    public class EnemySpawner : IEnemySpawner
    {
        private readonly GameConfig _config;
        private readonly IEntitiesManager _entitiesManager;
        private readonly IPoolService _poolService;
        private readonly IAssetProvider _assetProvider;
        private int _enemyCounter;
        
        public EnemySpawner(
            GameConfig config, 
            IEntitiesManager entitiesManager,
            IPoolService poolService,
            IAssetProvider assetProvider)
        {
            _config = config;
            _entitiesManager = entitiesManager;
            _poolService = poolService;
            _assetProvider = assetProvider;
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
            
            controller.Initialize(enemy, _poolService, _entitiesManager);
            
            Entity baseEntity = _entitiesManager.GetEntity("base");
            if (baseEntity != null)
            {
                controller.SetTargetEntity(baseEntity);
            }
            
            _entitiesManager.RegisterEntity(enemy);
            
            Debug.Log($"EnemySpawner: Spawned {enemyId} at {spawnPosition}, target: {targetPosition}");
            
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
            //todo rework
            float spawnRadius = 10f;
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            
            Vector3 offset = new Vector3(
                Mathf.Cos(angle) * spawnRadius,
                0f,
                Mathf.Sin(angle) * spawnRadius
            );
            
            return centerPosition + offset;
        }
    }
}

