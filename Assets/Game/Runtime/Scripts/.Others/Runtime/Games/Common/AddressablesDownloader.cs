using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Kumu.Kulitan.Common
{
    public class AddressablesDownloader : MonoBehaviour
    {
        [SerializeField] private string addressableLabel;
        [SerializeField] private UnityEvent<bool> onAvailable;
        [SerializeField] private UnityEvent<bool> onCacheCleared;
        [SerializeField] private UnityEvent<bool> onUpdateCatalog;
        [SerializeField] private UnityEvent onFail;
        [SerializeField] private bool downloadAtStart = true;

        private AsyncOperationHandle downloadDependenciesHandle;

        public void DownloadAssetsByLabel(string label)
        {
            addressableLabel = label;
            DownloadAssets();
        }

        public async void DownloadAssets()
        {
            onAvailable.Invoke(false);
            var success = await DownloadAssetsTask();
            if (success)
            {
                onAvailable.Invoke(true);
            }
            else
            {
                onFail.Invoke();
            }
        }

        public async void ClearAssets()
        {
            onCacheCleared.Invoke(false);
            await ClearAssetsTask();
            onCacheCleared.Invoke(true);
        }

        public async void UpdateCatalog()
        {
            onUpdateCatalog.Invoke(false);
            await UpdateCatalogTask();
            onUpdateCatalog.Invoke(true);
        }

        private async Task<bool> DownloadAssetsTask()
        {
            var loadLocationsHandle = Addressables.LoadResourceLocationsAsync(addressableLabel, typeof(GameObject));
            var locations = await loadLocationsHandle.Task;
            downloadDependenciesHandle = Addressables.DownloadDependenciesAsync(locations);
            var result = await downloadDependenciesHandle.Task;

            Addressables.Release(loadLocationsHandle);

            if (downloadDependenciesHandle.Status != AsyncOperationStatus.Succeeded)
            {
                return false;
            }
            if (result == null)
            {
                return false;
            }

            return true;
        }

        private async Task ClearAssetsTask()
        {
            var loadLocationsHandle = Addressables.LoadResourceLocationsAsync(addressableLabel, typeof(GameObject));
            var locations = await loadLocationsHandle.Task;
            Addressables.Release(downloadDependenciesHandle);
            Addressables.ClearDependencyCacheAsync(locations);
            Addressables.Release(loadLocationsHandle);
        }

        private async Task UpdateCatalogTask()
        {
            var updateCatalogHandle = Addressables.UpdateCatalogs();
            await updateCatalogHandle.Task;
        }

        private void Start()
        {
            if (downloadAtStart)
            {
                DownloadAssets();
            }
        }
    }
}
