using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Santelmo.Rinsurv
{
    using SceneName = GameConstants.SceneNames;
    using AppStateEvent = GameEvents.AppState;

    public class AssetLoaderState : State
    {
        [Inject] private ISceneManager _sceneManager;
        [Inject] private LazyInject<IAssetLoaderManager> _assetLoaderManager;

        public override async void OnEnter()
        {
            Dispatcher.AddListener(AppStateEvent.ToLoginScreenEvent, HandleLoginScreenEvent);
            await _sceneManager.LoadSceneAdditiveAsync(SceneName.AssetLoaderScene).Task;
            _assetLoaderManager.Value.InitializeAsync();
        }

        public override void OnExit()
        {
            _assetLoaderManager.Value.Cleanup();
            Object.Destroy(EventSystem.current.gameObject);
            Dispatcher.RemoveListener(AppStateEvent.ToLoginScreenEvent, HandleLoginScreenEvent, true);
        }

        private void HandleLoginScreenEvent(IMessage message)
        {
            EndState();
        }
    }
}
