using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using TacticalDroneCommander.Core;

namespace TacticalDroneCommander.Infrastructure
{
    public interface IPoolInitializer
    {
        UniTask InitializeAllPools();
    }

    public class PoolInitializer : IPoolInitializer
    {
        private readonly IPoolService _poolService;
        private readonly IAssetProvider _assetProvider;
        private readonly GameConfig _config;
        
        public PoolInitializer(
            IPoolService poolService,
            IAssetProvider assetProvider,
            GameConfig config)
        {
            _poolService = poolService;
            _assetProvider = assetProvider;
            _config = config;
        }

        public async UniTask InitializeAllPools()
        {
            Debug.Log("PoolInitializer: Starting pool initialization...");

            var loadTasks = _config.PoolConfigs
                .Select(config => _assetProvider.Load<GameObject>(config.PrefabPath))
                .ToArray();

            var prefabs = await UniTask.WhenAll((IEnumerable<UniTask<GameObject>>)loadTasks);
        
            for (int i = 0; i < _config.PoolConfigs.Length; i++)
            {
                var config = _config.PoolConfigs[i];
                var prefab = prefabs[i];

                if (prefab != null)
                {
                    _poolService.CreatePool(config.Key, prefab, config.InitialSize);
                    Debug.Log($"PoolInitializer: {config.Key} pool created ({config.InitialSize} objects)");
                }
                else
                {
                    Debug.LogError($"PoolInitializer: Failed to load {config.Key} prefab at {config.PrefabPath}!");
                }
            }

            Debug.Log("PoolInitializer: All pools initialized successfully!");
        }

    }
}