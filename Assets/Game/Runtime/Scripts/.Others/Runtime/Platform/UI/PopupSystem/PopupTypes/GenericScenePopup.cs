using UnityEngine;
using UnityEngine.UI;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;
using Kumu.Kulitan.Hangout;

namespace Kumu.Kulitan.UI
{
    public class GenericScenePopup : BasePopup
    {
        [SerializeField] private string sceneName = SceneNames.SOUND_SETTINGS_POPUP;
        [SerializeField] private Button closeButton;
        private Slot<string> eventSlot;

        public void OnPopupClose()
        {
            PopupManager.Instance.RemoveActivePopup(this);
            OnClosed?.Invoke();
            PopupManager.Instance.CloseScenePopup(sceneName);
        }
        
        private void SceneLoadingEventHandler(IEvent<string> obj)
        {
            if (!SceneLoadingManager.Instance.IsSceneLoaded(SceneNames.MAIN_SCREEN))
            {
                return;
            }
            
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
            eventSlot.Dispose();
            closeButton.onClick.RemoveListener(OnPopupClose);
        }
    }
}
