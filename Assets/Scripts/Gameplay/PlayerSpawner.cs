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
    public interface IPlayerSpawner
    {
        PlayerEntity SpawnPlayer(Vector3 spawnPosition);
    }
    
    public class PlayerSpawner : IPlayerSpawner
    {
        private readonly GameConfig _config;
        private readonly IEntitiesManager _entitiesManager;
        private readonly IPoolService _poolService;
        private readonly IHealthBarService _healthBarService;
        
        private readonly ICombatSystem _combatSystem;
        private readonly IRegenerationSystem _regenerationSystem;
        private readonly ITargetingSystem _targetingSystem;
        private readonly IMovementSystem _movementSystem;
        private readonly IEventBus _eventBus;
        
        private int _droneCounter;
        
        public PlayerSpawner(
            GameConfig config, 
            IEntitiesManager entitiesManager,
            IPoolService poolService,
            IHealthBarService healthBarService,
            ICombatSystem combatSystem,
            IRegenerationSystem regenerationSystem,
            ITargetingSystem targetingSystem,
            IMovementSystem movementSystem,
            IEventBus eventBus)
        {
            _config = config;
            _entitiesManager = entitiesManager;
            _poolService = poolService;
            _healthBarService = healthBarService;
            _combatSystem = combatSystem;
            _regenerationSystem = regenerationSystem;
            _targetingSystem = targetingSystem;
            _movementSystem = movementSystem;
            _eventBus = eventBus;
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
            
            controller.Initialize(
                player,
                _poolService,
                _entitiesManager,
                _config,
                _combatSystem,
                _regenerationSystem,
                _targetingSystem,
                _movementSystem,
                _eventBus);
            
            _entitiesManager.RegisterEntity(player);
            
            _ = _healthBarService.CreateHealthBarForEntity(player);
            
            _eventBus.Publish(new EntitySpawnedEvent(player, spawnPosition));
            
            Debug.Log($"PlayerSpawner: Spawned {playerId} at {spawnPosition}");
            
            return player;
        }
    }
}

