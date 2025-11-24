using System;
using System.Collections.Generic;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    public class PopupManager : IPopupManager
    {
        private readonly PopupFactory _popupFactory;
        private readonly Transform _parentTransform;
        private readonly GameObject _popupOverlay;

        private readonly List<(PopupType, IPopup)> _popupStack = new();

        public PopupManager(PopupFactory popupFactory, GameObject popupOverlay, Transform parentTransform)
        {
            _popupFactory = popupFactory;
            _popupOverlay = popupOverlay;
            _parentTransform = parentTransform;
        }

        public T ShowPopupAsync<T>(PopupType popupType) where T : IPopup
        {
            try
            {
                if (!_popupFactory.TryCreate<T>(popupType, _parentTransform, out var popup))
                {
                    throw new Exception("Popup type does not exist.");
                }

                _popupStack.Insert(0, new ValueTuple<PopupType, IPopup>(popupType, popup));
                SetPopupOverlayDirty();
                return popup;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        public void ShowPopup(PopupType popupType)
        {
            var popup = _popupFactory.Create(popupType, _parentTransform);
            _popupStack.Insert(0, new ValueTuple<PopupType, IPopup>(popupType, popup));
            SetPopupOverlayDirty();
        }

        public void ClosePopup(PopupType popupType)
        {
            var popupIdx = _popupStack.FindIndex(x => x.Item1 == popupType);

            if (popupIdx < 0)
            {
                throw new ArgumentException("Popup does not exist.", nameof(popupType));
            }

            _popupStack[popupIdx].Item2.OnPopupClose();
            _popupStack.RemoveAt(popupIdx);

            SetPopupOverlayDirty();
        }

        public void CloseLastPopup()
        {
            if (_popupStack.Count <= 0)
            {
                return;
            }

            _popupStack[0].Item2.Cleanup();
            _popupStack.RemoveAt(0);

            SetPopupOverlayDirty();
        }

        public void Cleanup()
        {
            foreach (var popup in _popupStack)
            {
                popup.Item2.Cleanup();
            }

            _popupStack.Clear();
            SetPopupOverlayDirty();
        }

        private void SetPopupOverlayDirty()
        {
            var isActive = _popupStack.Count > 0;
            _popupOverlay.SetActive(isActive);
        }
    }
}
