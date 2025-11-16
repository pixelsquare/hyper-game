using Kumu.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Kumu.Kulitan.Avatar
{
    public static class AvatarAddressablesUtility
    {
        private static readonly Dictionary<string, Task> taskTable = new Dictionary<string, Task>();

        public static async Task<T> LoadAddressable<T>(AssetReference assetRef) where T : Object
        {
            var loadLocationsHandle = Addressables.LoadResourceLocationsAsync(assetRef);
            var locations = await loadLocationsHandle.Task;
            if (locations.Count < 1)
            {
                $"{typeof(T)} not found for Addressable {assetRef.ToString().WrapColor(Color.magenta)}. Check the asset.".LogError();
                Addressables.Release(loadLocationsHandle);
                return null;
            }
            if (assetRef.Asset != null)
            {
                Addressables.Release(loadLocationsHandle);
                return (T)assetRef.Asset;
            }

            var hasRunningTask = taskTable.TryGetValue(assetRef.RuntimeKey.ToString(), out var task);
            if (hasRunningTask)
            {
                Addressables.Release(loadLocationsHandle);
                return await (Task<T>)task;
            }
            else
            {
                task = assetRef.LoadAssetAsync<T>().Task;
                taskTable.Add(assetRef.RuntimeKey.ToString(), task);
                var value = await (Task<T>)task;
                taskTable.Remove(assetRef.RuntimeKey.ToString());
                Addressables.Release(loadLocationsHandle);
                return value;
            }
        }

        public static async Task<T> LoadAddressable<T>(string addressablePath) where T : Object
        {
            var loadResourceHandle = Addressables.LoadResourceLocationsAsync(addressablePath, typeof(T));
            var locations = await loadResourceHandle.Task;

            if (locations.Count < 1)
            {
                $"{typeof(T)} not found for Addressable {addressablePath.WrapColor(Color.magenta)}. Check the asset.".LogError();
                Addressables.Release(loadResourceHandle);
                return null;
            }

            var hasRunningTask = taskTable.TryGetValue(addressablePath, out var task);
            if (hasRunningTask)
            {
                Addressables.Release(loadResourceHandle);
                return await (Task<T>)task;
            }
            else
            {
                var locationPath = locations[0];
                var handle = Addressables.LoadAssetAsync<T>(locationPath);
                task = handle.Task;
                taskTable.Add(addressablePath, task);
                var value = await (Task<T>)task;
                taskTable.Remove(addressablePath);
                Addressables.Release(loadResourceHandle);
                return value;
            }
        }

        public static async Task Release<T>(string addressablePath)
        {
            var loadResourceHandle = Addressables.LoadResourceLocationsAsync(addressablePath, typeof(T));
            var locations = await loadResourceHandle.Task;

            if (locations.Count < 1)
            {
                $"{typeof(T)} not found for Addressable {addressablePath.WrapColor(Color.magenta)}. Check the asset.".LogError();
            }
            else
            {
                var locationPath = locations[0];
                Addressables.Release(locationPath);
            }

            Addressables.Release(loadResourceHandle);
        }

        public static async Task Release<T>(AssetReference assetRef)
        {
            var loadResourceHandle = Addressables.LoadResourceLocationsAsync(assetRef);
            var locations = await loadResourceHandle.Task;

            if (locations.Count < 1)
            {
                $"{typeof(T)} not found for Addressable {assetRef.ToString().WrapColor(Color.magenta)}. Check the asset.".LogError();

            }
            else if (assetRef.Asset == null)
            {
                $"{typeof(T)} {assetRef.ToString().WrapColor(Color.yellow)} is already released.".LogWarning();
            }
            else
            {
                assetRef.ReleaseAsset();
            }

            Addressables.Release(loadResourceHandle);
        }
    }
}
