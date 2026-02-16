using UnityEngine;
namespace Entities
{
    public class Entity
    {
        private string _id;
        private int _health;
        private int _maxHealth;
        private int _regenerationRate;
        private GameObject _entityObject;
        public Entity(string id, int health, int maxHealth, int regenerationRate, GameObject entityObject)
        {
            _id = id;
            _health = health;
            _maxHealth = maxHealth;
            _regenerationRate = regenerationRate;
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

        public void SetHealth(int value)
        {
            _health = Mathf.Clamp(value, 0, _maxHealth);
        }
        
        public void TakeDamage(int damage)
        {
            _health = Mathf.Max(0, _health - damage);
        }
        
        public bool IsDead()
        {
            return _health <= 0;
        }
    }
}
