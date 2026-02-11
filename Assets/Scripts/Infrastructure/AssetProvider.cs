using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TacticalDroneCommander.Infrastructure
{
    public interface IAssetProvider
    {
        UniTask<T> Load<T>(string address) where T : Object;
        void Release(string address);
    }
    
    public class AssetProvider : IAssetProvider
    {
        public async UniTask<T> Load<T>(string address) where T : Object
        {
            //todo Implement Addressables loading
            await UniTask.Yield();
            var asset = Resources.Load<T>(address);
            
            if (asset == null)
            {
                Debug.LogError($"AssetProvider: Failed to load asset at {address}");
            }
            
            return asset;
        }

        public void Release(string address)
        {
            //todo Implement Addressables release
            Debug.Log($"AssetProvider: Released asset {address}");
        }
    }
}

