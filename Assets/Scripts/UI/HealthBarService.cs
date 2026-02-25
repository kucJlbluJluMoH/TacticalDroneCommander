using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Entities;
using TacticalDroneCommander.Core.Events;
using TacticalDroneCommander.Infrastructure;

namespace UI
{
    public interface IHealthBarService
    {
        Task CreateHealthBarForEntity(Entity entity);
        void RemoveHealthBar(Entity entity);
    }
    
    public class HealthBarService : IHealthBarService
    {
        private readonly IAssetProvider _assetProvider;
        private readonly Transform _healthBarsContainer;
        private readonly Dictionary<string, HealthBarUI> _healthBars = new();

        public HealthBarService(IAssetProvider assetProvider, IEventBus eventBus)
        {
            _assetProvider = assetProvider;
            eventBus.Subscribe<EntityDiedEvent>(OnEntityDied);

            GameObject container = new GameObject("HealthBarsContainer");
            _healthBarsContainer = container.transform;
        }

        private void OnEntityDied(EntityDiedEvent evt)
        {
            RemoveHealthBar(evt.Entity);
        }

        public void RemoveHealthBar(Entity entity)
        {
            if (entity == null) return;

            if (_healthBars.TryGetValue(entity.GetId(), out HealthBarUI bar))
            {
                _healthBars.Remove(entity.GetId());
                if (bar != null)
                    Object.Destroy(bar.gameObject);
            }
        }
        
        public async Task CreateHealthBarForEntity(Entity entity)
        {
            if (entity == null)
                return;
            
            GameObject healthBarPrefab = await _assetProvider.Load<GameObject>("Prefabs/HealthBar");
            
            if (healthBarPrefab == null)
            {
                Debug.LogWarning("HealthBarService: HealthBar prefab not found!");
                return;
            }
            
            GameObject healthBarObject = Object.Instantiate(healthBarPrefab, _healthBarsContainer);
            HealthBarUI healthBar = healthBarObject.GetComponent<HealthBarUI>();
            
            if (healthBar != null)
            {
                healthBar.Initialize(entity);
                _healthBars[entity.GetId()] = healthBar;
            }
        }
    }
}
