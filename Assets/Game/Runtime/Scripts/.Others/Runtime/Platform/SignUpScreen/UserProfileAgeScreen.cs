using System;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;
using Kumu.Kulitan.Tracking;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Kumu.Kulitan.UI
{
    public class UserProfileAgeScreen : MonoBehaviour
    {
        public static int userProfileAge;

        [SerializeField] private Button btnNext;
        [SerializeField] private UserProfileSelection[] selections;
        [SerializeField] private UnityEvent onAgeProfileSubmitted;
        
        private Slot<string> eventSlot;

        private void OnUserProfileButtonClicked(IEvent<string> callback)
        {
            var eventCallback = (UserProfileSelectionClickedEvent)callback;
            ActivateButton(eventCallback.Selection);
        }

        private void OnNextButtonClicked()
        {
            onAgeProfileSubmitted?.Invoke();
        }

        private void ActivateButton(UserProfileSelection selectionToActivate)
        {
            foreach (var selection in selections)
            {
                selection.ToggleSelect(selectionToActivate == selection);
            }

            userProfileAge = Array.IndexOf(selections, selectionToActivate);
        }

        private void OnEnable()
        {
            btnNext.onClick.AddListener(OnNextButtonClicked);
            eventSlot.SubscribeOn(UserProfileSelectionClickedEvent.EVENT_NAME, OnUserProfileButtonClicked);
            GlobalNotifier.Instance.Trigger(new UserJourneyEvent(UserJourneyEvent.Checkpoint.Age));
        }

        private void OnDisable()
        {
            btnNext.onClick.RemoveListener(OnNextButtonClicked);
            eventSlot.UnSubscribeFor(UserProfileSelectionClickedEvent.EVENT_NAME, OnUserProfileButtonClicked);
        }

        private void Awake()
        {
            eventSlot = new Slot<string>(GlobalNotifier.Instance);
            ActivateButton(selections[0]);
        }

        private void OnDestroy()
        {
            eventSlot.Dispose();
        }
    }
}
