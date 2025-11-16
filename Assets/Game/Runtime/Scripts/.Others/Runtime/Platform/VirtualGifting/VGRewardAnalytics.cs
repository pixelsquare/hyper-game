using System.Text;
using Kumu.Extensions;
using Kumu.Kulitan.Backend;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;
using Kumu.Kulitan.Multiplayer;
using Newtonsoft.Json;
using UnityEngine;

namespace Kumu.Kulitan.Gifting
{
    public class VGRewardAnalytics : MonoBehaviour
    {
        private void OnReceiveGiftEvent(VirtualGiftEventInfo info)
        {
            var currentRoom = ConnectionManager.Client.CurrentRoom;
            var localProfile = UserProfileLocalDataManager.Instance.GetLocalUserProfile();

            if (BaseMatchmakingHandler.State != BaseMatchmakingHandler.RoomStatus.IN_ROOM)
            {
                return;
            }

            if (currentRoom == null)
            {
                return;
            }

            if (!currentRoom.CustomProperties.TryGetValue(Constants.ROOM_DETAILS_PROP_KEY, out var roomDetailsObj))
            {
                "Failed to send VGGiftingEvent analytics.".LogError();
                return;
            }

            var roomDetails = JsonConvert.DeserializeObject<RoomDetails>(roomDetailsObj.ToString());

            var giftConfigs = VirtualGiftDatabase.Current.GiftConfigs;

            var totalRewards = 0;
            var giftIds = new StringBuilder();

            foreach (var gift in info.gifts)
            {
                var giftConfig = giftConfigs.Find(item => item.Data.giftId.Equals(gift.id));
                totalRewards += giftConfig.Data.cost.amount * gift.quantity;
                giftIds.Append(gift.id).Append(",");
            }

            var vgGiftEvent = new VGGiftingEvent
            (
                    info.gifter, 0,
                    localProfile.accountId, 0,
                    roomDetails.layoutName, roomDetails.roomId,
                    giftIds.ToString(), totalRewards, info.category.ToString()
            );

            GlobalNotifier.Instance.Trigger(vgGiftEvent);
        }

        private void OnEnable()
        {
            Services.VirtualGiftService.VirtualGiftReceivedEvent += OnReceiveGiftEvent;
        }

        private void OnDisable()
        {
            Services.VirtualGiftService.VirtualGiftReceivedEvent -= OnReceiveGiftEvent;
        }
    }
}
