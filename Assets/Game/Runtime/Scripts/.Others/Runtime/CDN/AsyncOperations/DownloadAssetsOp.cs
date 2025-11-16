#if ADDRESSABLES_ENABLED
using System;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Kumu.Kulitan.CDN
{
    public class DownloadAssetsOp : CDNAsyncOperation
    {
        private string assetReferenceLabel;

        public AsyncOperationHandle operationHandle;
        
        public static DownloadAssetsOp Create(string assetRefLabel)
        {
            return new DownloadAssetsOp { assetReferenceLabel = assetRefLabel };
        }

        public void Release()
        {
            Addressables.Release(operationHandle);
        }

        // prevent calling of default constructor
        public DownloadAssetsOp() { }

        #region CDNAsyncOperation

        public override bool IsFailed => operationHandle.Status == AsyncOperationStatus.Failed;
        public override object Result => operationHandle.Result;

        public override float Progress
        {
            get => operationHandle.GetDownloadStatus().Percent;
            protected set => throw new NotImplementedException(); // unused
        }

        public override bool IsDone => operationHandle.GetDownloadStatus().IsDone;

        public override void StartOp()
        {
            operationHandle = Addressables.DownloadDependenciesAsync(assetReferenceLabel);
        }
        
        #endregion
    }
}
#endif
