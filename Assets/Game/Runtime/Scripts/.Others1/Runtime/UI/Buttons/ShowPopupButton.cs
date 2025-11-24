using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class ShowPopupButton : BaseButton
    {
        [SerializeField] private PopupType _popupType;
        [Inject] private IPopupManager _popupManager;

        protected override void OnButtonClicked()
        {
            _popupManager?.ShowPopup(_popupType);
        }
    }
}
