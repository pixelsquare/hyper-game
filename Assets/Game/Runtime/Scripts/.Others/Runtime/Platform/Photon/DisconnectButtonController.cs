using Kumu.Kulitan.Backend;
using Kumu.Kulitan.Hangout;
using Kumu.Kulitan.Social;
using UnityEngine;

namespace Kumu.Kulitan.Multiplayer
{
    public class DisconnectButtonController : MonoBehaviour
    {
        public void OnDisconnectClicked()
        {
            SocialManager.Instance.ClearUserRoom();
            ConnectionManager.Instance.DisconnectFromGame();
            RoomConnectionDetails.Instance.roomExitMode = RoomExitMode.Normal;
        }       
    }
}
