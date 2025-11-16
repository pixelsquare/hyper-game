using Kumu.Kulitan.Backend;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;
using Quantum;
using UnityEngine;

namespace Kumu.Kulitan.Tracking.Components
{
    public class ExitHangoutEventTrigger : MonoBehaviour
    {
        private HangoutEventTriggerSharedPropertiesTracker sharedPropsTracker;

        private int textChatCount;
        private float voiceChatDuration;
        private int interactiveObjectsCount;
        private int emotesSoloCount;
        private int emotesPairedCount;
        
        private bool IsTracking { get; set; }
        
        private bool IsMuted { get; set; }

        private void Init()
        {
            textChatCount = 0;
            voiceChatDuration = 0;
            interactiveObjectsCount = 0;
            emotesSoloCount = 0;
            emotesPairedCount = 0;
            sharedPropsTracker = new HangoutEventTriggerSharedPropertiesTracker();
        }

        private ExitHangoutDetails BuildExitHangoutDetails(HangoutEventTriggerSharedPropertiesTracker.HangoutSharedPropertiesDetails details)
        {
            return new ExitHangoutDetails(
                details.hangoutId,
                details.sessionId,
                details.roomDuration,
                details.diamondsReceived,
                details.followersGained,
                textChatCount,
                voiceChatDuration,
                interactiveObjectsCount,
                emotesSoloCount,
                emotesPairedCount
            );
        }

        private void UpdateVoiceDuration()
        {
            if (IsMuted)
            {
                return;
            }

            voiceChatDuration += Time.deltaTime;
        }

        #region Event handlers

        private void OnJoinHangoutEvent(IEvent<string> obj)
        {
            IsTracking = true;
            Init();
        }

        private void OnLeaveHangoutEvent(IEvent<string> obj)
        {
            if (!IsTracking)
            {
                return;
            }
            
            IsTracking = false;

            sharedPropsTracker.EndTrackingAsync(details =>
            {
                GlobalNotifier.Instance.Trigger(new ExitHangoutEvent(UserProfileLocalDataManager.Instance.GetLocalUserProfile().accountId, BuildExitHangoutDetails(details)));
            });
        }

        private void OnSendMessageEvent(IEvent<string> obj)
        {
            if (!IsTracking)
            {
                return;
            }

            textChatCount++;
        }

        private void OnToggleMuteEvent(IEvent<string> callback)
        {
            if (!IsTracking)
            {
                return;
            }
            
            var toggleMuteEvent = (ToggleMuteEvent)callback;
            IsMuted = toggleMuteEvent.IsMuted;
        }

        private void OnEmoteUseEvent(IEvent<string> callback)
        {
            if (!IsTracking)
            {
                return;
            }
            
            var emoteUseEvent = (EmoteUseEvent)callback;
            var isSelf = UserProfileLocalDataManager.Instance.GetLocalUserProfile().accountId == emoteUseEvent.PlayerId;

            if (!isSelf)
            {
                return;
            }

            emotesSoloCount++;
        }

        #endregion

        #region Monobehaviours

        private void Update()
        {
            if (!IsTracking)
            {
                return;
            }

            UpdateVoiceDuration();
        }

        private void OnObjectInteractedLocal(EventOnObjectInteractedLocal callback)
        {
            if (!IsTracking)
            {
                return;
            }

            interactiveObjectsCount++;
        }

        private void OnEnable()
        {
            GlobalNotifier.Instance.SubscribeOn(JoinHangoutEvent.EVENT_ID, OnJoinHangoutEvent);
            GlobalNotifier.Instance.SubscribeOn(LeaveHangoutEvent.EVENT_ID, OnLeaveHangoutEvent);
            GlobalNotifier.Instance.SubscribeOn(SendMessageEvent.EVENT_ID, OnSendMessageEvent);
            GlobalNotifier.Instance.SubscribeOn(ToggleMuteEvent.EVENT_ID, OnToggleMuteEvent);
            QuantumEvent.Subscribe<EventOnObjectInteractedLocal>(listener: this, handler: OnObjectInteractedLocal);
            GlobalNotifier.Instance.SubscribeOn(EmoteUseEvent.EVENT_ID, OnEmoteUseEvent);
        }

        private void OnDisable()
        {
            GlobalNotifier.Instance.UnSubscribeFor(JoinHangoutEvent.EVENT_ID, OnJoinHangoutEvent);
            GlobalNotifier.Instance.UnSubscribeFor(LeaveHangoutEvent.EVENT_ID, OnLeaveHangoutEvent);
            GlobalNotifier.Instance.UnSubscribeFor(SendMessageEvent.EVENT_ID, OnSendMessageEvent);
            GlobalNotifier.Instance.UnSubscribeFor(ToggleMuteEvent.EVENT_ID, OnToggleMuteEvent);
            QuantumEvent.Subscribe<EventOnObjectInteractedLocal>(listener: this, handler: OnObjectInteractedLocal);
            GlobalNotifier.Instance.UnSubscribeFor(EmoteUseEvent.EVENT_ID, OnEmoteUseEvent);
        }

        #endregion
    }
}
