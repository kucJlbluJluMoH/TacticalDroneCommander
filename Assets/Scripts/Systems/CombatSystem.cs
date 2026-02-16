using Entities;
using TacticalDroneCommander.Core;
using TacticalDroneCommander.Core.Events;
using UnityEngine;

namespace TacticalDroneCommander.Systems
{
    public interface ICombatSystem
    {
        bool CanAttack(Entity attacker, Entity target);
        void ProcessAttack(Entity attacker, Entity target, Vector3 attackerPosition);
        bool IsInRange(Entity attacker, Entity target);
    }
    
    public class CombatSystem : ICombatSystem
    {
        private readonly IEventBus _eventBus;
        
        public CombatSystem(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public bool CanAttack(Entity attacker, Entity target)
        {
            if (attacker == null || target == null)
                return false;
            
            if (attacker.IsDead() || target.IsDead())
                return false;
            
            return true;
        }

        public bool IsInRange(Entity attacker, Entity target)
        {
            if (attacker == null || target == null)
                return false;

            float attackRange = 0f;
            
            if (attacker is PlayerEntity playerEntity)
                attackRange = playerEntity.GetAttackRange();
            else if (attacker is EnemyEntity enemyEntity)
                attackRange = enemyEntity.GetAttackRange();
            
            float distance = Vector3.Distance(
                attacker.GetTransform().position, 
                target.GetTransform().position);
            
            return distance <= attackRange;
        }

        public void ProcessAttack(Entity attacker, Entity target, Vector3 attackerPosition)
        {
            if (!CanAttack(attacker, target))
                return;

            float damage = 0f;
            
            if (attacker is PlayerEntity playerEntity)
                damage = playerEntity.GetAttackDamage();
            else if (attacker is EnemyEntity enemyEntity)
                damage = enemyEntity.GetAttackDamage();

            target.TakeDamage((int)damage);
            
            _eventBus.Publish(new AttackPerformedEvent(attacker, target, damage));
            _eventBus.Publish(new EntityDamagedEvent(target, attacker, damage));
            
            if (target.IsDead())
            {
                _eventBus.Publish(new EntityDiedEvent(target, target.GetTransform().position));
            }

            Debug.Log($"CombatSystem: {attacker.GetId()} attacked {target.GetId()} for {damage} damage. Target HP: {target.GetHealth()}");
        }
    }
}

