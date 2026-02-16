using Entities;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

namespace TacticalDroneCommander.Systems
{
    public interface IMovementSystem
    {
        void MoveToPosition(Entity entity, Vector3 targetPosition, float duration);
        void MoveWithNavMesh(NavMeshAgent agent, Entity entity, Vector3 targetPosition);
        void StopMovement(NavMeshAgent agent);
        bool HasReachedDestination(NavMeshAgent agent);
    }
    
    public class MovementSystem : IMovementSystem
    {
        public void MoveToPosition(Entity entity, Vector3 targetPosition, float duration)
        {
            if (entity == null)
                return;

            var transform = entity.GetTransform();
            
            transform.DOMove(targetPosition, duration)
                .SetEase(Ease.InOutQuad);

            Vector3 direction = (targetPosition - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                direction.y = 0;
                direction.Normalize();
                
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.DORotateQuaternion(lookRotation, 0.3f);
            }
        }

        public void MoveWithNavMesh(NavMeshAgent agent, Entity entity, Vector3 targetPosition)
        {
            if (agent == null || !agent.isActiveAndEnabled || entity == null)
                return;

            if (entity is EnemyEntity enemyEntity)
            {
                agent.speed = enemyEntity.GetMoveSpeed();
                agent.stoppingDistance = enemyEntity.GetAttackRange();
            }
            else if (entity is PlayerEntity playerEntity)
            {
                agent.speed = playerEntity.GetMoveSpeed();
            }

            agent.SetDestination(targetPosition);
        }

        public void StopMovement(NavMeshAgent agent)
        {
            if (agent != null && agent.isActiveAndEnabled)
            {
                agent.isStopped = true;
            }
        }

        public bool HasReachedDestination(NavMeshAgent agent)
        {
            if (agent == null || !agent.isActiveAndEnabled)
                return false;

            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }

            return false;
        }
    }
}

