using System.Collections.Generic;
using Kumu.Extensions;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Hangout;
using Photon.Realtime;
using UnityEngine;

namespace Kumu.Kulitan.Multiplayer
{
    public class HangoutLobbyHandler : BaseLobbyHandler
    {
        private Dictionary<string, RoomInfo> cachedRoomList = new();

        public override void JoinLobby()
        {
        }

        public override void OnJoinedLobby()
        {
            cachedRoomList.Clear();
            base.OnJoinedLobby();
        }

        public override void OnLeftLobby()
        {
            cachedRoomList.Clear();
            base.OnLeftLobby();
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            UpdateCachedRoomList(roomList);
            GlobalNotifier.Instance.Trigger(new RoomListUpdatedEvent(cachedRoomList));
        }
        
        private void UpdateCachedRoomList(List<RoomInfo> roomList)
        {
            foreach (var info in roomList)
            {
                if (info.RemovedFromList)
                {
                    cachedRoomList.Remove(info.Name);
                }
                else
                {
                    cachedRoomList[info.Name] = info;
                }
            }
        }

        [ContextMenu("Log Details")]
        private void LogDetails()
        {
            $"lobb id: {RoomConnectionDetails.Instance.myLobby.Name}".Log();
            $"lobby label: {RoomConnectionDetails.Instance.lobbyLabel}".Log();
        }
    }
}
