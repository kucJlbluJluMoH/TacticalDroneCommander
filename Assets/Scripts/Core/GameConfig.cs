using UnityEngine;

namespace TacticalDroneCommander.Core
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "TacticalDroneCommander/Game Config")]
    public class GameConfig : ScriptableObject
    {
        [Header("═══════════════ SYSTEM ═══════════════")]
        [Tooltip("Name of save file")]
        public string SaveFileName = "save.json";

        [Header("═══════════════ WAVE SYSTEM ═══════════════")]
        [Tooltip("Base number of enemies per wave")]
        public int BaseEnemiesPerWave = 5;
        [Tooltip("Multiplier for increasing enemy count each wave")]
        public float EnemiesCountMultiplier = 1.3f;
        [Tooltip("Time between waves in seconds")]
        public float TimeBetweenWaves = 10f;

        [Header("═══════════════ BASE ═══════════════")]
        [Tooltip("Base coordinates")]
        public Vector3 BaseCoordinates = new Vector3(0, 0, 0);
        [Tooltip("Base HP")]
        public int BaseHP = 200;
        
        [Header("Base Regeneration")]
        [Tooltip("Base regeneration amount per tick")]
        public int BaseRegenerationAmount = 10;
        [Tooltip("Base regeneration delay after taking damage (seconds)")]
        public float BaseRegenerationDelay = 5f;
        [Tooltip("Base regeneration rate (seconds between ticks)")]
        public float BaseRegenerationRate = 2f;

        [Header("═══════════════ PLAYER DRONE ═══════════════")]
        [Header("Movement")]
        [Tooltip("Player drone speed")]
        public float PlayerDroneSpeed = 5f;
        [Tooltip("Player hover height")]
        public float PlayerHoverHeight = 2f;

        [Header("Combat")]
        [Tooltip("HP of player drone")]
        public int PlayerDroneHP = 100;
        [Tooltip("Player attack damage")]
        public float PlayerAttackDamage = 15f;
        [Tooltip("Player attack range")]
        public float PlayerAttackRange = 10f;
        [Tooltip("Player attack cooldown in seconds")]
        public float PlayerAttackCooldown = 0.5f;

        [Header("Regeneration")]
        [Tooltip("Player regeneration amount per tick")]
        public int PlayerRegenerationAmount = 5;
        [Tooltip("Player regeneration delay after taking damage (seconds)")]
        public float PlayerRegenerationDelay = 3f;
        [Tooltip("Player regeneration rate (seconds between ticks)")]
        public float PlayerRegenerationRate = 1f;

        [Header("═══════════════ ENEMY ═══════════════")]
        [Header("Movement")]
        [Tooltip("Enemy drone speed")]
        public float EnemyDroneSpeed = 3f;
        
        [Header("Combat")]
        [Tooltip("HP of enemy drone")]
        public int EnemyDroneHP = 50;
        [Tooltip("Enemy attack damage")]
        public float EnemyAttackDamage = 10f;
        [Tooltip("Enemy attack range")]
        public float EnemyAttackRange = 2f;
        [Tooltip("Enemy attack cooldown in seconds")]
        public float EnemyAttackCooldown = 1.5f;

        [Header("Behavior")]
        [Tooltip("Probability that enemy will target the player base (0-1). Remainder will target player drones.")]
        [Range(0f, 1f)]
        public float EnemyBaseTargetProbability = 0.6f;

        [Header("═══════════════ UPGRADES ═══════════════")]
        [Tooltip("Chance for upgrade drop on enemy death (0-1)")]
        [Range(0f, 1f)]
        public float UpgradeDropChance = 0.3f;
        [Tooltip("Lifetime of upgrade pickups in seconds")]
        public float UpgradeLifetime = 10f;

        [Header("Upgrade Values")]
        [Tooltip("Upgrade values for each type")]
        public UpgradeValueConfig[] UpgradeValues = new[]
        {
            new UpgradeValueConfig { Type = "AttackSpeed", Value = 0.85f }, // 15% faster
            new UpgradeValueConfig { Type = "AttackRange", Value = 1.2f },  // 20% more range
            new UpgradeValueConfig { Type = "AttackDamage", Value = 1.3f }, // 30% more damage
            new UpgradeValueConfig { Type = "MoveSpeed", Value = 1.15f }    // 15% faster movement
        };

        [Header("═══════════════ OBJECT POOLING ═══════════════")]
        public PoolConfig[] PoolConfigs = new[]
        {
            new PoolConfig { Key = "Enemy", PrefabPath = "Prefabs/Enemy", InitialSize = 50 },
            new PoolConfig { Key = "Bullet", PrefabPath = "Prefabs/Bullet", InitialSize = 50 },
            new PoolConfig { Key = "Drone", PrefabPath = "Prefabs/PlayerDrone", InitialSize = 5 },
            new PoolConfig { Key = "Upgrade", PrefabPath = "Prefabs/Upgrade", InitialSize = 20 }
        };
    }

    [System.Serializable]
    public class PoolConfig
    {
        public string Key;
        public string PrefabPath;
        public int InitialSize;
    }

    [System.Serializable]
    public class UpgradeValueConfig
    {
        public string Type;
        public float Value;
    }
}