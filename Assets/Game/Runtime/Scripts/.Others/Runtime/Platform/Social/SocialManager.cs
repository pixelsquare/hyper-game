using System.Collections;
using Kumu.Extensions;
using Kumu.Kulitan.Backend;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;
using Kumu.Kulitan.Multiplayer;
using Photon.Realtime;
using UnityEngine;

namespace Kumu.Kulitan.Social
{
    public class SocialManager : SingletonMonoBehaviour<SocialManager>
    {
        [SerializeField] private RoomLayoutConfigs roomLayoutConfigs;
        
        private Slot<string> eventSlot;

        public RoomLayoutConfigs RoomLayoutConfigs => roomLayoutConfigs;

        public void JoinRoomDirect(string roomId)
        {
            var enterRoomParams = new EnterRoomParams();
            enterRoomParams.RoomName = roomId;
            ConnectionManager.Client.OpJoinRoom(enterRoomParams);
        }

        public async void RegisterCreatedRoom(string roomId, bool isFriendsOnly)
        {
            var request = new RecordUserCreatedRoomRequest();
            request.roomId = roomId;
            request.friendsOnly = isFriendsOnly;

            var result = await Services.SocialService.RecordUserCreatedRoomAsync(request);
            if (result.HasError)
            {
                $"[SocialManager] Error encountered when registering created room {roomId}. ({result.Error.Code})".LogError();
            }
        }

        public void ClearUserRoom()
        {
            //task gets cancelled without the slight delay
            StartCoroutine("WaitThenDoClearRoom");
        }

        private IEnumerator WaitThenDoClearRoom()
        {
            yield return new WaitForSeconds(0.05f);
            
            var request = new ClearUserRoomRecordRequest();

            var task = Services.SocialService.ClearUserRoomRecordAsync(request);
            
            yield return new WaitUntil(() => task.IsCompleted);
            
            if (task.Result.HasError)
            {
                $"[SocialManager] Error encountered when clearing created room. ({task.Result.Error.Code}) ({task.Result.Error.Message})".LogError();
            }
            
            StopCoroutine("WaitThenDoClearRoom");
        }
    }
}
