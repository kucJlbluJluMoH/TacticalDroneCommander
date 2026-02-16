using UnityEngine;
using TacticalDroneCommander.Core;
using TacticalDroneCommander.Infrastructure;
using Entities;
using Controllers;
using UI;

namespace Gameplay
{
    public interface IPlayerSpawner
    {
        PlayerEntity SpawnPlayer(Vector3 spawnPosition);
    }
    
    public class PlayerSpawner : IPlayerSpawner
    {
        private readonly GameConfig _config;
        private readonly IEntitiesManager _entitiesManager;
        private readonly IPoolService _poolService;
        private readonly ITargetFinder _targetFinder;
        private readonly IAssetProvider _assetProvider;
        private readonly IHealthBarService _healthBarService;
        private int _droneCounter = 0;
        
        public PlayerSpawner(
            GameConfig config, 
            IEntitiesManager entitiesManager,
            IPoolService poolService,
            ITargetFinder targetFinder,
            IAssetProvider assetProvider,
            IHealthBarService healthBarService)
        {
            _config = config;
            _entitiesManager = entitiesManager;
            _poolService = poolService;
            _targetFinder = targetFinder;
            _assetProvider = assetProvider;
            _healthBarService = healthBarService;
        }
        
        public PlayerEntity SpawnPlayer(Vector3 spawnPosition)
        {
            spawnPosition.y = _config.PlayerHoverHeight;
            
            GameObject playerObject = _poolService.Get("Drone", spawnPosition, Quaternion.identity);
            
            if (playerObject == null)
            {
                Debug.LogError("PlayerSpawner: Failed to get player drone from pool!");
                return null;
            }
            
            string playerId = $"player_{_droneCounter++}";
            PlayerEntity player = new PlayerEntity(playerId, playerObject, _config);
            
            PlayerDroneController controller = playerObject.GetComponent<PlayerDroneController>();
            if (controller == null)
            {
                controller = playerObject.AddComponent<PlayerDroneController>();
            }
            
            controller.Initialize(player, _targetFinder, _poolService, _entitiesManager, _config);
            
            _entitiesManager.RegisterEntity(player);
            
            _ = _healthBarService.CreateHealthBarForEntity(player);
            
            Debug.Log($"PlayerSpawner: Spawned {playerId} at {spawnPosition}");
            
            return player;
        }
    }
}

