using UnityEngine;
using UnityEngine.AI;
using Entities;
using TacticalDroneCommander.Infrastructure;
using Gameplay;

namespace Controllers
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyController : MonoBehaviour
    {
        private NavMeshAgent _navMeshAgent;
        private EnemyEntity _enemyEntity;
        private Vector3 _targetPosition;
        private Entity _targetEntity;
        private bool _isInitialized;
        private IPoolService _poolService;
        private IEntitiesManager _entitiesManager;
        
        private void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
        }
        
        public void Initialize(EnemyEntity enemyEntity, IPoolService poolService, IEntitiesManager entitiesManager)
        {
            _enemyEntity = enemyEntity;
            _poolService = poolService;
            _entitiesManager = entitiesManager;
            
            if (_navMeshAgent != null)
            {
                _navMeshAgent.speed = enemyEntity.GetMoveSpeed();
                _navMeshAgent.stoppingDistance = enemyEntity.GetAttackRange();
                _navMeshAgent.SetDestination(_targetPosition);
            }
            
            _isInitialized = true;
        }
        
        public void SetTarget(Vector3 targetPosition)
        {
            _targetPosition = targetPosition;
            if (_navMeshAgent != null && _navMeshAgent.isActiveAndEnabled)
            {
                _navMeshAgent.SetDestination(_targetPosition);
            }
        }
        
        public void SetTargetEntity(Entity targetEntity)
        {
            _targetEntity = targetEntity;
            SetTarget(targetEntity.GetTransform().position);
        }
        
        private void Update()
        {
            if (!_isInitialized || _enemyEntity == null || _enemyEntity.IsDead())
            {
                if (_enemyEntity != null && _enemyEntity.IsDead())
                {
                    OnEnemyDeath();
                }
                return;
            }
            
            float distanceToTarget = Vector3.Distance(transform.position, _targetPosition);
            
            if (distanceToTarget <= _enemyEntity.GetAttackRange())
            {
                if (_navMeshAgent.isActiveAndEnabled)
                {
                    _navMeshAgent.isStopped = true;
                }
                
                TryAttack();
            }
            else
            {
                if (_navMeshAgent.isActiveAndEnabled && _navMeshAgent.isStopped)
                {
                    _navMeshAgent.isStopped = false;
                }
            }
        }
        
        private void TryAttack()
        {
            if (!_enemyEntity.CanAttack())
                return;

            if (_targetEntity == null || _targetEntity.IsDead())
                return;
            float distanceToTarget = Vector3.Distance(transform.position, _targetEntity.GetTransform().position);
            if (!(distanceToTarget <= _enemyEntity.GetAttackRange()))
                return;
            _targetEntity.TakeDamage((int)_enemyEntity.GetAttackDamage());
            _enemyEntity.RegisterAttack();
                    
            Debug.Log($"Enemy {_enemyEntity.GetId()} attacked target for {_enemyEntity.GetAttackDamage()} damage!");
        }
        
        private void OnEnemyDeath()
        {
            Debug.Log($"Enemy {_enemyEntity.GetId()} died!");
            
            if (_navMeshAgent != null)
            {
                _navMeshAgent.isStopped = true;
            }
            
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
        }
        
        private void OnDisable()
        {
            _isInitialized = false;
            try
            {
                if (_navMeshAgent != null && _navMeshAgent.isActiveAndEnabled && _navMeshAgent.isOnNavMesh)
                {
                    _navMeshAgent.ResetPath();
                }
            }
            catch
            {
            }

        }
    }
}

