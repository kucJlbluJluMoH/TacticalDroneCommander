using System.Collections.Generic;
using Entities;
using TacticalDroneCommander.Core.Events;
using UnityEngine;

namespace TacticalDroneCommander.Systems
{
    public interface IRegenerationSystem
    {
        void ProcessRegeneration(Entity entity, float regenAmount, float regenDelay, float regenRate);
    }
    
    public class RegenerationSystem : IRegenerationSystem
    {
        private readonly Dictionary<string, float> _lastRegenTimes = new();
        private readonly Dictionary<string, float> _lastDamageTimes = new();

        public RegenerationSystem(IEventBus eventBus)
        {
            eventBus.Subscribe<EntityDamagedEvent>(OnEntityDamaged);
        }

        private void OnEntityDamaged(EntityDamagedEvent evt)
        {
            _lastDamageTimes[evt.Victim.GetId()] = Time.time;
        }

        public void ProcessRegeneration(Entity entity, float regenAmount, float regenDelay, float regenRate)
        {
            if (entity == null || entity.IsDead())
                return;
            
            if (entity.GetHealth() >= entity.GetMaxHealth())
                return;

            _lastDamageTimes.TryGetValue(entity.GetId(), out float lastDamageTime);
            if (Time.time - lastDamageTime < regenDelay)
                return;

            _lastRegenTimes.TryGetValue(entity.GetId(), out float lastRegenTime);
            if (Time.time - lastRegenTime >= regenRate)
            {
                entity.Regenerate((int)regenAmount);
                _lastRegenTimes[entity.GetId()] = Time.time;
            }
        }
    }
}

