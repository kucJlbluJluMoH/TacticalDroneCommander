using System.Threading.Tasks;
using UnityEngine;
using Entities;
using TacticalDroneCommander.Infrastructure;

namespace UI
{
    public interface IHealthBarService
    {
        Task CreateHealthBarForEntity(Entity entity);
    }
    
    public class HealthBarService : IHealthBarService
    {
        private readonly IAssetProvider _assetProvider;
        private readonly Transform _healthBarsContainer;
        
        public HealthBarService(IAssetProvider assetProvider)
        {
            _assetProvider = assetProvider;
            
            GameObject container = new GameObject("HealthBarsContainer");
            _healthBarsContainer = container.transform;
        }
        
        public async Task CreateHealthBarForEntity(Entity entity)
        {
            if (entity == null)
                return;
            
            GameObject healthBarPrefab = await _assetProvider.Load<GameObject>("Prefabs/HealthBar");
            
            if (healthBarPrefab == null)
            {
                Debug.LogWarning("HealthBarService: HealthBar prefab not found! Creating simple health bar...");
                return;
            }
            
            GameObject healthBarObject = Object.Instantiate(healthBarPrefab, _healthBarsContainer);
            HealthBarUI healthBar = healthBarObject.GetComponent<HealthBarUI>();
            
            if (healthBar != null)
            {
                healthBar.Initialize(entity);
            }
        }
    }
}

