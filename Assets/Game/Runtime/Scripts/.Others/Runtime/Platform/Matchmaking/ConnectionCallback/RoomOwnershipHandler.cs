using ExitGames.Client.Photon;
using Kumu.Kulitan.Backend;
using Kumu.Kulitan.Multiplayer;
using Kumu.Kulitan.Social;
using Photon.Realtime;
using UnityEngine;

namespace Kumu.Kulitan.Hangout
{
    public class RoomOwnershipHandler : MonoBehaviour, IInRoomCallbacks
    {   
        public void OnPlayerEnteredRoom(Player newPlayer)
        {
        }

        /// <summary>
        /// Callback for when a Player leaves the room.
        /// This callback is invoked -after- OnMasterClientSwitched,
        /// so Player.IsMasterClient will always return false when checking from here. 
        /// </summary>
        /// <param name="otherPlayer"></param>
        public void OnPlayerLeftRoom(Player otherPlayer)
        {
        }

        public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
        }

        public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
        }

        public void OnMasterClientSwitched(Player newMasterClient)
        {
            SocialManager.Instance.ClearUserRoom();

            ConnectionManager.Instance.DisconnectFromGame();
            RoomConnectionDetails.Instance.roomExitMode = RoomExitMode.OnHostLeft;
        }

        private void OnEnable()
        {
            ConnectionManager.Client.AddCallbackTarget(this);
        }

        private void OnDisable()
        {
            ConnectionManager.Client.RemoveCallbackTarget(this);
        }
    }
}
