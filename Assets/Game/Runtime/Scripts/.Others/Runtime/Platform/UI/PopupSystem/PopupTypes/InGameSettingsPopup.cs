using UnityEngine;
using UnityEngine.UI;
using Kumu.Kulitan.Common;

namespace Kumu.Kulitan.UI
{
    public class InGameSettingsPopup : BasePopup
    {
        [SerializeField] private Button closeButton;
        [SerializeField] private Toggle fpsToggle;

        private static bool toggleState = true;
        
        public void OnPopupClose()
        {
            PopupManager.Instance.RemoveActivePopup(this);
            OnClosed?.Invoke();
            PopupManager.Instance.CloseScenePopup(SceneNames.INGAME_SETTINGS_POPUP);
        }

        private void OnFPSToggled(bool value)
        {
            toggleState = value;
            GlobalNotifier.Instance.Trigger(new FPSCounterShowHideEvent(value));
        }

        private void OnEnable()
        {
            fpsToggle.isOn = toggleState;
            closeButton.onClick.AddListener(OnPopupClose);
            fpsToggle.onValueChanged.AddListener(OnFPSToggled);
        }

        private void OnDisable()
        {
            closeButton.onClick.RemoveListener(OnPopupClose);
            fpsToggle.onValueChanged.RemoveListener(OnFPSToggled);
        }
    }
}
