using UnityEngine;
namespace Entities
{
    public class Entity
    {
        private string _id;
        private int _health;
        private int _maxHealth;
        private GameObject _entityObject;
        
        public EntityTag Tag { get; protected set; }

        public Entity(string id, int health, int maxHealth, GameObject entityObject)
        {
            _id = id;
            _health = health;
            _maxHealth = maxHealth;
            _entityObject = entityObject;
        }
        
        public string GetId() => _id;
        
        public GameObject GetGameObject() => _entityObject;
        
        public Transform GetTransform()
        {
            return _entityObject.transform;
        }

        public int GetHealth()
        {
            return _health;
        }
        
        public int GetMaxHealth()
        {
            return _maxHealth;
        }

        public void SetHealth(int value)
        {
            _health = Mathf.Clamp(value, 0, _maxHealth);
        }
        
        public void TakeDamage(int damage)
        {
            if (damage == 0) return;
            if (damage < 0)
            {
                Debug.LogError($"Entity.TakeDamage: negative damage value '{damage}' for entity '{_id}'.");
                return;
            }
            _health = Mathf.Max(0, _health - damage);
        }
        
        public bool IsDead()
        {
            return _health <= 0;
        }
        
        public void Regenerate(int amount)
        {
            if (!IsDead())
            {
                _health = Mathf.Min(_health + amount, _maxHealth);
            }
        }
    }
    public enum EntityTag
    {
        Base,
        Player,
        Enemy
    }
}
