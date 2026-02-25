using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace TacticalDroneCommander.Infrastructure
{
    public interface IPoolService
    {
        GameObject Get(string poolKey, Vector3 position, Quaternion rotation);
        void Return(string poolKey, GameObject obj);
        void ReturnAll(string poolKey);
        void CreatePool(string poolKey, GameObject prefab, int initialSize);
    }
    
    public class PoolService : IPoolService
    {
        private class Pool
        {
            public GameObject Prefab;
            public Queue<GameObject> Available = new Queue<GameObject>();
            public List<GameObject> Active = new List<GameObject>();
        }

        private readonly Dictionary<string, Pool> _pools = new Dictionary<string, Pool>();

        public PoolService()
        {
            Debug.Log("PoolService: Initialized");
        }
        
        public void CreatePool(string poolKey, GameObject prefab, int initialSize)
        {
            if (_pools.ContainsKey(poolKey))
            {
                Debug.LogWarning($"PoolService: Pool '{poolKey}' already exists!");
                return;
            }

            var pool = new Pool { Prefab = prefab };
            _pools[poolKey] = pool;

            for (int i = 0; i < initialSize; i++)
            {
                GameObject obj = Object.Instantiate(prefab);
                obj.SetActive(false);
                pool.Available.Enqueue(obj);
            }

            Debug.Log($"PoolService: Created pool '{poolKey}' with {initialSize} objects");
        }
        
        public GameObject Get(string poolKey, Vector3 position, Quaternion rotation)
        {
            if (!_pools.ContainsKey(poolKey))
            {
                Debug.LogError($"PoolService: Pool '{poolKey}' does not exist!");
                return null;
            }

            Pool pool = _pools[poolKey];
            GameObject obj;

            if (pool.Available.Count > 0)
            {
                obj = pool.Available.Dequeue();
            }
            else
            {
                obj = Object.Instantiate(pool.Prefab);
                Debug.LogWarning($"PoolService: Pool '{poolKey}' exhausted, creating new object");
            }

            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.SetActive(true);
            pool.Active.Add(obj);

            return obj;
        }
        
        public void Return(string poolKey, GameObject obj)
        {
            if (!_pools.ContainsKey(poolKey))
            {
                Debug.LogError($"PoolService: Pool '{poolKey}' does not exist!");
                Object.Destroy(obj);
                return;
            }

            Pool pool = _pools[poolKey];
            
            if (pool.Active.Contains(obj))
            {
                pool.Active.Remove(obj);
            }

            obj.SetActive(false);
            pool.Available.Enqueue(obj);
        }
        
        public void ReturnAll(string poolKey)
        {
            if (!_pools.ContainsKey(poolKey)) return;

            Pool pool = _pools[poolKey];
            
            var activeObjects = new List<GameObject>(pool.Active);
            
            foreach (var obj in activeObjects)
            {
                Return(poolKey, obj);
            }

            Debug.Log($"PoolService: Returned all objects in pool '{poolKey}'");
        }
    }
}

