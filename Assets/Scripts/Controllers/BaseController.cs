using UnityEngine;
using Entities;
using Gameplay;

namespace Controllers
{
    public class BaseController : MonoBehaviour
    {
        private BaseEntity _baseEntity;
        private IEntitiesManager _entitiesManager;
        private bool _isInitialized;
        
        public void Initialize(BaseEntity baseEntity, IEntitiesManager entitiesManager)
        {
            _baseEntity = baseEntity;
            _entitiesManager = entitiesManager;
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

