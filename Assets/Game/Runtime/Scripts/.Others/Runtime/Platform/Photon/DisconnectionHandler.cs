using UnityEngine;
using Kumu.Kulitan.Common;
using Kumu.Extensions;

namespace Kumu.Kulitan.Multiplayer
{
    public class DisconnectionHandler : MonoBehaviour
    {
        private void ReturnToMenu()
        {
            SceneLoadingManager.Instance.LoadMainScreen(OnMainScreenLoaded, true, false);
        }

        private void OnMainScreenLoaded()
        {
            $"Server State: {ConnectionManager.Client.State}".Log();
            ConnectionManager.Client.ReconnectToMaster();
            LoadingScreenManager.Instance.HideLoadingScreen();
        }

        private void OnDisconnectedFromServerByClient()
        {
            "Disconnected from server by client. Returning to Main Menu".Log();
            ReturnToMenu();
        }

        private void OnEnable()
        {
            ConnectionManager.OnDisconnectedFromServerByClient += OnDisconnectedFromServerByClient;
        }

        private void OnDisable()
        {
            ConnectionManager.OnDisconnectedFromServerByClient -= OnDisconnectedFromServerByClient;
        }
    }
}
