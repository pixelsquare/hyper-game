using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;
using Kumu.Kulitan.UI;
using UnityEngine;

namespace Kumu.Kulitan.Avatar
{
    public class AvatarShopCartChecker : MonoBehaviour
    {
        [Header("Popup Settings")] 
        [SerializeField] private string popupTitle = "Exit shop?";
        [SerializeField] private string popupMessage = "You have unequipped items. Are you sure you want to exit the shop?";

        [Header("References")]
        [SerializeField] private UINavigationController navController;
        [SerializeField] private UINavigationButton navButton;

        [Header("Config")]
        [SerializeField] private string resetSelectionStartEvt = "ResetSelectionStart";
        [SerializeField] private string exitShopEvt = "ExitShop";
        
        private ConfirmationPopup popup;
        
        private Slot<string> eventSlot;
        
        /// <summary>
        /// Must be attached to any NavBar button except on AvatarCustomization button.
        /// </summary>
        /// <param name="navButton"></param>
        public void OnNavPanelActivated()
        {
            eventSlot.SubscribeOn(ExitShopEvent.EVENT_NAME, OnPromptExit);            
            GlobalNotifier.Instance.Trigger(new FSMSendUnityEvent(exitShopEvt));
        }

        private void OnPromptExit(IEvent<string> rawEventData)
        {
            var eventData = (ExitShopEvent)rawEventData;
            eventSlot.UnSubscribeFor(ExitShopEvent.EVENT_NAME, OnPromptExit);
            
            if (eventData.ExitMode == ExitShopEvent.Mode.Confirmation)
            {
                popup = (ConfirmationPopup)PopupManager.Instance.OpenConfirmationPopup(popupTitle,
                    popupMessage, "Yes", "No");
                popup.OnConfirm += () =>
                {
                    GlobalNotifier.Instance.Trigger(new FSMSendUnityEvent(resetSelectionStartEvt));
                    navController.NavPanelActivated(navButton);
                };
            }
            else if (eventData.ExitMode == ExitShopEvent.Mode.Immediate)
            {
                navController.NavPanelActivated(navButton);
            }
            
        }

        #region MonoBehaviour events
        private void Awake()
        {
            eventSlot = new Slot<string>(GlobalNotifier.Instance);
        }

        private void OnDestroy()
        {
            eventSlot.Dispose();
        }

        #endregion
    }

    public class ExitShopEvent : Event<string>
    {
        public const string EVENT_NAME = "ExitShopEvent";

        public enum Mode
        {
            Immediate,
            Confirmation,
        }

        private Mode mode;

        public ExitShopEvent(Mode mode) : base(EVENT_NAME)
        {
            this.mode = mode;
        }
        
        public Mode ExitMode => mode;
    }
}
