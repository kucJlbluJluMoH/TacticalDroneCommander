using UnityEngine;
using DG.Tweening;
using Entities;
using TacticalDroneCommander.Infrastructure;
using TacticalDroneCommander.Core;
using Gameplay;
using Cysharp.Threading.Tasks;

namespace Controllers
{
    public class PlayerDroneController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform _firePoint;
        [SerializeField] private GameObject _selectionIndicator;
        
        private PlayerEntity _playerEntity;
        private ITargetFinder _targetFinder;
        private IPoolService _poolService;
        private IEntitiesManager _entitiesManager;
        private GameConfig _config;
        
        private bool _isInitialized;
        private bool _isSelected;
        private Tween _moveTween;
        private Vector3 _targetPosition;
        private bool _isMoving;
        
        private void Awake()
        {
            if (_selectionIndicator != null)
            {
                _selectionIndicator.SetActive(false);
            }
        }
        
        public void Initialize(
            PlayerEntity playerEntity, 
            ITargetFinder targetFinder, 
            IPoolService poolService,
            IEntitiesManager entitiesManager,
            GameConfig config)
        {
            _playerEntity = playerEntity;
            _targetFinder = targetFinder;
            _poolService = poolService;
            _entitiesManager = entitiesManager;
            _config = config;
            
            _targetPosition = transform.position;
            _isInitialized = true;
            
            AutoAttackLoop().Forget();
        }
        
        public void SetSelected(bool selected)
        {
            _isSelected = selected;
            if (_selectionIndicator != null)
            {
                _selectionIndicator.SetActive(selected);
            }
        }
        
        public bool IsSelected()
        {
            return _isSelected;
        }
        
        public void MoveToPosition(Vector3 targetPosition)
        {
            _targetPosition = new Vector3(targetPosition.x, _config.PlayerHoverHeight, targetPosition.z);

            _moveTween?.Kill();

            float distance = Vector3.Distance(transform.position, _targetPosition);
            float duration = distance / _playerEntity.GetMoveSpeed();

            _isMoving = true;
            _moveTween = transform.DOMove(_targetPosition, duration)
                .SetEase(Ease.InOutQuad)
                .OnComplete(() => _isMoving = false);

            Vector3 direction = (_targetPosition - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                direction.y = 0;
                direction.Normalize();
        
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.DORotateQuaternion(lookRotation, 0.3f);
            }
        }

        
        private async UniTaskVoid AutoAttackLoop()
        {
            while (_isInitialized && _playerEntity != null && !_playerEntity.IsDead())
            {
                await UniTask.Yield();
                
                if (_playerEntity.CanAttack())
                {
                    Entity target = _targetFinder.FindClosestEnemy(transform.position, _playerEntity.GetAttackRange());
                    
                    if (target != null)
                    {
                        Attack(target);
                        _playerEntity.RegisterAttack();
                    }
                }
            }
            
            if (_playerEntity != null && _playerEntity.IsDead())
            {
                OnDroneDeath();
            }
        }
        
        private void Update()
        {
            if (!_isInitialized || _playerEntity == null)
                return;
            
            if (_playerEntity.IsDead())
            {
                OnDroneDeath();
                return;
            }
            
            ProcessRegeneration();
        }
        
        private void ProcessRegeneration()
        {
            if (_playerEntity.GetHealth() >= _playerEntity.GetMaxHealth())
                return;
            
            float timeSinceLastDamage = Time.time - _playerEntity.GetLastDamageTime();
            if (timeSinceLastDamage < _config.PlayerRegenerationDelay)
                return;
            
            float timeSinceLastRegen = Time.time - _playerEntity.GetLastRegenerationTime();
            if (timeSinceLastRegen >= _config.PlayerRegenerationRate)
            {
                _playerEntity.Regenerate(_config.PlayerRegenerationAmount);
                _playerEntity.SetLastRegenerationTime(Time.time);
            }
        }
        
        private void OnDroneDeath()
        {
            Debug.Log($"Player drone {_playerEntity.GetId()} destroyed!");
            
            if (_entitiesManager != null && _playerEntity != null)
            {
                _entitiesManager.UnregisterEntity(_playerEntity);
            }
            if (_poolService != null)
            {
                _poolService.Return("Drone", gameObject);
            }
            _isInitialized = false;
            gameObject.SetActive(false);//todo
        }
        
private void Attack(Entity target)
       {
           Vector3 direction = (target.GetTransform().position - transform.position).normalized;
           if (direction != Vector3.zero)
           {
               direction.y = 0;
               direction.Normalize();
               
               Quaternion lookRotation = Quaternion.LookRotation(direction);
               transform.rotation = lookRotation;
           }
       
           Vector3 spawnPosition = _firePoint != null ? _firePoint.position : transform.position + transform.forward;
           GameObject bullet = _poolService.Get("Bullet", spawnPosition, Quaternion.identity);
       
           if (bullet != null)
           {
               BulletController bulletController = bullet.GetComponent<BulletController>();
               if (bulletController != null)
               {
                   bulletController.Initialize(target, _playerEntity.GetAttackDamage(), _poolService);
               }
           }
       
           Debug.Log($"Player attacking {target.GetId()} for {_playerEntity.GetAttackDamage()} damage!");
       }
       
        
        public void ApplyUpgrade(UpgradeType upgradeType, float value)
        {
            _playerEntity.ApplyUpgrade(upgradeType, value);
            Debug.Log($"Player received upgrade: {upgradeType} x{value}");
        }
        
        private void OnDisable()
        {
            _moveTween?.Kill();
        }
        
        private void OnDestroy()
        {
            _moveTween?.Kill();
        }
    }
}


