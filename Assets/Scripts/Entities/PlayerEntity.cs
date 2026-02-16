using UnityEngine;
using TacticalDroneCommander.Core;

namespace Entities
{
    public class PlayerEntity : Entity
    {
        private float _attackCooldown;
        private float _attackRange;
        private float _attackDamage;
        private float _moveSpeed;
        private float _lastAttackTime;
        
        private float _attackCooldownMultiplier = 1f;
        private float _attackRangeMultiplier = 1f;
        private float _attackDamageMultiplier = 1f;
        private float _moveSpeedMultiplier = 1f;

        public PlayerEntity(string id, GameObject entityObject, GameConfig config) 
            : base(id, config.PlayerDroneHP, config.PlayerDroneHP, entityObject)
        {
            _attackCooldown = config.PlayerAttackCooldown;
            _attackRange = config.PlayerAttackRange;
            _attackDamage = config.PlayerAttackDamage;
            _moveSpeed = config.PlayerDroneSpeed;
            _lastAttackTime = -_attackCooldown;
        }

        public float GetMoveSpeed() => _moveSpeed * _moveSpeedMultiplier;
        public float GetAttackRange() => _attackRange * _attackRangeMultiplier;
        public float GetAttackDamage() => _attackDamage * _attackDamageMultiplier;
        public float GetAttackCooldown() => _attackCooldown * _attackCooldownMultiplier;
        
        public bool CanAttack()
        {
            return Time.time >= _lastAttackTime + GetAttackCooldown();
        }
        
        public void RegisterAttack()
        {
            _lastAttackTime = Time.time;
        }
        
        public void ApplyUpgrade(UpgradeType upgradeType, float value)
        {
            switch (upgradeType)
            {
                case UpgradeType.AttackSpeed:
                    _attackCooldownMultiplier *= value;
                    break;
                case UpgradeType.AttackRange:
                    _attackRangeMultiplier *= value;
                    break;
                case UpgradeType.AttackDamage:
                    _attackDamageMultiplier *= value;
                    break;
                case UpgradeType.MoveSpeed:
                    _moveSpeedMultiplier *= value;
                    break;
            }
        }
    }
    
    public enum UpgradeType
    {
        AttackSpeed,
        AttackRange,
        AttackDamage,
        MoveSpeed,
    }
}

