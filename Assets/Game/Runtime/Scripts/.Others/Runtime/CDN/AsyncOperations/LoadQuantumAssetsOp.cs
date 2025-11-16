#if ADDRESSABLES_ENABLED
using System.Collections.Generic;
using System.Linq;
using Photon.Deterministic;
using Quantum;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Kumu.Kulitan.CDN
{
    public class LoadQuantumAssetsOp : CDNAsyncOperation
    {
        public static LoadQuantumAssetsOp Create()
        {
            return new LoadQuantumAssetsOp();
        }

        public override bool IsFailed =>
                loadAssetAsyncOps.Where(op => op.IsValid())
                                 .Any(op => op.Status == AsyncOperationStatus.Failed);

        public override object Result => loadAssetAsyncOps;

        public override float Progress
        {
            get
            {
                var totalProgress = loadAssetAsyncOps
                                   .Where(op => op.IsValid())
                                   .Sum(op => op.PercentComplete);

                var operationLen = loadAssetAsyncOps.Count;
                var percentProgress = operationLen > 0f ? totalProgress / operationLen : 0f;

                if (percentProgress >= 1f)
                {
                    loadedAssets = new List<AssetBase>();
                    loadAssetAsyncOps.ForEach(a => loadedAssets.Add((AssetBase)a.Result));
                }

                return percentProgress;
            }
        }

        private List<AssetBase> loadedAssets;
        private List<AsyncOperationHandle> loadAssetAsyncOps;

        /// <summary>
        ///     Prevents the creation of this class with `new` keyword.
        /// </summary>
        private LoadQuantumAssetsOp() { }

        public override void StartOp()
        {
#if QUANTUM_ADDRESSABLES
            // there's also an overload that accepts a target list paramter
            var addressableAssets = new List<QTuple<AssetRef, string>>();
            UnityDB.CollectAddressableAssets(addressableAssets);

            var addressableAssetsLen = addressableAssets.Count;
            loadAssetAsyncOps = new List<AsyncOperationHandle>();

            // preload all the addressable assets
            for (var i = 0; i < addressableAssetsLen; i++)
            {
                // there are a few ways to load an asset with Addressables (by label, by IResourceLocation, by address etc.)
                // but it seems that they're not fully interchangeable, i.e. loading by label will not make loading by address
                // be reported as done immediately; hence the only way to preload an asset for Quantum is to replicate
                // what it does internally, i.e. load with the very same parameters
                loadAssetAsyncOps.Add(Addressables.LoadAssetAsync<AssetBase>(addressableAssets[i].Item1));
            }
#else
            Progress = 1f;
#endif
        }
    }
}
#endif
