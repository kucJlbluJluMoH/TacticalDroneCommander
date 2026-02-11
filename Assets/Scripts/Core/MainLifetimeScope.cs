using VContainer;
using VContainer.Unity;
using UnityEngine;

namespace TacticalDroneCommander.Core
{
    public class MainLifetimeScope : LifetimeScope
    {
        [SerializeField] private GameConfig _gameConfig;
        
        protected override void Configure(IContainerBuilder builder)
        {
            Debug.Log("VContainer: container has successfully started configuration...");
            
            if (_gameConfig != null)
            {
                builder.RegisterInstance(_gameConfig);
            }
            else
            {
                Debug.LogError("MainLifetimeScope: GameConfig is not assigned!");
            }
            
            var installer = new GameInstaller();
            installer.Install(builder);
            
            builder.RegisterComponentInHierarchy<GameBootstrapper>();
            builder.RegisterComponentInHierarchy<GameStateMachine>().AsImplementedInterfaces();
            
            Debug.Log("VContainer: All dependencies configured!");
        }
    }
}