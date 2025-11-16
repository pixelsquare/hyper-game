using System.Collections.Generic;
using System.Linq;
using Kumu.Kulitan.Backend;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;
using Kumu.Kulitan.Gifting;
using Kumu.Kulitan.Multiplayer;
using Photon.Realtime;
using UnityEngine;

namespace Kumu.Kulitan.Tracking.Components
{
    public class EndHangoutEventTrigger : MonoBehaviour
    {
        private HangoutEventTriggerSharedPropertiesTracker sharedPropsTracker;

        private Dictionary<string, int> gifters = new ();
        private int maxVisitorsCount;
        private int uniqueVisitorsCount;
        
        private bool IsTracking { get; set; }
        
        public void Init()
        {
            gifters.Clear();
            maxVisitorsCount = 0;
            uniqueVisitorsCount = 0;
            sharedPropsTracker = new HangoutEventTriggerSharedPropertiesTracker();
        }

        private void UpdateGifterTable(VirtualGiftEventInfo vgEvent)
        {
            if (!IsTracking)
            {
                return;
            }
            
            var gifter = vgEvent.gifter;
            if (!gifters.ContainsKey(gifter))
            {
                gifters.Add(gifter, 0);
            }

            gifters[gifter] += vgEvent.GetTotalValue();
        }

        private void UpdateUniqueVisitorsCount(Room currentRoom)
        {
            var props = currentRoom.CustomProperties;
            
            if (props.ContainsKey(Constants.UNIQUE_VISITORS_PROP_KEY))
            {
                uniqueVisitorsCount = (int)props[Constants.UNIQUE_VISITORS_PROP_KEY];
            }
        }

        private void UpdateMaxVisitorsCount(Room currentRoom)
        {
            var props = currentRoom.CustomProperties;

            if (props.ContainsKey(Constants.MAX_CONCURRENT_VISITORS_PROP_KEY))
            {
                maxVisitorsCount = (int)props[Constants.MAX_CONCURRENT_VISITORS_PROP_KEY];
            }
        }

        private string[] GetTopThreeGifters()
        {
            var topThree = gifters.OrderByDescending(kvp => kvp.Value)
                                  .Take(3)
                                  .Select(kvp => kvp.Key)
                                  .ToList();
            
            for (var i = topThree.Count; i < 3; i++)
            {
                topThree.Add("null");
            }

            return topThree.ToArray();
        }
        
        private EndHangoutDetails BuildEndHangoutDetails(HangoutEventTriggerSharedPropertiesTracker.HangoutSharedPropertiesDetails details)
        {
            return new EndHangoutDetails(
                details.hangoutId,
                details.sessionId,
                details.roomDuration,
                details.diamondsReceived,
                details.followersGained,
                GetTopThreeGifters(),
                maxVisitorsCount, 
                uniqueVisitorsCount);
        }

        #region EventHandlers

        private void OnStartHangoutEvent(IEvent<string> callback)
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
            
            sharedPropsTracker.EndTrackingAsync(details => GlobalNotifier.Instance.Trigger(new EndHangoutEvent(UserProfileLocalDataManager.Instance.GetLocalUserProfile().accountId, BuildEndHangoutDetails(details))));
        }
        
        private void OnVirtualGiftReceivedEvent(VirtualGiftEventInfo vgEvent)
        {
            if (!IsTracking)
            {
                return;
            }
            
            UpdateGifterTable(vgEvent);
        }

        #endregion

        #region Monobehaviours

        public void Update()
        {
            if (!IsTracking)
            {
                return;
            }
            
            var currentRoom = ConnectionManager.Client.CurrentRoom;
            if (currentRoom == null)
            {
                return;
            }
            
            // We keep and update our own copy of maxConcurrentVisitors and uniqueVisitors
            // because disconnecting from the room means we lose access to RoomCustomProperties
            UpdateMaxVisitorsCount(currentRoom);
            UpdateUniqueVisitorsCount(currentRoom);
        }

        private void OnEnable()
        {
            GlobalNotifier.Instance.SubscribeOn(StartHangoutEvent.EVENT_ID, OnStartHangoutEvent);
            GlobalNotifier.Instance.SubscribeOn(LeaveHangoutEvent.EVENT_ID, OnLeaveHangoutEvent);
            Services.VirtualGiftService.VirtualGiftReceivedEvent += OnVirtualGiftReceivedEvent;
        }

        private void OnDisable()
        {
            GlobalNotifier.Instance.UnSubscribeFor(StartHangoutEvent.EVENT_ID, OnStartHangoutEvent);
            GlobalNotifier.Instance.UnSubscribeFor(LeaveHangoutEvent.EVENT_ID, OnLeaveHangoutEvent);
            Services.VirtualGiftService.VirtualGiftReceivedEvent -= OnVirtualGiftReceivedEvent;
        }

        #endregion
    }
}
