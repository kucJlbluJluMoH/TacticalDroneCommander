using VContainer;
using VContainer.Unity;
using UnityEngine;
using TacticalDroneCommander.Infrastructure;

namespace TacticalDroneCommander.Core
{
    public class GameInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            Debug.Log("GameInstaller: Registering services...");
            
            builder.Register<AssetProvider>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<SaveLoadService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<PoolService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            
            Debug.Log("GameInstaller: All services registered successfully!");
        }
    }
}
