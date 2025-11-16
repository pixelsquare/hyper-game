using Kumu.Kulitan.Backend;
using Kumu.Kulitan.Multiplayer;
using Kumu.Kulitan.UI;
using UnityEngine;

namespace Kumu.Kulitan.Social
{
    public class RoomKeepAlive : MonoBehaviour
    {
        [Tooltip("How often the user's entered room data will be called in seconds.")]
        [SerializeField] private float updateRoomDataFrequency = 10;

        private bool isActive;
        private float timer;
        private RecordUserEnteredRoomRequest reusableRequest;

        private bool IsActive => ConnectionManager.Client.IsConnected && ConnectionManager.Client.InRoom;
        
        private const float SMALL_DELAY = 3f;

        private async void RecordUserEnterRoom()
        {
            reusableRequest = new RecordUserEnteredRoomRequest();
            reusableRequest.roomId = ConnectionManager.Client.CurrentRoom.Name;

            var result = await Services.SocialService.RecordUserEnteredRoomAsync(reusableRequest);
            if (result.HasError && result.Error.Code == ServiceErrorCodes.ROOM_ID_DOES_NOT_EXIST)
            {
                isActive = false;
                
                var errorPopup = PopupManager.Instance.OpenErrorPopup("Room Missing", "Room does not exist.", "Back");
                errorPopup.OnClosed += () =>
                {
                    ConnectionManager.Instance.DisconnectFromGame();
                };
            }
        }

        private void SetTimerToEnd(float offset = 0)
        {
            timer = updateRoomDataFrequency - offset;
        }

        #region Monobehaviour

        private void Update()
        {
            if (!IsActive)
            {
                SetTimerToEnd(SMALL_DELAY);
                return;
            }
            
            timer += Time.deltaTime;

            if (!(timer >= updateRoomDataFrequency))
            {
                return;
            }
            
            reusableRequest = new RecordUserEnteredRoomRequest();
            reusableRequest.roomId = ConnectionManager.Client.CurrentRoom.Name;
            RecordUserEnterRoom();
            timer = 0;
        }

        private void Start()
        {
            SetTimerToEnd(SMALL_DELAY);
        }

        #endregion
    }
}
