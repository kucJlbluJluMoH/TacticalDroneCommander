using UnityEngine;
using DG.Tweening;
using Entities;
using TacticalDroneCommander.Infrastructure;
using TacticalDroneCommander.Core;
using TacticalDroneCommander.Core.Events;
using TacticalDroneCommander.Systems;
using Gameplay;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace Controllers
{
    public class PlayerDroneController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform _firePoint;
        [SerializeField] private GameObject _selectionIndicator;
        
        private PlayerEntity _playerEntity;
        private IPoolService _poolService;
        private IEntitiesManager _entitiesManager;
        private GameConfig _config;
        
        private ICombatSystem _combatSystem;
        private IRegenerationSystem _regenerationSystem;
        private ITargetingSystem _targetingSystem;
        private IMovementSystem _movementSystem;
        private IEventBus _eventBus;
        
        private bool _isInitialized;
        private bool _isSelected;
        private CancellationTokenSource _cancellationTokenSource;
        
        private void Awake()
        {
            if (_selectionIndicator != null)
            {
                _selectionIndicator.SetActive(false);
            }
            
            _cancellationTokenSource = new CancellationTokenSource();
        }
        
        public void Initialize(
            PlayerEntity playerEntity,
            IPoolService poolService,
            IEntitiesManager entitiesManager,
            GameConfig config,
            ICombatSystem combatSystem,
            IRegenerationSystem regenerationSystem,
            ITargetingSystem targetingSystem,
            IMovementSystem movementSystem,
            IEventBus eventBus)
        {
            _playerEntity = playerEntity;
            _poolService = poolService;
            _entitiesManager = entitiesManager;
            _config = config;
            _combatSystem = combatSystem;
            _regenerationSystem = regenerationSystem;
            _targetingSystem = targetingSystem;
            _movementSystem = movementSystem;
            _eventBus = eventBus;
            
            _isInitialized = true;
            
            AutoAttackLoop(_cancellationTokenSource.Token).Forget();
        }
        
        public void SetSelected(bool selected)
        {
            _isSelected = selected;
            if (_selectionIndicator != null)
            {
                _selectionIndicator.SetActive(selected);
            }
        }
        
        public bool IsSelected() => _isSelected;
        
        public void MoveToPosition(Vector3 targetPosition)
        {
            if (_playerEntity == null || _playerEntity.IsDead())
                return;

            Vector3 adjustedPosition = new Vector3(targetPosition.x, _config.PlayerHoverHeight, targetPosition.z);

            float distance = Vector3.Distance(transform.position, adjustedPosition);
            float duration = distance / _playerEntity.GetMoveSpeed();

            _movementSystem.MoveToPosition(_playerEntity, adjustedPosition, duration);
        }
        
        private async UniTaskVoid AutoAttackLoop(CancellationToken cancellationToken)
        {
            while (_isInitialized && _playerEntity != null && !_playerEntity.IsDead())
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                await UniTask.Yield(cancellationToken);
                
                if (_playerEntity.CanAttack())
                {
                    Entity target = _targetingSystem.FindNearestEnemyForPlayer(_playerEntity, _entitiesManager);
                    
                    if (target != null && _combatSystem.IsInRange(_playerEntity, target))
                    {
                        PerformAttack(target);
                        _playerEntity.RegisterAttack();
                    }
                }
            }
            
            if (_playerEntity != null && _playerEntity.IsDead())
            {
                HandleDeath();
            }
        }
        
        private void Update()
        {
            if (!_isInitialized || _playerEntity == null)
                return;
            
            if (_playerEntity.IsDead())
            {
                HandleDeath();
                return;
            }
            
            _regenerationSystem.ProcessRegeneration(
                _playerEntity,
                _config.PlayerRegenerationAmount,
                _config.PlayerRegenerationDelay,
                _config.PlayerRegenerationRate);
        }
        
        private void HandleDeath()
        {
            if (!_isInitialized)
                return;

            Debug.Log($"PlayerDroneController: Drone {_playerEntity.GetId()} destroyed!");
            
            _cancellationTokenSource?.Cancel();
            
            _eventBus?.Publish(new EntityDiedEvent(_playerEntity, transform.position));
            
            if (_poolService != null)
            {
                _poolService.Return("Drone", gameObject);
            }
            
            _isInitialized = false;
            gameObject.SetActive(false);
        }

        public void Despawn()
        {
            if (!_isInitialized)
                return;

            _isInitialized = false;
            _cancellationTokenSource?.Cancel();
            DOTween.Kill(transform);
            SetSelected(false);

            if (_poolService != null)
                _poolService.Return("Drone", gameObject);
            else
                gameObject.SetActive(false);
        }
        
        private void PerformAttack(Entity target)
        {
            Vector3 direction = (target.GetTransform().position - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                direction.y = 0;
                direction.Normalize();
                transform.rotation = Quaternion.LookRotation(direction);
            }
        
            Vector3 spawnPosition = _firePoint != null ? _firePoint.position : transform.position + transform.forward;
            GameObject bullet = _poolService.Get("Bullet", spawnPosition, Quaternion.identity);
        
            if (bullet != null)
            {
                BulletController bulletController = bullet.GetComponent<BulletController>();
                bulletController?.Initialize(_playerEntity, target, _playerEntity.GetAttackDamage(), _poolService, _eventBus);
            }
        }
        
        public void ApplyUpgrade(UpgradeType upgradeType, float value)
        {
            _playerEntity.ApplyUpgrade(upgradeType, value);
            
            _eventBus.Publish(new UpgradeCollectedEvent(_playerEntity, upgradeType.ToString(), value));
            
            Debug.Log($"PlayerDroneController: Upgrade applied - {upgradeType} x{value}");
        }
        
        private void OnDisable()
        {
            DOTween.Kill(transform);
        }
        
        private void OnDestroy()
        {
            DOTween.Kill(transform);
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }
    }
}

