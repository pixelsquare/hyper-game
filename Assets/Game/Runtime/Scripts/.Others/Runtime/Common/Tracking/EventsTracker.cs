using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;
using Kumu.Kulitan.Hangout;
using UnityEngine;

namespace Kumu.Kulitan.Tracking
{
    public class EventsTracker : MonoBehaviour
    {
        public static IEventsTrackerInit Current;

        private Slot<string> eventSlot;

        private void Awake()
        {
#if TRACKING_DEBUG
            Current = new DebugTracker();
#else
            Current = new FirebaseTracker();
#endif

            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            eventSlot = new Slot<string>(GlobalNotifier.Instance);

            Current.Init(eventSlot);

            if (Current is ITrackUserHandle trackUserHandle)
            {
                eventSlot.SubscribeOn(TrackUserEvent.EVENT_ID,
                        eventData => trackUserHandle.OnTrackUserEvent(eventData as TrackUserEvent));
            }

            if (Current is IUserJourneyHandle userJourneyHandle)
            {
                eventSlot.SubscribeOn(UserJourneyEvent.EVENT_ID,
                        eventData => userJourneyHandle.OnUserJourneyEvent(eventData as UserJourneyEvent));
            }

            if (Current is IEmoteUseHandle emoteUseHandle)
            {
                eventSlot.SubscribeOn(EmoteUseEvent.EVENT_ID,
                        eventData => emoteUseHandle.OnEmoteUseEvent(eventData as EmoteUseEvent));
            }

            if (Current is IScreenViewHandle screenViewHandle)
            {
                eventSlot.SubscribeOn(ScreenViewEvent.EVENT_ID,
                        eventData => screenViewHandle.OnScreenView(eventData as ScreenViewEvent));
            }

            if (Current is IPhotoModeHandle photoModeHandle)
            {
                eventSlot.SubscribeOn(OnPhotoModeStart.EVENT_NAME, photoModeHandle.OnPhotoModeStart);
                eventSlot.SubscribeOn(PhotoModeEvent.TAKE_ID, photoModeHandle.OnPhotoTaken);
                eventSlot.SubscribeOn(PhotoModeEvent.END_ID,
                        eventData => photoModeHandle.OnPhotoModeEnd(eventData as PhotoModeEvent));
            }

            if (Current is IVGGiftingHandle vgGiftingHandle)
            {
                eventSlot.SubscribeOn(VGGiftingEvent.EVENT_ID, eventData
                        => vgGiftingHandle.OnVGGiftingEvent(eventData as VGGiftingEvent));
            }
            
            if (Current is IVGTrayOpenHandle vgTrayOpenHandle)
            {
                eventSlot.SubscribeOn(VGTrayOpenEvent.EVENT_ID, eventData
                    => vgTrayOpenHandle.OnVGTrayOpenEvent(eventData as VGTrayOpenEvent));
            }

            if (Current is IInteractHandle interactHandle)
            {
                eventSlot.SubscribeOn(InteractEvent.EVENT_ID, eventData => interactHandle.OnInteractEvent(eventData as InteractEvent));
            }

            if (Current is IItemPurchasedHandle itemPurchasedHandle)
            {
                eventSlot.SubscribeOn(ItemPurchasedEvent.EVENT_ID, eventData
                        => itemPurchasedHandle.OnItemPurchasedEvent(eventData as ItemPurchasedEvent));
            }

            if (Current is IItemSelectedHandle itemSelectedHandle)
            {
                eventSlot.SubscribeOn(ItemSelectedEvent.EVENT_ID, eventData
                        => itemSelectedHandle.OnItemSelectedEvent(eventData as ItemSelectedEvent));
            }

            if (Current is ISendMessageHandle sendMessageHandle)
            {
                eventSlot.SubscribeOn(SendMessageEvent.EVENT_ID, eventData
                    => sendMessageHandle.OnSendMessageEvent(eventData as SendMessageEvent));
            }

            if (Current is IStartHangoutHandle startHangoutHandle)
            {
                eventSlot.SubscribeOn(StartHangoutEvent.EVENT_ID, eventData => startHangoutHandle.OnStartHangoutEvent(eventData as StartHangoutEvent));
            }

            if (Current is IJoinHangoutHandle joinHangoutHandle)
            {
                eventSlot.SubscribeOn(JoinHangoutEvent.EVENT_ID, eventData => joinHangoutHandle.OnJoinHangoutEvent(eventData as JoinHangoutEvent));
            }

            if (Current is IEndHangoutHandle endHangoutHandle)
            {
                eventSlot.SubscribeOn(EndHangoutEvent.EVENT_ID, eventData => endHangoutHandle.OnEndHangoutEvent(eventData as EndHangoutEvent));
            }

            if (Current is IExitHangoutHandle exitHangoutHandle)
            {
                eventSlot.SubscribeOn(ExitHangoutEvent.EVENT_ID, eventData => exitHangoutHandle.OnExitHangoutEvent(eventData as ExitHangoutEvent));
            }
        }

        private void OnDisable()
        {
            eventSlot.Dispose();
        }
    }
}
