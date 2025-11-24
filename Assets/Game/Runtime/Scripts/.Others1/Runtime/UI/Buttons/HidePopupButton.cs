using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class HidePopupButton : BaseButton
    {
        [SerializeField] private PopupType _popupType;
        [Inject] private IPopupManager _popupManager;

        protected override void OnButtonClicked()
        {
            _popupManager?.ClosePopup(_popupType);
        }
    }
}
