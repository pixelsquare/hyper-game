#if ADDRESSABLES_ENABLED
using Quantum;
using Photon.Deterministic;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;

namespace Kumu.Kulitan.CDN
{
    public class UnloadQuantumAssetsOp : CDNAsyncOperation
    {
        public static UnloadQuantumAssetsOp Create() => new();

        public override bool IsFailed => false;

        public override object Result => null;

        public override float Progress
        {
            get
            {
                var progress = 0f;
                var addressableAssetsLen = 0;

#if QUANTUM_ADDRESSABLES
                var addressableAssets = new List<QTuple<AssetRef, string>>();
                UnityDB.CollectAddressableAssets(addressableAssets);
                addressableAssetsLen = addressableAssets.Count;

                for (var i = 0; i < addressableAssetsLen; i++)
                {
                    Addressables.Release(addressableAssets[i].Item1);
                    progress = (float)i / (addressableAssetsLen - 1);
                }
#endif

                return addressableAssetsLen > 0 ? progress : 1f;
            }
        }
        
        /// <summary>
        /// Prevents the creation of this class with `new` keyword.
        /// </summary>
        private UnloadQuantumAssetsOp()
        {
        }
        
        public override void StartOp()
        {
        }
    }
}
#endif
