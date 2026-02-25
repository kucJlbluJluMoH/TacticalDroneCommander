using UnityEngine;
using DG.Tweening;
using Entities;
using TacticalDroneCommander.Core.Events;
using TacticalDroneCommander.Infrastructure;

namespace Controllers
{
    public class BulletController : MonoBehaviour
    {
        [SerializeField] private float _bulletSpeed = 20f;
        [SerializeField] private float _maxLifetime = 5f;
        
        private Entity _target;
        private Entity _attacker;
        private float _damage;
        private IPoolService _poolService;
        private IEventBus _eventBus;
        private Tween _moveTween;
        private bool _isInitialized;
        
        public void Initialize(Entity attacker, Entity target, float damage, IPoolService poolService, IEventBus eventBus)
        {
            _attacker = attacker;
            _target = target;
            _damage = damage;
            _poolService = poolService;
            _eventBus = eventBus;
            _isInitialized = true;
            
            if (target != null && !target.IsDead())
                MoveToTarget();
            else
                ReturnToPool();
        }
        
        private void MoveToTarget()
        {
            if (_target == null || _target.IsDead())
            {
                ReturnToPool();
                return;
            }
            
            Vector3 targetPosition = _target.GetTransform().position;
            float distance = Vector3.Distance(transform.position, targetPosition);
            float duration = distance / _bulletSpeed;
            
            transform.LookAt(targetPosition);
            
            _moveTween = transform.DOMove(targetPosition, duration)
                .SetEase(Ease.Linear)
                .OnUpdate(() =>
                {
                    if (_target != null && !_target.IsDead())
                    {
                        Vector3 currentTarget = _target.GetTransform().position;
                        transform.LookAt(currentTarget);
                    }
                })
                .OnComplete(OnReachedTarget);
            
            DOVirtual.DelayedCall(_maxLifetime, () =>
            {
                if (_isInitialized)
                    ReturnToPool();
            });
        }
        
        private void OnReachedTarget()
        {
            if (_target != null && !_target.IsDead())
            {
                _target.TakeDamage((int)_damage);
                _eventBus.Publish(new EntityDamagedEvent(_target, _attacker, _damage));
                
                if (_target.IsDead())
                    _eventBus.Publish(new EntityDiedEvent(_target, _target.GetTransform().position));
            }
            
            ReturnToPool();
        }
        
        private void ReturnToPool()
        {
            _isInitialized = false;
            _moveTween?.Kill();
            
            if (_poolService != null)
            {
                _poolService.Return("Bullet", gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void OnDisable()
        {
            _moveTween?.Kill();
        }
    }
}
