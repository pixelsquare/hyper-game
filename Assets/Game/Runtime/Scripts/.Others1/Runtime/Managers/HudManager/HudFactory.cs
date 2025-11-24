using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class HudFactory : PlaceholderFactory<HudType, Transform, BaseHud>, IInitializable, IDisposable
    {
        public bool IsInitialized { get; private set; }

        private readonly DiContainer _diContainer;
        private readonly Dictionary<HudType, AssetReferenceGameObject> _hudMappingRef;

        private Dictionary<HudType, GameObject> _hudMapping;

        public HudFactory(DiContainer diContainer, Dictionary<HudType, AssetReferenceGameObject> hudMappingRef)
        {
            _diContainer = diContainer;
            _hudMappingRef = hudMappingRef;
        }

        public override BaseHud Create(HudType hudType, Transform parentTransform)
        {
            return Create<BaseHud>(hudType, parentTransform);
        }

        public T Create<T>(HudType hudType, Transform parentTransform) where T : BaseHud
        {
            if (!_hudMapping.TryGetValue(hudType, out var hudPrefab))
            {
                throw new NullReferenceException($"Hud prefab does not exist. [{hudType}]");
            }

            return _diContainer.InstantiatePrefabForComponent<T>(hudPrefab, parentTransform);
        }

        public bool TryCreate<T>(HudType hudType, Transform parentTransform, out T hud) where T : BaseHud
        {
            if (!_hudMapping.TryGetValue(hudType, out var hudPrefab))
            {
                hud = default;
                return false;
            }

            hud = _diContainer.InstantiatePrefabForComponent<T>(hudPrefab, parentTransform);
            return true;
        }

        public async void Initialize()
        {
            if (IsInitialized)
            {
                return;
            }

            _hudMapping = new Dictionary<HudType, GameObject>();

            foreach (var hud in _hudMappingRef)
            {
                _hudMapping[hud.Key] = await hud.Value.LoadAssetAsync<GameObject>().Task;
            }

            IsInitialized = true;
        }

        public void Dispose()
        {
            foreach (var hud in _hudMappingRef)
            {
                hud.Value.ReleaseAsset();
            }
        }
    }
}
