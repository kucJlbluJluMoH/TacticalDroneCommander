using UnityEngine;
using Entities;

namespace TacticalDroneCommander.Core.Events
{
    public class EntitySpawnedEvent : IGameEvent
    {
        public Entity Entity { get; }
        public Vector3 Position { get; }
        
        public EntitySpawnedEvent(Entity entity, Vector3 position)
        {
            Entity = entity;
            Position = position;
        }
    }
    
    public class EntityDiedEvent : IGameEvent
    {
        public Entity Entity { get; }
        public Vector3 Position { get; }
        
        public EntityDiedEvent(Entity entity, Vector3 position)
        {
            Entity = entity;
            Position = position;
        }
    }
    
    public class EntityDamagedEvent : IGameEvent
    {
        public Entity Victim { get; }
        public Entity Attacker { get; }
        public float Damage { get; }
        
        public EntityDamagedEvent(Entity victim, Entity attacker, float damage)
        {
            Victim = victim;
            Attacker = attacker;
            Damage = damage;
        }
    }
    
    public class WaveStartedEvent : IGameEvent
    {
        public int WaveNumber { get; }
        public int EnemyCount { get; }
        
        public WaveStartedEvent(int waveNumber, int enemyCount)
        {
            WaveNumber = waveNumber;
            EnemyCount = enemyCount;
        }
    }
    
    public class WaveCompletedEvent : IGameEvent
    {
        public int WaveNumber { get; }
        
        public WaveCompletedEvent(int waveNumber)
        {
            WaveNumber = waveNumber;
        }
    }
    
    public class AttackPerformedEvent : IGameEvent
    {
        public Entity Attacker { get; }
        public Entity Target { get; }
        public float Damage { get; }
        
        public AttackPerformedEvent(Entity attacker, Entity target, float damage)
        {
            Attacker = attacker;
            Target = target;
            Damage = damage;
        }
    }
    
    public class UpgradeCollectedEvent : IGameEvent
    {
        public Entity Collector { get; }
        public string UpgradeType { get; }
        public float Value { get; }
        
        public UpgradeCollectedEvent(Entity collector, string upgradeType, float value)
        {
            Collector = collector;
            UpgradeType = upgradeType;
            Value = value;
        }
    }
    
    public class UpgradeSpawnedEvent : IGameEvent
    {
        public Vector3 Position { get; }
        public string UpgradeType { get; }
        
        public UpgradeSpawnedEvent(Vector3 position, string upgradeType)
        {
            Position = position;
            UpgradeType = upgradeType;
        }
    }
    
    public class GameStateChangedEvent : IGameEvent
    {
        public GameState PreviousState { get; }
        public GameState NewState { get; }
        
        public GameStateChangedEvent(GameState previousState, GameState newState)
        {
            PreviousState = previousState;
            NewState = newState;
        }
    }

    public class GameOverEvent : IGameEvent
    {
        public bool PlayerWon { get; }
        public int WaveNumber { get; }
        public GameOverEvent(bool playerWon, int waveNumber)
        {
            PlayerWon = playerWon;
            WaveNumber = waveNumber;
        }
    }
}

