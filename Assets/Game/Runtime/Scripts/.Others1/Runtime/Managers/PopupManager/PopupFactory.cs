using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class PopupFactory : PlaceholderFactory<PopupType, Transform, BasePopup>, IInitializable, IDisposable
    {
        public bool IsInitialized { get; private set; }

        private readonly DiContainer _diContainer;
        private readonly Dictionary<PopupType, AssetReferenceGameObject> _popupMappingRef;

        private Dictionary<PopupType, GameObject> _popupMapping;

        public PopupFactory(DiContainer diContainer, Dictionary<PopupType, AssetReferenceGameObject> popupMappingRef)
        {
            _diContainer = diContainer;
            _popupMappingRef = popupMappingRef;
        }

        public override BasePopup Create(PopupType popupType, Transform parentTransform)
        {
            return Create<BasePopup>(popupType, parentTransform);
        }

        public T Create<T>(PopupType popupType, Transform parentTransform) where T : IPopup
        {
            if (!_popupMapping.TryGetValue(popupType, out var popupPrefab))
            {
                throw new NullReferenceException($"Popup prefab does not exist. {popupType}");
            }

            return _diContainer.InstantiatePrefabForComponent<T>(popupPrefab, parentTransform);
        }

        public bool TryCreate<T>(PopupType popupType, Transform parentTransform, out T popup) where T : IPopup
        {
            if (!_popupMapping.TryGetValue(popupType, out var popupPrefab))
            {
                popup = default;
                return false;
            }

            popup = _diContainer.InstantiatePrefabForComponent<T>(popupPrefab, parentTransform);
            return true;
        }

        public async void Initialize()
        {
            _popupMapping = new Dictionary<PopupType, GameObject>();

            foreach (var popup in _popupMappingRef)
            {
                _popupMapping[popup.Key] = await popup.Value.LoadAssetAsync<GameObject>().Task;
            }

            IsInitialized = true;
        }

        public void Dispose()
        {
            foreach (var popup in _popupMappingRef)
            {
                popup.Value.ReleaseAsset();
            }
        }
    }
}
