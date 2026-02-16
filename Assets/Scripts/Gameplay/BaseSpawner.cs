using UnityEngine;
using TacticalDroneCommander.Core;
using TacticalDroneCommander.Infrastructure;
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
        
        public BaseSpawner(
            GameConfig config, 
            IEntitiesManager entitiesManager,
            IAssetProvider assetProvider,
            IHealthBarService healthBarService)
        {
            _config = config;
            _entitiesManager = entitiesManager;
            _assetProvider = assetProvider;
            _healthBarService = healthBarService;
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
            controller.Initialize(baseEntity, _entitiesManager, _config);
            
            _entitiesManager.RegisterEntity(baseEntity);
            
            await _healthBarService.CreateHealthBarForEntity(baseEntity);
            
            Debug.Log($"BaseSpawner: Base spawned at {_config.BaseCoordinates}");
            
            return baseEntity;
        }
    }
}

