using UnityEngine;
using UnityEngine.UI;
using Kumu.Extensions;
using TMPro;

namespace Kumu.Kulitan.CDN
{
    [RequireComponent(typeof(Button))]
    public class CDNDownloadButton : MonoBehaviour
    {
        [SerializeField] private CDNLoader loader = null;
        [SerializeField] private TMP_Text buttonText = null;

        private Button downloadButton = null;

        public void DownloadResources()
        {
#if ADDRESSABLES_ENABLED
            downloadButton.interactable = false;
            loader?.DownloadResources();
            gameObject.SetActive(false);
#endif
        }

        private void Awake()
        {
            downloadButton = GetComponent<Button>();
        }

        private void OnEnable()
        {
            CDNLoader.OnInit += HandleInit;
            CDNLoader.OnLoadStarted += HandleLoadStarted;
        }

        private void OnDisable()
        {
            CDNLoader.OnInit -= HandleInit;
            CDNLoader.OnLoadStarted -= HandleLoadStarted;
        }

        private void Start()
        {
            downloadButton.interactable = false;
            downloadButton?.onClick.AddListener(DownloadResources);
        }

        private void HandleInit()
        {
            if (loader.HasNewContent)
            {
                downloadButton.interactable = loader.DownloadSize > 0 && !loader.IsLoading && !loader.IsDownloading;
                buttonText.text = $"DOWNLOAD NOW\n{loader.DownloadSize.ToSizeString()}";
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        private void HandleLoadStarted()
        {
            downloadButton.interactable = false;
        }
    }
}
