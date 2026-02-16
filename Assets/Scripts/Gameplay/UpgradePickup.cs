using UnityEngine;
using DG.Tweening;
using Entities;
using TacticalDroneCommander.Infrastructure;
using TacticalDroneCommander.Core;
using Cysharp.Threading.Tasks;

namespace Gameplay
{
    public class UpgradePickup : MonoBehaviour
    {
        [SerializeField] private float _rotationSpeed = 90f;
        [SerializeField] private float _bobSpeed = 2f;
        [SerializeField] private float _bobHeight = 0.3f;
        
        private UpgradeType _upgradeType;
        private float _upgradeValue;
        private IPoolService _poolService;
        private GameConfig _config;
        private bool _isInitialized;
        private Vector3 _startPosition;
        private Tween _bobTween;
        
        public void Initialize(UpgradeType upgradeType, float upgradeValue, IPoolService poolService, GameConfig config)
        {
            _upgradeType = upgradeType;
            _upgradeValue = upgradeValue;
            _poolService = poolService;
            _config = config;
            _isInitialized = true;
            _startPosition = transform.position;
            
            StartVisualEffects();
            
            DestroyAfterLifetime().Forget();
        }
        
        private void StartVisualEffects()
        {
            transform.DORotate(new Vector3(0, 360, 0), 360f / _rotationSpeed, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart);
            
            _bobTween = transform.DOMoveY(_startPosition.y + _bobHeight, 1f / _bobSpeed)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }
        
        private async UniTaskVoid DestroyAfterLifetime()
        {
            await UniTask.Delay((int)(_config.UpgradeLifetime * 1000));
            
            if (_isInitialized)
            {
                ReturnToPool();
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (!_isInitialized)
                return;
            
            var playerController = other.GetComponent<Controllers.PlayerDroneController>();
            if (playerController != null)
            {
                playerController.ApplyUpgrade(_upgradeType, _upgradeValue);
                ReturnToPool();
            }
        }
        
        private void ReturnToPool()
        {
            _isInitialized = false;
            transform.DOKill();
            _bobTween?.Kill();
            
            if (_poolService != null)
            {
                _poolService.Return("Upgrade", gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void OnDisable()
        {
            transform.DOKill();
            _bobTween?.Kill();
        }
    }
}

