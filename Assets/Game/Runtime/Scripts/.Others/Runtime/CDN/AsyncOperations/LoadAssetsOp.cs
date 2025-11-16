#if ADDRESSABLES_ENABLED
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Kumu.Kulitan.CDN
{
    public class LoadAssetsOp : CDNAsyncOperation
    {
        public static LoadAssetsOp Create()
        {
            return new();
        }

        public static LoadAssetsOp Create(string assetRefLabel)
        {
            return new LoadAssetsOp { assetReferenceLabel = assetRefLabel };
        }

        public override bool IsFailed =>
                loadAssetLocOps.Where(op => op.IsValid())
                                   .Any(op => op.Status == AsyncOperationStatus.Failed);

        public override object Result { get; } = null;

        public override float Progress
        {
            get
            {
                var totalProgress = loadAssetLocOps
                                   .Where(op => op.IsValid())
                                   .Sum(op => op.PercentComplete);

                var progressOffset = 0.5f;
                var operationLen = loadAssetLocOps.Count;
                var percentProgress = operationLen > 0f ? totalProgress / operationLen : 0f;
                return (percentProgress - progressOffset) / (1.0f - progressOffset);
            }
        }

        private string assetReferenceLabel;
        private readonly List<AsyncOperationHandle> loadAssetLocOps = new();

        /// <summary>
        ///     Prevents the creation of this class with `new` keyword.
        /// </summary>
        private LoadAssetsOp() { }

        public override async void StartOp()
        {
            var locations = await Addressables.LoadResourceLocationsAsync(assetReferenceLabel, typeof(object)).Task;
            var dlLocations = GetDependenciesFromLocations(locations);

            foreach (var location in dlLocations)
            {
                loadAssetLocOps.Add(Addressables.LoadAssetAsync<IAssetBundleResource>(location));
            }
        }

        private List<IResourceLocation> GetDependenciesFromLocations(IList<IResourceLocation> locations)
        {
            var locHash = new HashSet<IResourceLocation>();

            foreach (var loc in locations)
            {
                if (loc.ResourceType == typeof(IAssetBundleResource))
                {
                    locHash.Add(loc);
                }

                if (!loc.HasDependencies)
                {
                    continue;
                }

                foreach (var dep in loc.Dependencies)
                {
                    if (dep.ResourceType == typeof(IAssetBundleResource))
                    {
                        locHash.Add(dep);
                    }
                }
            }

            return new List<IResourceLocation>(locHash);
        }
    }
}
#endif
