using System;
using System.Collections.Generic;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    public class HudManager : IHudManager
    {
        public bool IsInitialized => _hudFactory.IsInitialized;

        private readonly HudFactory _hudFactory;
        private readonly Transform _parentTransform;
        private readonly Dictionary<HudType, BaseHud> _hudMapping = new();

        public HudManager(HudFactory hudFactory, Transform parentTransform)
        {
            _hudFactory = hudFactory;
            _parentTransform = parentTransform;
        }

        public T ShowHudAsync<T>(HudType hudType) where T : BaseHud
        {
            if (_hudMapping.ContainsKey(hudType))
            {
                throw new Exception("Hud already exist.");
            }

            var hud = _hudFactory.Create<T>(hudType, _parentTransform);
            _hudMapping.Add(hudType, hud);
            return hud;
        }

        public void ShowHud(HudType hudType)
        {
            ShowHudAsync<BaseHud>(hudType);
        }

        public void HideHud(HudType hudType)
        {
            if (!_hudMapping.TryGetValue(hudType, out var hud))
            {
                throw new ArgumentNullException("Hud does not exist.", nameof(hudType));
            }

            hud.Cleanup();
            _hudMapping.Remove(hudType);
        }

        public bool HudExist(HudType hudType)
        {
            return _hudMapping.ContainsKey(hudType);
        }

        public void Cleanup()
        {
            foreach (var hud in _hudMapping)
            {
                hud.Value.Cleanup();
            }

            _hudMapping.Clear();
        }
    }
}
