using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Santelmo.Rinsurv
{
    public class AssetLoaderManager : IAssetLoaderManager
    {
        public bool IsInitialized { get; private set; }
        public bool IsDownloading { get; private set; }
        public long DownloadSize { get; private set; }
        public bool HasNewContent => DownloadSize > 0 || _newCatalogs.Count > 0;

        private CancellationTokenSource cts;

        private IResourceLocator _resourceLocator;
        private List<string> _newCatalogs = new();
        private List<string> _assetNames = new();
        private IList<IResourceLocator> _resourceLocators;
        private IList<IResourceLocation> _resourceLocations;

        public async UniTaskVoid InitializeAsync()
        {
            if (IsInitialized)
            {
                Debug.LogError("Asset loader is already initialized!");
                return;
            }

            try
            {
                using (cts = new CancellationTokenSource())
                {
                    _resourceLocator = await Addressables.InitializeAsync().ToUniTask(cancellationToken: cts.Token);
                }

                _assetNames.AddRange(_resourceLocator.Keys.Select(x => x.ToString()).Where(x => Path.HasExtension(x)));
                _newCatalogs = await GetCatalogUpdatesAsync();
                _resourceLocators = await UpdateCatalogAsync();
                DownloadSize = await GetDownloadSizeAsync();
                IsInitialized = true;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        public async UniTask<long> GetDownloadSizeAsync()
        {
            if (_resourceLocator == null)
            {
                throw new Exception("Asset loader is not initialized!");
            }

            try
            {
                using (cts = new CancellationTokenSource())
                {
                    DownloadSize = await Addressables.GetDownloadSizeAsync(_resourceLocator.Keys).ToUniTask(cancellationToken: cts.Token);
                    return DownloadSize;
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        public async UniTaskVoid DownloadAssetsAsync(IProgress<float> progress = null)
        {
            if (IsDownloading)
            {
                throw new Exception("Asset loader already downloading!");
            }

            try
            {
                IsDownloading = true;
                _newCatalogs = await GetCatalogUpdatesAsync();
                _resourceLocators = await UpdateCatalogAsync();
                await DownloadDependencyLocationsAsync(null, progress);
                IsDownloading = false;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
            finally
            {
                progress?.Report(1.0f);
            }
        }

        public string ResolveAssetName(string assetName)
        {
            return _assetNames.First(x => x.Contains(assetName));
        }

        public void Cleanup()
        {
            cts?.Dispose();
        }

        private async UniTask<List<string>> GetCatalogUpdatesAsync()
        {
            try
            {
                using (cts = new CancellationTokenSource())
                {
                    _newCatalogs = await Addressables.CheckForCatalogUpdates().ToUniTask(cancellationToken: cts.Token);
                    return _newCatalogs;
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        private async UniTask<IList<IResourceLocator>> UpdateCatalogAsync()
        {
            if (_newCatalogs.Count <= 0)
            {
                return null;
            }

            try
            {
                using (cts = new CancellationTokenSource())
                {
                    _resourceLocators = await Addressables.UpdateCatalogs().ToUniTask(cancellationToken: cts.Token);
                    _assetNames.AddRange(_resourceLocators.Select(x => x.Keys).Select(x => x.ToString()).Where(x => Path.HasExtension(x)).ToList());
                    return _resourceLocators;
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        private async UniTask DownloadDependencyLocationsAsync(string key, IProgress<float> progress = null)
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                {
                    // Gets all bundle location without the need for a key or label.
                    var bundleLocs = await GetAllBundleLocationAsync();
                    _resourceLocations = new List<IResourceLocation>(bundleLocs);
                }
                else
                {
                    _resourceLocations = await GetResourceLocationsAsync(key);
                }


                AssetLoaderUtil.GetDependenciesFromLocations(_resourceLocations, out var dependencies);

                var curProgress = 0.0f;
                var totalProgress = 0.03f;
                var dependencyLen = dependencies.Count;
                var tasks = new List<UniTask>();

                foreach (var dependency in dependencies)
                {
                    var taskProgressValue = 0f;
                    var taskProgress = Progress.Create<float>(x =>
                    {
                        var progressDelta = x - taskProgressValue;
                        taskProgressValue = x;
                        totalProgress += progressDelta;
                        curProgress = Mathf.Min(totalProgress / dependencyLen, 1.0f);
                        progress?.Report(curProgress);
                    });

                    using (cts = new CancellationTokenSource())
                    {
                        tasks.Add(Addressables.LoadAssetAsync<IAssetBundleResource>(dependency)
                                              .ToUniTask(taskProgress, cancellationToken: cts.Token));
                    }
                }

                await UniTask.WhenAll(tasks.ToArray());
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        private async UniTask<IList<IResourceLocation>> GetResourceLocationsAsync(string key)
        {
            try
            {
                using (cts = new CancellationTokenSource())
                {
                    _resourceLocations = await Addressables.LoadResourceLocationsAsync(key, typeof(object)).ToUniTask(cancellationToken: cts.Token);
                    return _resourceLocations;
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        private async UniTask<List<IResourceLocation>> GetAllBundleLocationAsync()
        {
            try
            {
                var bundleLocs = new List<IResourceLocation>();

                foreach (var resource in _resourceLocator.Keys)
                {
                    var resourceStr = resource.ToString();
                    var resourceExt = Path.GetExtension(resourceStr);

                    if (string.IsNullOrEmpty(resourceStr) || string.IsNullOrEmpty(resourceExt) || !resourceExt.Equals(".bundle"))
                    {
                        continue;
                    }

                    var resourceLoc = await GetResourceLocationsAsync(resourceStr);
                    bundleLocs.AddRange(resourceLoc);
                }

                return bundleLocs;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }
    }
}
