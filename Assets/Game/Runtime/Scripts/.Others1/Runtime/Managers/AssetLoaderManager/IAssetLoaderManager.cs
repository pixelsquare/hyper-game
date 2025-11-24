using System;
using Cysharp.Threading.Tasks;

namespace Santelmo.Rinsurv
{
    public interface IAssetLoaderManager : IGlobalBinding
    {
        public bool IsInitialized { get; }
        public bool IsDownloading { get; }
        public bool HasNewContent { get; }
        public long DownloadSize { get; }

        public UniTaskVoid InitializeAsync();

        public UniTask<long> GetDownloadSizeAsync();

        public UniTaskVoid DownloadAssetsAsync(IProgress<float> progress = null);

        public string ResolveAssetName(string assetName);

        public void Cleanup();
    }
}
