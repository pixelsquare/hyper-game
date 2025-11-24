using System.Text;
using Kumu.Extensions;
using UniRx;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class AssetLoaderProgress : MonoBehaviour
    {
        [SerializeField] private RinawaText _downloadProgressText;
        [SerializeField] private RinawaSlider _downloadProgressSlider;

        [SerializeField] private string _downloadProgressFormat = "Downloading Assets ({0}/{1})";

        [Inject] private IAssetLoaderManager _assetLoaderManager;

        private readonly ReactiveProperty<float> _downloadProgressProp = new(0.0f);

        public void HandleDownloadProgress(float progress)
        {
            _downloadProgressProp.Value = progress;
        }

        private void OnEnable()
        {
            var sb = new StringBuilder();

            _downloadProgressProp.Subscribe(x =>
            {
                _downloadProgressSlider.value = x;

                var maxSize = _assetLoaderManager.DownloadSize;
                var curSize = (long)(x * maxSize);

                sb.Clear();
                sb.AppendFormat(_downloadProgressFormat, curSize.ToSizeString(), maxSize.ToSizeString());

                _downloadProgressText.text = sb.ToString();
            });
        }

        private void OnDestroy()
        {
            _downloadProgressProp.Dispose();
        }
    }
}
