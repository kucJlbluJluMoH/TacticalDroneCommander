using UnityEngine;
using UnityEngine.AI;
using Entities;
using TacticalDroneCommander.Infrastructure;
using Gameplay;
using TacticalDroneCommander.Core;
using VContainer;

namespace Controllers
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyController : MonoBehaviour
    {
        private NavMeshAgent _navMeshAgent;
        private EnemyEntity _enemyEntity;
        private Vector3 _targetPosition;
        private Entity _targetEntity;
        private Entity _currentAttackTarget;
        private Entity _huntingTarget;
        private bool _isHuntingBase;
        private bool _isInitialized;
        private IPoolService _poolService;
        private IEntitiesManager _entitiesManager;
        private IUpgradeSpawner _upgradeSpawner;
        private GameConfig _config;
        
        private void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
        }
        
        public void Initialize(EnemyEntity enemyEntity, IPoolService poolService, IEntitiesManager entitiesManager, GameConfig config, IUpgradeSpawner upgradeSpawner = null)
        {
            _enemyEntity = enemyEntity;
            _poolService = poolService;
            _entitiesManager = entitiesManager;
            _upgradeSpawner = upgradeSpawner;
            _config = config;
            
            SelectHuntingTarget();
            
            if (_navMeshAgent != null)
            {
                _navMeshAgent.speed = enemyEntity.GetMoveSpeed();
                _navMeshAgent.stoppingDistance = enemyEntity.GetAttackRange();
                
                if (_huntingTarget != null)
                {
                    UpdateNavigationToHuntingTarget();
                }
                else if (_targetEntity != null)
                {
                    _navMeshAgent.SetDestination(_targetPosition);
                }
            }
            
            _isInitialized = true;
        }
        
        private void SelectHuntingTarget()
        {
            if (_entitiesManager == null)
                return;
            
            Entity baseEntity = _entitiesManager.GetEntity("base");
            
            var baseProbability = _config.EnemyBaseTargetProbability;
            
            float roll = Random.Range(0f, 1f);
            
            if (roll < baseProbability)
            {
                _isHuntingBase = true;
                _huntingTarget = baseEntity;
                Debug.Log($"Enemy {_enemyEntity.GetId()} will hunt the BASE (rolled {roll:F2} < {baseProbability:F2})");
            }
            else
            {
                _isHuntingBase = false;
                _huntingTarget = SelectRandomPlayerDrone();
                
                if (_huntingTarget != null)
                {
                    Debug.Log($"Enemy {_enemyEntity.GetId()} will hunt DRONE {_huntingTarget.GetId()} (rolled {roll:F2} >= {baseProbability:F2})");
                }
                else
                {
                    _isHuntingBase = true;
                    _huntingTarget = baseEntity;
                    Debug.Log($"Enemy {_enemyEntity.GetId()} will hunt the BASE (no drones available)");
                }
            }
        }
        
        private Entity SelectRandomPlayerDrone()
        {
            if (_entitiesManager == null)
                return null;
            
            var allEntities = _entitiesManager.GetAllEntities();
            var playerDrones = new System.Collections.Generic.List<Entity>();
            
            foreach (var entity in allEntities)
            {
                if (entity is PlayerEntity && !entity.IsDead())
                {
                    playerDrones.Add(entity);
                }
            }
            
            if (playerDrones.Count == 0)
                return null;
            
            int randomIndex = Random.Range(0, playerDrones.Count);
            return playerDrones[randomIndex];
        }
        
        private void UpdateNavigationToHuntingTarget()
        {
            if (_huntingTarget == null || _huntingTarget.IsDead() || _navMeshAgent == null)
                return;
            
            Vector3 targetPos = _huntingTarget.GetTransform().position;
            
            targetPos.y = transform.position.y;
            
            _targetPosition = targetPos;
            
            if (_navMeshAgent.isActiveAndEnabled)
            {
                _navMeshAgent.SetDestination(_targetPosition);
            }
        }

        private void SetTarget(Vector3 targetPosition)
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
            
            if (_huntingTarget != null && _huntingTarget.IsDead())
            {
                if (!_isHuntingBase)
                {
                    _huntingTarget = SelectRandomPlayerDrone();
                    if (_huntingTarget == null)
                    {
                        _isHuntingBase = true;
                        _huntingTarget = _entitiesManager.GetEntity("base");
                        Debug.Log($"Enemy {_enemyEntity.GetId()} switching to hunt BASE (no drones left)");
                    }
                    else
                    {
                        Debug.Log($"Enemy {_enemyEntity.GetId()} switching to hunt DRONE {_huntingTarget.GetId()}");
                    }
                }
            }
            
            if (_huntingTarget != null && !_huntingTarget.IsDead())
            {
                UpdateNavigationToHuntingTarget();
            }
            
            if (_navMeshAgent.isActiveAndEnabled && (_navMeshAgent.isStopped || !_navMeshAgent.hasPath))
            {
                _navMeshAgent.isStopped = false;
                if (_huntingTarget != null)
                {
                    UpdateNavigationToHuntingTarget();
                }
                else
                {
                    _navMeshAgent.SetDestination(_targetPosition);
                }
            }
            
            DetermineAttackTarget();
            
            if (_currentAttackTarget != null)
            {
                float distanceToAttackTarget = CalculateDistanceIgnoringHeight(_currentAttackTarget.GetTransform().position);
                
                if (distanceToAttackTarget <= _enemyEntity.GetAttackRange())
                {
                    TryAttack();
                }
            }
        }
        
        private float CalculateDistanceIgnoringHeight(Vector3 targetPosition)
        {
            Vector3 myPos = transform.position;
            myPos.y = 0;
            targetPosition.y = 0;
            return Vector3.Distance(myPos, targetPosition);
        }
        
        private void DetermineAttackTarget()
        {
            if (_huntingTarget != null && !_huntingTarget.IsDead())
            {
                float distanceToHuntingTarget = CalculateDistanceIgnoringHeight(_huntingTarget.GetTransform().position);
                if (distanceToHuntingTarget <= _enemyEntity.GetAttackRange())
                {
                    _currentAttackTarget = _huntingTarget;
                    return;
                }
            }
            
            if (_targetEntity != null && !_targetEntity.IsDead())
            {
                float distanceToCore = CalculateDistanceIgnoringHeight(_targetEntity.GetTransform().position);
                if (distanceToCore <= _enemyEntity.GetAttackRange())
                {
                    _currentAttackTarget = _targetEntity;
                    return;
                }
            }
            
            Entity closestPlayerDrone = FindClosestPlayerDroneInRange();
            if (closestPlayerDrone != null)
            {
                _currentAttackTarget = closestPlayerDrone;
                return;
            }
            
            _currentAttackTarget = null;
        }
        
        private Entity FindClosestPlayerDroneInRange()
        {
            if (_entitiesManager == null)
                return null;
            
            var allEntities = _entitiesManager.GetAllEntities();
            Entity closestDrone = null;
            float closestDistance = float.MaxValue;
            
            foreach (var entity in allEntities)
            {
                if (entity is PlayerEntity && !entity.IsDead())
                {
                    float distance = CalculateDistanceIgnoringHeight(entity.GetTransform().position);
                    if (distance <= _enemyEntity.GetAttackRange() && distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestDrone = entity;
                    }
                }
            }
            
            return closestDrone;
        }
        
        private void TryAttack()
        {
            if (!_enemyEntity.CanAttack())
                return;

            if (_currentAttackTarget == null || _currentAttackTarget.IsDead())
                return;
                
            float distanceToTarget = CalculateDistanceIgnoringHeight(_currentAttackTarget.GetTransform().position);
            if (!(distanceToTarget <= _enemyEntity.GetAttackRange()))
                return;
                
            _currentAttackTarget.TakeDamage((int)_enemyEntity.GetAttackDamage());
            _enemyEntity.RegisterAttack();
                    
            Debug.Log($"Enemy {_enemyEntity.GetId()} attacked {_currentAttackTarget.GetId()} for {_enemyEntity.GetAttackDamage()} damage!");
        }
        
        private void OnEnemyDeath()
        {
            Debug.Log($"Enemy {_enemyEntity.GetId()} died!");
            
            if (_upgradeSpawner != null)
            {
                _upgradeSpawner.TrySpawnUpgrade(transform.position);
            }
            
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
            }//todo 

        }
    }
}

