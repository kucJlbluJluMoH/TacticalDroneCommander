using UnityEngine;
using Entities;
using Gameplay;
using TacticalDroneCommander.Core;
using TacticalDroneCommander.Core.Events;
using TacticalDroneCommander.Systems;

namespace Controllers
{
    public class BaseController : MonoBehaviour
    {
        private BaseEntity _baseEntity;
        private IEntitiesManager _entitiesManager;
        private IGameStateMachine _stateMachine;
        private GameConfig _config;
        private bool _isInitialized;
        
        private IRegenerationSystem _regenerationSystem;
        private IEventBus _eventBus;
        
        public void Initialize(
            BaseEntity baseEntity,
            IEntitiesManager entitiesManager,
            IGameStateMachine stateMachine,
            GameConfig config,
            IRegenerationSystem regenerationSystem,
            IEventBus eventBus)
        {
            _baseEntity = baseEntity;
            _entitiesManager = entitiesManager;
            _stateMachine = stateMachine;
            _config = config;
            _regenerationSystem = regenerationSystem;
            _eventBus = eventBus;
            _isInitialized = true;
            
            Debug.Log($"BaseController: Base initialized at {transform.position}");
        }
        
        private void Update()
        {
            if (!_isInitialized || _baseEntity == null)
                return;
            
            if (_baseEntity.IsDead())
            {
                HandleDeath();
                return;
            }
            
            _regenerationSystem.ProcessRegeneration(
                _baseEntity,
                _config.BaseRegenerationAmount,
                _config.BaseRegenerationDelay,
                _config.BaseRegenerationRate);
        }
        
        private void HandleDeath()
        {
            if (!_isInitialized)
                return;

            Debug.Log("BaseController: Base destroyed! Publishing EntityDiedEvent...");
            
            _eventBus?.Publish(new EntityDiedEvent(_baseEntity, transform.position));
            
            if (_entitiesManager != null && _baseEntity != null)
            {
                _entitiesManager.UnregisterEntity(_baseEntity);
            }
            
            gameObject.SetActive(false);
            _isInitialized = false;
        }
        
        private void OnDisable()
        {
            _isInitialized = false;
        }
    }
}

