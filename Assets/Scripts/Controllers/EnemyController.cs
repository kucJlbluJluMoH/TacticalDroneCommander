using UnityEngine;
using UnityEngine.AI;
using Entities;
using TacticalDroneCommander.Infrastructure;
using TacticalDroneCommander.Core;
using TacticalDroneCommander.Core.Events;
using TacticalDroneCommander.Systems;
using Gameplay;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Controllers
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyController : MonoBehaviour
    {
        private NavMeshAgent _navMeshAgent;
        private EnemyEntity _enemyEntity;
        private Entity _huntingTarget;
        private bool _isInitialized;
        
        private IPoolService _poolService;
        private IEntitiesManager _entitiesManager;
        private IUpgradeSpawner _upgradeSpawner;
        private GameConfig _config;
        
        private ICombatSystem _combatSystem;
        private IMovementSystem _movementSystem;
        private ITargetingSystem _targetingSystem;
        private IEventBus _eventBus;
        
        private CancellationTokenSource _cancellationTokenSource;
        
        private void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _cancellationTokenSource = new CancellationTokenSource();
        }
        
        public void Initialize(
            EnemyEntity enemyEntity,
            IPoolService poolService,
            IEntitiesManager entitiesManager,
            GameConfig config,
            ICombatSystem combatSystem,
            IMovementSystem movementSystem,
            ITargetingSystem targetingSystem,
            IEventBus eventBus,
            IUpgradeSpawner upgradeSpawner = null)
        {
            _enemyEntity = enemyEntity;
            _poolService = poolService;
            _entitiesManager = entitiesManager;
            _config = config;
            _combatSystem = combatSystem;
            _movementSystem = movementSystem;
            _targetingSystem = targetingSystem;
            _eventBus = eventBus;
            _upgradeSpawner = upgradeSpawner;
            
            _huntingTarget = _targetingSystem.SelectTargetForEnemy(_enemyEntity, _entitiesManager, _config.EnemyBaseTargetProbability);
            
            if (_navMeshAgent != null && _huntingTarget != null)
            {
                _movementSystem.MoveWithNavMesh(_navMeshAgent, _enemyEntity, _huntingTarget.GetTransform().position);
            }
            
            _isInitialized = true;
            
            EnemyAILoop(_cancellationTokenSource.Token).Forget();
        }
        
        private async UniTaskVoid EnemyAILoop(CancellationToken cancellationToken)
        {
            while (_isInitialized && _enemyEntity != null && !_enemyEntity.IsDead())
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                await UniTask.Yield(cancellationToken);
                
                if (_huntingTarget == null || _huntingTarget.IsDead())
                {
                    _huntingTarget = _targetingSystem.SelectTargetForEnemy(_enemyEntity, _entitiesManager, _config.EnemyBaseTargetProbability);
                }
                
                if (_huntingTarget != null && !_huntingTarget.IsDead())
                {
                    _movementSystem.MoveWithNavMesh(_navMeshAgent, _enemyEntity, _huntingTarget.GetTransform().position);
                }
                
                if (_huntingTarget != null && _combatSystem.IsInRange(_enemyEntity, _huntingTarget))
                {
                    if (_enemyEntity.CanAttack())
                    {
                        _combatSystem.ProcessAttack(_enemyEntity, _huntingTarget, transform.position);
                        _enemyEntity.RegisterAttack();
                    }
                }
                
                await UniTask.Delay(100, cancellationToken: cancellationToken);
            }
            
            if (_enemyEntity != null && _enemyEntity.IsDead())
            {
                HandleDeath();
            }
        }
        
        private void HandleDeath()
        {
            if (!_isInitialized)
                return;

            Debug.Log($"EnemyController: Enemy {_enemyEntity.GetId()} died!");
            
            _cancellationTokenSource?.Cancel();
            
            if (_upgradeSpawner != null)
            {
                _upgradeSpawner.TrySpawnUpgrade(transform.position);
            }
            
            _movementSystem.StopMovement(_navMeshAgent);
            
            if (_entitiesManager != null && _enemyEntity != null)
            {
                _entitiesManager.UnregisterEntity(_enemyEntity);
            }
            
            if (_poolService != null)
            {
                _poolService.Return("Enemy", gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }
            
            _isInitialized = false;
        }
        
        private void OnDisable()
        {
            _cancellationTokenSource?.Cancel();
        }
        
        private void OnDestroy()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }
    }
}

