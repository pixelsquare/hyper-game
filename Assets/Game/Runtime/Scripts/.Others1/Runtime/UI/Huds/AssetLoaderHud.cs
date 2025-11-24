using System;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    using AppStateEvent = GameEvents.AppState;
    using UIEvent = GameEvents.UserInterface;

    public class AssetLoaderHud : BaseHud
    {
        [SerializeField] private GameObject _startPanel;
        [SerializeField] private GameObject _downloadPanel;

        [SerializeField] private AssetLoaderProgress _assetLoaderProgress;

        [Inject] private IAssetLoaderManager _assetLoaderManager;

        private CompositeDisposable _compositeDisposable = new();

        private readonly ReactiveProperty<int> _panelActiveProp = new(1);

        private void InvokeLoginStateEvent()
        {
            Dispatcher.SendMessage(AppStateEvent.ToLoginScreenEvent);
        }

        private async void HandleStartDownloadEvent(IMessage message)
        {
            try
            {
                await UniTask.WaitUntil(() => _assetLoaderManager.IsInitialized);

                var hasNewContents = _assetLoaderManager.HasNewContent;

                if (!hasNewContents)
                {
                    InvokeLoginStateEvent();
                    return;
                }

                _panelActiveProp.Value = 2;
                var progress = Progress.Create<float>(_assetLoaderProgress.HandleDownloadProgress);
                _assetLoaderManager.DownloadAssetsAsync(progress);

                await UniTask.RunOnThreadPool(async () =>
                {
                    await UniTask.WaitUntil(() => !_assetLoaderManager.IsDownloading);
                    await UniTask.NextFrame();
                    InvokeLoginStateEvent();
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void OnEnable()
        {
            Dispatcher.AddListener(UIEvent.OnStartDownloadAssets, HandleStartDownloadEvent);

            _panelActiveProp.Subscribe(x =>
            {
                _startPanel.SetActive(x == 1);
                _downloadPanel.SetActive(x == 2);
            }).AddTo(_compositeDisposable);
        }

        private void OnDisable()
        {
            Dispatcher.RemoveListener(UIEvent.OnStartDownloadAssets, HandleStartDownloadEvent, true);
            _compositeDisposable.Clear();
        }

        private void OnDestroy()
        {
            _compositeDisposable.Dispose();
            _compositeDisposable = null;
        }
    }
}
