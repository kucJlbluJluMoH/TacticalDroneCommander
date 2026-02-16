using UnityEngine;

namespace TacticalDroneCommander.Core
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "TacticalDroneCommander/Game Config")]
    public class GameConfig : ScriptableObject
    {
        [Header("Save Settings")]
        [Tooltip("Name of save file")]
        public string SaveFileName = "save.json";
        
        [Header("Gameplay Settings")]
        [Tooltip("Base number of enemies per wave")]
        public int BaseEnemiesPerWave = 5;
        [Tooltip("Multiplier for increasing enemy count each wave")]
        public float EnemiesCountMultiplier = 1.3f;
        [Tooltip("Time between waves in seconds")]
        public float TimeBetweenWaves = 10f;
        [Tooltip("Base coordinates")]
        public Vector3 BaseCoordinates = new Vector3(0, 0, 0);
        
        [Header("Entity Settings")]
        [Tooltip("Player drone speed")]
        public float PlayerDroneSpeed = 5f;
       
        [Tooltip("HP of player drone")]
        public int PlayerDroneHP = 100;

        [Tooltip("Base HP")]
        public int BaseHP = 200;
        
        [Header("Enemy Settings")]
        [Tooltip("HP of enemy drone")]
        public int EnemyDroneHP = 50;
        [Tooltip("Enemy drone speed")]
        public float EnemyDroneSpeed = 3f;
        [Tooltip("Enemy attack damage")]
        public float EnemyAttackDamage = 10f;
        [Tooltip("Enemy attack range")]
        public float EnemyAttackRange = 2f;
        [Tooltip("Enemy attack cooldown in seconds")]
        public float EnemyAttackCooldown = 1.5f;
        [Tooltip("Enemy regeneration rate (HP per second)")]
        public int EnemyRegenerationRate = 0;
        
        [Header("Pool Settings")]
        public PoolConfig[] PoolConfigs = new[]
        {
            new PoolConfig { Key = "Enemy", PrefabPath = "Prefabs/Enemy", InitialSize = 50 },
            new PoolConfig { Key = "Bullet", PrefabPath = "Prefabs/Bullet", InitialSize = 50 },
            new PoolConfig { Key = "Drone", PrefabPath = "Prefabs/PlayerDrone", InitialSize = 5 }
        };
        
    }
    
    [System.Serializable]
    public class PoolConfig
    {
        public string Key;
        public string PrefabPath;
        public int InitialSize;
    }
    
}

