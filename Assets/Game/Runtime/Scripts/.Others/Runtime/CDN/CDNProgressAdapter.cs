using UnityEngine;
using TMPro;

namespace Kumu.Kulitan.CDN
{
    public class CDNProgressAdapter : MonoBehaviour
    {
        [SerializeField] private CDNLoader loader = null;
        [SerializeField] private GameObject loadingProgressBar = null;
        [SerializeField] private TMP_Text loadingProgressText = null;
        [SerializeField] private TMP_Text loadingMessageText = null;
        [SerializeField] private SlicedFilledImage loadingBarFill = null;

        public void SetProgressBarActive(bool active)
        {
            loadingProgressBar.SetActive(active);
        }

        private void OnEnable()
        {
            CDNLoader.OnInit += HandleInit;
            CDNLoader.OnLoadStarted += HandleLoadStarted;
            CDNLoader.OnLoadProgress += HandleLoadProgress;
            CDNLoader.OnDownloadStarted += HandleDownloadStarted;
            CDNLoader.OnDownloadProgress += HandleDownloadProgress;
            CDNLoader.OnLoadMessage += HandleLoadMessage;
            
        }

        private void OnDisable()
        {
            CDNLoader.OnInit -= HandleInit;
            CDNLoader.OnLoadStarted -= HandleLoadStarted;
            CDNLoader.OnLoadProgress -= HandleLoadProgress;
            CDNLoader.OnDownloadStarted -= HandleDownloadStarted;
            CDNLoader.OnDownloadProgress -= HandleDownloadProgress;
            CDNLoader.OnLoadMessage -= HandleLoadMessage;
        }

        private void Start()
        {
            SetProgressBarActive(false);
        }

        private void HandleInit()
        {
            if (!loader.HasNewContent)
            {
                SetProgressBarActive(true);
            }
        }

        private void HandleLoadStarted()
        {
            if (loader.HasNewContent)
            {
                SetProgressBarActive(true);
            }
        }

        private void HandleLoadProgress(float progress)
        {
            loadingBarFill.fillAmount = progress;
            loadingProgressText.text = $"{progress * 100f:#,0}%";
        }

        private void HandleDownloadStarted()
        {
            if (loader.HasNewContent)
            {
                SetProgressBarActive(true);
            }
        }

        private void HandleDownloadProgress(float progress)
        {
            loadingBarFill.fillAmount = progress;
            loadingProgressText.text = $"{progress * 100f:#,0}%";
        }

        private void HandleLoadMessage(string message)
        {
            loadingMessageText.text = message;
        }
    }
}
