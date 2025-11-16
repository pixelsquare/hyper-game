#if ADDRESSABLES_ENABLED
using System.Collections.Generic;
using System.Linq;
using Photon.Deterministic;
using Quantum;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Kumu.Kulitan.CDN
{
    public class LoadConfigAssetsOp : CDNAsyncOperation
    {
        public static LoadConfigAssetsOp Create()
        {
            return new();
        }

        public override bool IsFailed =>
                loadAssetAsyncOps.Where(op => op.IsValid())
                                 .Any(op => op.Status == AsyncOperationStatus.Failed);

        public override object Result => loadAssetAsyncOps;

        public override float Progress
        {
            get
            {
                var doneOps = loadAssetAsyncOps.FindAll(a => a.IsDone).Count;
                var loadAssetsAsyncOpLen = loadAssetAsyncOps.Count;
                var progress = loadAssetsAsyncOpLen > 0 ? doneOps / (float)loadAssetsAsyncOpLen : 1f;

                if (progress >= 1f)
                {
                    loadedAssets = new List<AssetBase>();
                    loadAssetAsyncOps.ForEach(a => loadedAssets.Add((AssetBase)a.Result));
                }

                return progress;
            }
        }

        private List<AssetBase> loadedAssets;
        private List<AsyncOperationHandle> loadAssetAsyncOps;

        /// <summary>
        ///     Prevents the creation of this class with `new` keyword.
        /// </summary>
        private LoadConfigAssetsOp() { }

        public override void StartOp()
        {
#if QUANTUM_ADDRESSABLES
            // there's also an overload that accepts a target list parameter
            var addressableAssets = new List<QTuple<AssetRef, string>>();
            UnityDB.CollectAddressableAssets(addressableAssets);

            addressableAssets.RemoveAll(a => !a.Item1.Contains("QuantumAssets/Config"));

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
