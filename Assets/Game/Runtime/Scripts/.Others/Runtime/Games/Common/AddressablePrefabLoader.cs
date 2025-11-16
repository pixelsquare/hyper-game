using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Kumu.Kulitan.Common
{
    public class AddressablePrefabLoader : MonoBehaviour
    {
        [SerializeField] private string addressableLabel;
        [SerializeField] private bool loadOnStart = true;
        [SerializeField] private UnityEvent onDownloaded;

        public static bool IsLoaded => isLoaded;

        private static bool isLoaded = false;

        private static AsyncOperationHandle<IList<GameObject>> handle;

        public static void Release()
        {
            if (handle.IsValid())
                Addressables.Release(handle);
        }

        public async void LoadAddressables()
        {
            isLoaded = false;
            await LoadAddressablesAsync();
            isLoaded = true;
            onDownloaded?.Invoke();
        }

        private async Task LoadAddressablesAsync()
        {
            Release();

            handle = Addressables.LoadAssetsAsync<GameObject>(addressableLabel, OnPrefabsLoaded);
            await handle.Task;
        }

        private void OnPrefabsLoaded(GameObject prefab)
        {
            // handling for completing loading prefabs individually
            // blank on purpose: no further handling required currently
        }

        private void Start()
        {
            if (loadOnStart)
            {
                LoadAddressables();
            }
        }

        private void OnDestroy()
        {
            Release();
        }
    }
}
