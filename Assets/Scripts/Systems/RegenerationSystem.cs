using Entities;
using UnityEngine;
using TacticalDroneCommander.Core;

namespace TacticalDroneCommander.Systems
{
    public interface IRegenerationSystem
    {
        void ProcessRegeneration(Entity entity, float regenAmount, float regenDelay, float regenRate);
    }
    
    public class RegenerationSystem : IRegenerationSystem
    {
        public void ProcessRegeneration(Entity entity, float regenAmount, float regenDelay, float regenRate)
        {
            if (entity == null || entity.IsDead())
                return;
            
            if (entity.GetHealth() >= entity.GetMaxHealth())
                return;
            
            float timeSinceLastDamage = Time.time - entity.GetLastDamageTime();
            if (timeSinceLastDamage < regenDelay)
                return;
            
            float timeSinceLastRegen = Time.time - entity.GetLastRegenerationTime();
            if (timeSinceLastRegen >= regenRate)
            {
                entity.Regenerate((int)regenAmount);
                entity.SetLastRegenerationTime(Time.time);
            }
        }
    }
}

