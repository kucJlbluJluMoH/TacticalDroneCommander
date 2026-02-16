using UnityEngine;
using Entities;
using Gameplay;
using TacticalDroneCommander.Core;

namespace Controllers
{
    public class BaseController : MonoBehaviour
    {
        private BaseEntity _baseEntity;
        private IEntitiesManager _entitiesManager;
        private GameConfig _config;
        private bool _isInitialized;
        
        public void Initialize(BaseEntity baseEntity, IEntitiesManager entitiesManager, GameConfig config)
        {
            _baseEntity = baseEntity;
            _entitiesManager = entitiesManager;
            _config = config;
            _isInitialized = true;
            
            Debug.Log($"BaseController: Base initialized at {transform.position}");
        }
        
        private void Update()
        {
            if (!_isInitialized || _baseEntity == null)
                return;
            
            if (_baseEntity.IsDead())
            {
                OnBaseDeath();
                return;
            }
            
            ProcessRegeneration();
        }
        
        private void ProcessRegeneration()
        {
            if (_baseEntity.GetHealth() >= _baseEntity.GetMaxHealth())
                return;
            
            float timeSinceLastDamage = Time.time - _baseEntity.GetLastDamageTime();
            if (timeSinceLastDamage < _config.BaseRegenerationDelay)
                return;
            
            float timeSinceLastRegen = Time.time - _baseEntity.GetLastRegenerationTime();
            if (timeSinceLastRegen >= _config.BaseRegenerationRate)
            {
                _baseEntity.Regenerate(_config.BaseRegenerationAmount);
                _baseEntity.SetLastRegenerationTime(Time.time);
                Debug.Log($"BaseController: Base regenerated {_config.BaseRegenerationAmount} HP. Current HP: {_baseEntity.GetHealth()}/{_baseEntity.GetMaxHealth()}");
            }
        }
        
        private void OnBaseDeath()
        {
            Debug.Log("BaseController: Base destroyed! Game Over!");
            
            if (_entitiesManager != null && _baseEntity != null)
            {
                _entitiesManager.UnregisterEntity(_baseEntity);
            }
            //todo
            
            gameObject.SetActive(false);
        }
        
        private void OnDisable()
        {
            _isInitialized = false;
        }
    }
}

