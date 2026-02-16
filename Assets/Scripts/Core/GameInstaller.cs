using VContainer;
using VContainer.Unity;
using UnityEngine;
using TacticalDroneCommander.Infrastructure;
using Gameplay;
using UI;

namespace TacticalDroneCommander.Core
{
    public class GameInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            Debug.Log("GameInstaller: Registering services...");
        
            builder.Register<GameStateMachine>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<AssetProvider>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<SaveLoadService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<PoolService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<PoolInitializer>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<InputController>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<HealthBarService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            
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
