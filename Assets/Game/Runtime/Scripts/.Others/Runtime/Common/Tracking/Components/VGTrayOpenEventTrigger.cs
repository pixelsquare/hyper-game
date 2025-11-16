using System;
using Kumu.Extensions;
using Kumu.Kulitan.Avatar;
using Kumu.Kulitan.Backend;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;
using Kumu.Kulitan.Multiplayer;
using UnityEngine;

namespace Kumu.Kulitan.Tracking.Components
{
    public class VGTrayOpenEventTrigger : MonoBehaviour
    {
        private float duration;
        private bool isOpen;
        
        public void StartTracking()
        {
            isOpen = true;
            duration = 0;
        }

        public void EndTracking()
        {
            isOpen = false;
            Trigger();
        }

        private void Trigger()
        {
            var currentRoom = ConnectionManager.Client.CurrentRoom;
            var localProfile = UserProfileLocalDataManager.Instance.GetLocalUserProfile();
            var localCoinBalance = UserInventoryData.UserWallet.GetCurrency(Currency.UBE_COI).amount;
            var finalDuration = (int)Math.Round(duration);
            var maxConcurrentCount = AnalyticsDataTracker.VisitorCountTracker.GetMaxConcurrentVisitorsCount();
            var uniqueVisitorsCount = AnalyticsDataTracker.VisitorCountTracker.GetUniqueVisitorsCount();
            $"Unique visitors count: {uniqueVisitorsCount}".Log();
            $"Max concurrent count: {maxConcurrentCount}".Log();

            var roomDetails = currentRoom.GetRoomDetails();

            var trayOpenEvent = new VGTrayOpenEvent(localProfile.accountId, 0, localCoinBalance,
                roomDetails.layoutName, roomDetails.roomId, maxConcurrentCount,
                uniqueVisitorsCount, finalDuration);
            
            GlobalNotifier.Instance.Trigger(trayOpenEvent);
        }

        private void Update()
        {
            if (!isOpen)
            {
                return;
            }

            duration += Time.deltaTime;
        }
    }
}
