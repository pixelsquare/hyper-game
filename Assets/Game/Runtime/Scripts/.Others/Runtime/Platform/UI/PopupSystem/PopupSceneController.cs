using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;
using UnityEngine;

namespace Kumu.Kulitan.UI
{
    public class PopupSceneController : MonoBehaviour
    {
        [SerializeField] private string popupSceneName;
        [SerializeField] private bool openOnlyOnce = false;

        private bool isOpened;
        private Slot<string> eventSlot;
        public string PopupSceneName => popupSceneName;
        
        public void OpenPopupScene()
        {
            OpenPopupScene(popupSceneName);
        }

        public void OpenPopupScene(string sceneName)
        {
            if (openOnlyOnce && isOpened)
            {
                return;
            }

            popupSceneName = sceneName;
            PopupManager.Instance.OpenScenePopup(sceneName);
            isOpened = true;
        }

        private void OnPopupClosed(IEvent<string> callback)
        {
            var popupClosedCallback = (OnScenePopupClosedEvent)callback;
            if (!popupClosedCallback.ScenePopupName.Equals(popupSceneName))
            {
                return;
            }

            isOpened = false;
        }

        private void OnEnable()
        {
            eventSlot.SubscribeOn(OnScenePopupClosedEvent.EVENT_NAME, OnPopupClosed);
        }

        private void OnDisable()
        {
            eventSlot.Dispose();
        }

        private void Awake()
        {
            eventSlot = new Slot<string>(GlobalNotifier.Instance);
        }
    }
}
