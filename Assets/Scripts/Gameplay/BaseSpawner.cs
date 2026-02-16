using UnityEngine;
using TacticalDroneCommander.Core;
using TacticalDroneCommander.Core.Events;
using TacticalDroneCommander.Infrastructure;
using TacticalDroneCommander.Systems;
using Entities;
using Controllers;
using Cysharp.Threading.Tasks;
using UI;

namespace Gameplay
{
    public interface IBaseSpawner
    {
        UniTask<BaseEntity> SpawnBase();
    }
    
    public class BaseSpawner : IBaseSpawner
    {
        private readonly GameConfig _config;
        private readonly IEntitiesManager _entitiesManager;
        private readonly IAssetProvider _assetProvider;
        private readonly IHealthBarService _healthBarService;
        private readonly IGameStateMachine _stateMachine;
        
        private readonly IRegenerationSystem _regenerationSystem;
        private readonly IEventBus _eventBus;
        
        public BaseSpawner(
            GameConfig config, 
            IEntitiesManager entitiesManager,
            IAssetProvider assetProvider,
            IHealthBarService healthBarService,
            IGameStateMachine stateMachine,
            IRegenerationSystem regenerationSystem,
            IEventBus eventBus)
        {
            _config = config;
            _entitiesManager = entitiesManager;
            _assetProvider = assetProvider;
            _healthBarService = healthBarService;
            _stateMachine = stateMachine;
            _regenerationSystem = regenerationSystem;
            _eventBus = eventBus;
        }
        
        public async UniTask<BaseEntity> SpawnBase()
        {
            GameObject basePrefab = await _assetProvider.Load<GameObject>("Prefabs/Base");
            
            if (basePrefab == null)
            {
                Debug.LogError("BaseSpawner: Failed to load base prefab!");
                return null;
            }
            
            GameObject baseObject = Object.Instantiate(basePrefab, _config.BaseCoordinates, Quaternion.identity);
            
            if (baseObject == null)
            {
                Debug.LogError("BaseSpawner: Failed to instantiate base!");
                return null;
            }
            
            BaseEntity baseEntity = new BaseEntity("base", baseObject, _config);
            BaseController controller = baseObject.GetComponent<BaseController>();
            
            controller.Initialize(
                baseEntity,
                _entitiesManager,
                _stateMachine,
                _config,
                _regenerationSystem,
                _eventBus);
            
            _entitiesManager.RegisterEntity(baseEntity);
            
            await _healthBarService.CreateHealthBarForEntity(baseEntity);
            
            _eventBus.Publish(new EntitySpawnedEvent(baseEntity, _config.BaseCoordinates));
            
            Debug.Log($"BaseSpawner: Base spawned at {_config.BaseCoordinates}");
            
            return baseEntity;
        }
    }
}

