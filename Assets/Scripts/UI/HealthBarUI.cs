using UnityEngine;
using UnityEngine.UI;
using Entities;

namespace UI
{
    public class HealthBarUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Canvas _canvas;
        [SerializeField] private Image _healthBarFill;
        [SerializeField] private Image _healthBarBackground;
        
        [Header("Settings")]
        [SerializeField] private Vector3 _offset = new Vector3(0, 2f, 0);
        [SerializeField] private bool _hideWhenFull = true;
        [SerializeField] private Color _fullHealthColor = Color.green;
        [SerializeField] private Color _lowHealthColor = Color.red;
        
        private Entity _entity;
        private Camera _mainCamera;
        private Transform _target;
        
        private void Awake()
        {
            _mainCamera = Camera.main;
            
            if (_canvas == null)
            {
                _canvas = GetComponent<Canvas>();
            }
            
            if (_canvas != null)
            {
                _canvas.renderMode = RenderMode.WorldSpace;
            }
        }
        
        public void Initialize(Entity entity)
        {
            _entity = entity;
            _target = entity.GetTransform();
            
            UpdateHealthBar();
        }
        
        private void LateUpdate()
        {
            if (_entity == null || _entity.IsDead())
            {
                gameObject.SetActive(false);
                return;
            }
            
            UpdateHealthBar();
            UpdatePosition();
            FaceCamera();
        }
        
        private void UpdateHealthBar()
        {
            if (_entity == null || _healthBarFill == null)
                return;
            
            float healthPercent = (float)_entity.GetHealth() / _entity.GetMaxHealth();
            _healthBarFill.fillAmount = healthPercent;
            
            _healthBarFill.color = Color.Lerp(_lowHealthColor, _fullHealthColor, healthPercent);
            
            if (_hideWhenFull && healthPercent >= 1f)
            {
                _canvas.enabled = false;
            }
            else
            {
                _canvas.enabled = true;
            }
        }
        
        private void UpdatePosition()
        {
            if (_target != null)
            {
                transform.position = _target.position + _offset;
            }
        }
        
        private void FaceCamera()
        {
            if (_mainCamera != null)
            {
                transform.LookAt(transform.position + _mainCamera.transform.rotation * Vector3.forward,
                    _mainCamera.transform.rotation * Vector3.up);
            }
        }
    }
}

