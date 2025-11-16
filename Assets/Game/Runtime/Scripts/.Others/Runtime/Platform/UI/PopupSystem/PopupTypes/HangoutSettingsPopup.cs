using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;
using Kumu.Kulitan.Hangout;
using UnityEngine;
using UnityEngine.UI;

namespace Kumu.Kulitan.UI
{
    public class HangoutSettingsPopup : BasePopup
    {
        [SerializeField] private Button closeButton;
        private Slot<string> eventSlot;

        public void OnPopupClose()
        {
            PopupManager.Instance.RemoveActivePopup(this);
            OnClosed?.Invoke();
            PopupManager.Instance.CloseScenePopup(SceneNames.HANGOUT_SETTINGS_POPUP);
        }

        private void SceneLoadingEventHandler(IEvent<string> obj)
        {
            OnPopupClose();
        }
        
        private void Awake()
        {
            eventSlot = new Slot<string>(GlobalNotifier.Instance);
        }

        private void OnEnable()
        {
            eventSlot.SubscribeOn(SceneLoadingEvent.EVENT_NAME, SceneLoadingEventHandler);
            closeButton.onClick.AddListener(OnPopupClose);
        }

        private void OnDisable()
        {
            closeButton.onClick.RemoveListener(OnPopupClose);
        }
    }
}
