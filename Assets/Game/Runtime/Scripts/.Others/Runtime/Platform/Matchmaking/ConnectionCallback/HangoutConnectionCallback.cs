using System.Collections.Generic;
using System.Threading.Tasks;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;
using Kumu.Kulitan.Hangout;
using Kumu.Kulitan.Social;
using Kumu.Kulitan.UI;
using Photon.Realtime;
using Quantum;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kumu.Kulitan.Multiplayer
{
    /// <summary>
    /// Connection Callback class used inside any hangout room.
    /// </summary>
    public class HangoutConnectionCallback : ConnectionCallback, ILobbyCallbacks
    {
        [SerializeField] private Reconnecter reconn;
        
        private Slot<string> eventSlot;
        private bool returningToMenu;
        private List<RoomInfo> allRooms;
        private bool isReconnecting;
        private string reconSceneName;

        public void ReturnToMenu()
        {
            returningToMenu = true; //avoid handling of this specific disconnect locally
            ConnectionManager.Client.Disconnect(DisconnectCause.None); //ensure disconnection from server to avoid errors
            PopupManager.Instance.CloseAllPopups();

            LoadMainScreenOnlyAsync();
        }
        
        private async Task LoadMainScreenOnlyAsync()
        {
            while (SceneLoadingManager.Instance.IsBusy)
            {
                await Task.Yield();
            }
            
            var currentScenes = new List<string>();
            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                currentScenes.Add(SceneManager.GetSceneAt(i).name);
            }
            
            SceneLoadingManager.Instance.LoadMainScreenOnly(() =>
            {
                ConnectionManager.Instance.ConnectToServer();
            }, ignoreChecks: true);
        }

        public void AttemptReconnect()
        {            
            reconSceneName = SceneManager.GetActiveScene().name;
            isReconnecting = true;
            allRooms = null;
            
            ConnectionManager.Instance.ConnectToServer();
            PopupManager.Instance.OpenTextPopup("Connecting", "Please wait...", "");
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            if (returningToMenu)
            {
                return;
            }
            
            if (cause == DisconnectCause.DisconnectByClientLogic)
            {
                ReturnToMenu();
                return;
            }
            
            SocialManager.Instance.ClearUserRoom();

            ConnectionManager.Instance.DisconnectFromGame();

            var photoManager = FindObjectOfType<PhotoModeManager>();
            if (photoManager != null && photoManager.PhotoCamera.enabled)
            {
                photoManager.EndPhotoMode();
                photoManager.GameUI.SetActive(true);
                photoManager.PhotoModeUI.SetActive(false);
            }

            var popup = (ConfirmationPopup)PopupManager.Instance.OpenConfirmationPopup("Error", $"Connection Failed", "Retry", "Menu");
            popup.OnConfirm += AttemptReconnect;
            popup.OnCancel += ReturnToMenu;
            
            reconn.Initialize();
        }

        public override void OnConnected()
        {
            GlobalNotifier.Instance.Trigger(new GameSceneLoadedEvent());
        }

        public override void OnConnectedToMaster()
        {
            if (returningToMenu)
            {
                return;
            }
            
            ConnectionManager.Client.OpJoinLobby(RoomConnectionDetails.Instance.myLobby);
        }
     
        #region ILobbyCallbacks
        public void OnJoinedLobby()
        {
        }
        
        public void OnLeftLobby()
        {
        }

        public void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            allRooms = roomList;
        }

        public void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
        {
        }
        #endregion
        
        private void OnCallbackPluginDisconnect(string callbackReason)
        {
            ConnectionManager.Instance.DisconnectFromGame();
        }

        private void ReloadHangoutScenes()
        {
            SceneLoadingManager.Instance.UnloadScene(reconSceneName, () =>
            {
                SceneLoadingManager.Instance.UnloadScene(SceneNames.HANGOUT_SHARED_SCENE, () =>
                {
                    SceneLoadingManager.Instance.LoadScene(SceneNames.HANGOUT_SHARED_SCENE, "", true, () =>
                    {
                        SceneLoadingManager.Instance.LoadScene(reconSceneName, "", true,() =>
                        {
                            SceneLoadingManager.Instance.SetActiveScene(reconSceneName);
                            ConnectionManager.Client.OpJoinRoom(RoomConnectionDetails.Instance.enterRoomParams);
                        });
                    });
                });
            });
        }

        private void ReloadMinigameScenes()
        {
            SceneLoadingManager.Instance.UnloadScene(reconSceneName, () =>
            {
                SceneLoadingManager.Instance.UnloadScene(SceneNames.HANGOUT_MINIGAME_SHARED_SCENE, () =>
                {
                    SceneLoadingManager.Instance.UnloadScene(SceneNames.HANGOUT_SHARED_SCENE, () =>
                    {
                        SceneLoadingManager.Instance.LoadScene(SceneNames.HANGOUT_SHARED_SCENE, "", true, () =>
                        {
                            SceneLoadingManager.Instance.LoadScene(SceneNames.HANGOUT_MINIGAME_SHARED_SCENE, "", true, () =>
                            {
                                SceneLoadingManager.Instance.LoadScene(reconSceneName, "", true,() =>
                                {
                                    SceneLoadingManager.Instance.SetActiveScene(reconSceneName);
                                    ConnectionManager.Client.OpJoinRoom(RoomConnectionDetails.Instance.enterRoomParams);
                                });
                            });
                        });                        
                    });
                });
            });
        }

        private void Start()
        {
            eventSlot = new Slot<string>(GlobalNotifier.Instance);
            QuantumCallback.Subscribe(this, (CallbackPluginDisconnect c) => OnCallbackPluginDisconnect(c.Reason));
        }

        private void Update()
        {
            if (isReconnecting && allRooms != null)
            {
                foreach (var info in allRooms)
                {
                    if (info.Name == RoomConnectionDetails.Instance.enterRoomParams.RoomName && info.PlayerCount > 0)
                    {
                        isReconnecting = false;

                        if (RoomConnectionDetails.Instance.levelConfig.HasMinigame)
                        {
                            ReloadMinigameScenes();
                        }
                        else
                        {
                            ReloadHangoutScenes();
                        }
                        
                        return;
                    }
                }

                //no room match
                PopupManager.Instance.CloseAllPopups();
                var popup = PopupManager.Instance.OpenTextPopup("Error", $"Room does not exist", "Menu");
                popup.OnClosed += ReturnToMenu;
                isReconnecting = false;
            }
        }
        
        private void OnDestroy()
        {
            eventSlot.Dispose();
            QuantumCallback.UnsubscribeListener(this);
        }
    }
}
