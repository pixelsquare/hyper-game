using Kumu.Kulitan.Common;
using Kumu.Kulitan.Multiplayer;
using Newtonsoft.Json;
using Photon.Realtime;
using TMPro;
using UnityEngine;

namespace Kumu.Kulitan.UI
{
    public class HangoutSettingsInfo : MonoBehaviour
    {
        [SerializeField] private TMP_Text roomNameLabel;
        [SerializeField] private TMP_Text roomDescLabel;

        [SerializeField] private TMP_Text hangoutTypeLabel;
        [SerializeField] private TMP_Text visibilityLabel;
        [SerializeField] private TMP_Text passwordLabel;
        [SerializeField] private TMP_Text maxPlayerCountLabel;

        private void Initialize()
        {
            var currentRoom = ConnectionManager.Client.CurrentRoom;

            if (currentRoom.CustomProperties.TryGetValue(Constants.ROOM_DETAILS_PROP_KEY, out var roomDetailsObj))
            {
                var roomDetails = JsonConvert.DeserializeObject<RoomDetails>(roomDetailsObj.ToString());
                roomNameLabel.text = roomDetails.roomName;
                roomDescLabel.text = roomDetails.roomId;
                hangoutTypeLabel.text = roomDetails.layoutName;
            }

            visibilityLabel.text = currentRoom.IsVisible ? "Public" : "Friends Only";
            passwordLabel.text = IsPasswordProtected(currentRoom) ? "YES" : "NO";
            maxPlayerCountLabel.text = $"{currentRoom.MaxPlayers}";
        }

        private bool IsPasswordProtected(RoomInfo room)
        {
            if (room.CustomProperties.TryGetValue(Constants.ROOM_PASS_PROP_KEY, out var passwordObj))
            {
                return passwordObj != null && !string.IsNullOrEmpty(passwordObj.ToString());
            }

            return false;
        }

        private void Start()
        {
            Initialize();
        }
    }
}
