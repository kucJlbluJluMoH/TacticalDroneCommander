using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Entities;

namespace Gameplay
{
    public interface ITargetFinder
    {
        Entity FindClosestEnemy(Vector3 position, float maxRange);
        List<Entity> FindEnemiesInRange(Vector3 position, float range);
    }
    
    public class TargetFinder : ITargetFinder
    {
        private readonly IEntitiesManager _entitiesManager;
        
        public TargetFinder(IEntitiesManager entitiesManager)
        {
            _entitiesManager = entitiesManager;
        }
        
        public Entity FindClosestEnemy(Vector3 position, float maxRange)
        {
            var enemies = _entitiesManager.GetEntitiesOfType<EnemyEntity>()
                .Where(e => !e.IsDead())
                .ToList();
            
            Entity closestEnemy = null;
            float closestDistance = maxRange;
            
            foreach (var enemy in enemies)
            {
                float distance = Vector3.Distance(position, enemy.GetTransform().position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = enemy;
                }
            }
            
            return closestEnemy;
        }
        
        public List<Entity> FindEnemiesInRange(Vector3 position, float range)
        {
            return _entitiesManager.GetEntitiesOfType<EnemyEntity>()
                .Where(e => !e.IsDead() && Vector3.Distance(position, e.GetTransform().position) <= range)
                .Cast<Entity>()
                .ToList();
        }
    }
}

