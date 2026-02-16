using UnityEngine;
using TacticalDroneCommander.Core;
using TacticalDroneCommander.Infrastructure;
using Entities;

namespace Gameplay
{
    public interface IUpgradeSpawner
    {
        void TrySpawnUpgrade(Vector3 position);
    }
    
    public class UpgradeSpawner : IUpgradeSpawner
    {
        private readonly GameConfig _config;
        private readonly IPoolService _poolService;
        
        public UpgradeSpawner(GameConfig config, IPoolService poolService)
        {
            _config = config;
            _poolService = poolService;
        }
        
        public void TrySpawnUpgrade(Vector3 position)
        {
            if (Random.value > _config.UpgradeDropChance)
                return;
            
            position.y += 0.5f;
            
            GameObject upgradeObject = _poolService.Get("Upgrade", position, Quaternion.identity);
            
            if (upgradeObject == null)
            {
                Debug.LogWarning("UpgradeSpawner: Failed to get upgrade from pool!");
                return;
            }
            
            var upgradeConfigs = _config.UpgradeValues;
            var randomConfig = upgradeConfigs[Random.Range(0, upgradeConfigs.Length)];
            
            UpgradeType upgradeType = System.Enum.Parse<UpgradeType>(randomConfig.Type);
            
            UpgradePickup pickup = upgradeObject.GetComponent<UpgradePickup>();
            if (pickup == null)
            {
                pickup = upgradeObject.AddComponent<UpgradePickup>();
            }
            
            pickup.Initialize(upgradeType, randomConfig.Value, _poolService, _config);
            
            Debug.Log($"UpgradeSpawner: Spawned {upgradeType} upgrade at {position}");
        }
    }
}

