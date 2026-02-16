using System.Collections.Generic;
using System.Linq;
using Entities;
using Gameplay;
using UnityEngine;

namespace TacticalDroneCommander.Systems
{
    public interface ITargetingSystem
    {
        Entity FindNearestTarget(Entity seeker, IEntitiesManager entitiesManager, EntityTag targetTag);
        Entity FindNearestEnemyForPlayer(Entity player, IEntitiesManager entitiesManager);
        Entity SelectTargetForEnemy(Entity enemy, IEntitiesManager entitiesManager, float baseProbability);
    }
    
    public class TargetingSystem : ITargetingSystem
    {
        public Entity FindNearestTarget(Entity seeker, IEntitiesManager entitiesManager, EntityTag targetTag)
        {
            if (seeker == null || entitiesManager == null)
                return null;

            var allEntities = entitiesManager.GetAllEntities();
            Entity nearestTarget = null;
            float minDistance = float.MaxValue;

            foreach (var entity in allEntities)
            {
                if (entity.Tag != targetTag || entity.IsDead())
                    continue;

                float distance = Vector3.Distance(
                    seeker.GetTransform().position,
                    entity.GetTransform().position);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestTarget = entity;
                }
            }

            return nearestTarget;
        }

        public Entity FindNearestEnemyForPlayer(Entity player, IEntitiesManager entitiesManager)
        {
            if (player == null || entitiesManager == null)
                return null;

            var enemies = entitiesManager.GetEntitiesOfType<EnemyEntity>().Where(e => !e.IsDead());
            
            Entity nearestEnemy = null;
            float minDistance = float.MaxValue;

            foreach (var enemy in enemies)
            {
                float distance = Vector3.Distance(
                    player.GetTransform().position,
                    enemy.GetTransform().position);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestEnemy = enemy;
                }
            }

            return nearestEnemy;
        }

        public Entity SelectTargetForEnemy(Entity enemy, IEntitiesManager entitiesManager, float baseProbability)
        {
            if (enemy == null || entitiesManager == null)
                return null;

            Entity baseEntity = entitiesManager.GetEntity("base");
            
            float roll = Random.Range(0f, 1f);
            
            if (roll < baseProbability)
            {
                if (baseEntity != null && !baseEntity.IsDead())
                {
                    Debug.Log($"Enemy {enemy.GetId()} targeting BASE (roll: {roll:F2})");
                    return baseEntity;
                }
            }
            
            var playerDrones = entitiesManager.GetEntitiesOfType<PlayerEntity>()
                .Where(p => !p.IsDead())
                .ToList();
            
            if (playerDrones.Count > 0)
            {
                int randomIndex = Random.Range(0, playerDrones.Count);
                var target = playerDrones[randomIndex];
                Debug.Log($"Enemy {enemy.GetId()} targeting DRONE {target.GetId()} (roll: {roll:F2})");
                return target;
            }
            
            if (baseEntity != null && !baseEntity.IsDead())
            {
                Debug.Log($"Enemy {enemy.GetId()} targeting BASE (fallback)");
                return baseEntity;
            }

            return null;
        }
    }
}

