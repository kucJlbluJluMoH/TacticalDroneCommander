using VContainer;
using VContainer.Unity;
using UnityEngine;
using TacticalDroneCommander.Infrastructure;
using TacticalDroneCommander.Core.Events;
using TacticalDroneCommander.Systems;
using Gameplay;
using UI;

namespace TacticalDroneCommander.Core
{
    public class GameInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            Debug.Log("GameInstaller: Registering services...");
        
            //core systems
            builder.Register<EventBus>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<GameStateMachine>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            
            //infrastructure
            builder.Register<AssetProvider>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<SaveLoadService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<PoolService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<PoolInitializer>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<InputController>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<HealthBarService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            
            //gameplay systems
            builder.Register<CombatSystem>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<RegenerationSystem>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<MovementSystem>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<TargetingSystem>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            
            //managers and services
            builder.Register<EntitiesManager>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<TargetFinder>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<BaseSpawner>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<PlayerSpawner>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<UpgradeSpawner>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<EnemySpawner>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<WaveManager>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<PlayerDroneManager>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            
            Debug.Log("GameInstaller: All services registered successfully!");
        }
    }
}
