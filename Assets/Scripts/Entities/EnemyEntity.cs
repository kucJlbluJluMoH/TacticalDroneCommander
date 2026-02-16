using UnityEngine;
using TacticalDroneCommander.Core;

namespace Entities
{
    public class EnemyEntity : Entity
    {
        private float _attackCooldown;
        private float _attackRange;
        private float _attackDamage;
        private float _moveSpeed;
        private float _lastAttackTime;

        public EnemyEntity(string id, GameObject entityObject, GameConfig config) 
            : base(id, config.EnemyDroneHP, config.EnemyDroneHP, entityObject)
        {
            _attackCooldown = config.EnemyAttackCooldown;
            _attackRange = config.EnemyAttackRange;
            _attackDamage = config.EnemyAttackDamage;
            _moveSpeed = config.EnemyDroneSpeed;
            _lastAttackTime = -_attackCooldown;
        }

        public float GetMoveSpeed() => _moveSpeed;
        public float GetAttackRange() => _attackRange;
        public float GetAttackDamage() => _attackDamage;
        public float GetAttackCooldown() => _attackCooldown;
        
        public bool CanAttack()
        {
            return Time.time >= _lastAttackTime + _attackCooldown;
        }
        
        public void RegisterAttack()
        {
            _lastAttackTime = Time.time;
        }
    }
}
