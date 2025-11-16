using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;

namespace Kumu.Kulitan.UI
{
    public class MainSettingsPopup : BasePopup
    {
        [SerializeField] private Button feedbackFormButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button signOutButton;

        [Header("Event Handlers")] 
        [SerializeField] private UnityEvent onFeedbackFormBtnClicked;
        [SerializeField] private UnityEvent onSignOutBtnClicked;

        public void OnFeedbackFormClicked()
        {
            onFeedbackFormBtnClicked?.Invoke();
        }

        public void OnPopupClose()
        {
            PopupManager.Instance.RemoveActivePopup(this);
            OnClosed?.Invoke();
            PopupManager.Instance.CloseScenePopup(SceneNames.MAIN_SETTINGS_POPUP);
        }
        
        private void OnSignOutButtonClicked()
        {
            onSignOutBtnClicked?.Invoke();
        }

        private void OnLoadingScreenStateEvent(IEvent<string> callback)
        {
            var eventData = (LoadingScreenStateEvent)callback;

            if (eventData.State == LoadingScreenStateEvent.LoadingScreenState.SHOWN)
            {
                Close();
            }
        }

        private void OnEnable()
        {
            closeButton.onClick.AddListener(OnPopupClose);
            feedbackFormButton.onClick.AddListener(OnFeedbackFormClicked);
            signOutButton.onClick.AddListener(OnSignOutButtonClicked);
            GlobalNotifier.Instance.SubscribeOn(LoadingScreenStateEvent.EVENT_ID, OnLoadingScreenStateEvent);
        }

        private void OnDisable()
        {
            closeButton.onClick.RemoveListener(OnPopupClose);
            feedbackFormButton.onClick.RemoveListener(OnFeedbackFormClicked);
            signOutButton.onClick.RemoveListener(OnSignOutButtonClicked);
            GlobalNotifier.Instance.UnSubscribeFor(LoadingScreenStateEvent.EVENT_ID, OnLoadingScreenStateEvent);
        }
    }
}
