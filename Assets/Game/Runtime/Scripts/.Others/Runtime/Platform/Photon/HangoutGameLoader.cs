using Kumu.Extensions;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;
using Kumu.Kulitan.Hangout;
using Quantum;

namespace Kumu.Kulitan.Multiplayer
{
    public class HangoutGameLoader : SingletonMonoBehaviour<HangoutGameLoader>
    {
        private string sceneToLoad;
        private Slot<string> eventSlot;
        private GameSceneLoadedEvent gameSceneLoadedEvent = new();
        public bool IsBusy { get; private set; }

        public void LoadGameScene(string sceneToLoad)
        {
            IsBusy = true;
            
            this.sceneToLoad = sceneToLoad;
            string[] scenesToLoad = { sceneToLoad };
            string[] scenesToUnload = { SceneNames.MAIN_SCREEN, SceneNames.AVATAR_CUSTOMIZATION, SceneNames.SOCIAL_SCREEN, SceneNames.USER_PROFILE_SCREEN };

            SceneLoadingManager.Instance.LoadScene(scenesToLoad, scenesToUnload, false, () =>
            {
                OnGameSceneLoaded();
            });
        }
        
        private void OnGameSceneLoaded()
        {
            SceneLoadingManager.Instance.SetActiveScene(sceneToLoad);
            StartMatch(ConnectionManager.Instance.CurrentLevelConfig.Config);
            GlobalNotifier.Instance.Trigger(gameSceneLoadedEvent);
            IsBusy = false;
        }

        private void OnPlayerJoined(IEvent<string> callback)
        {
            var joinedEventCallback = (QuantumPlayerJoinedEvent)callback;
            if (!joinedEventCallback.IsLocal)
            {
                return;
            }
            LoadingScreenManager.Instance.HideLoadingScreen();
        }

        protected virtual void StartMatch(RuntimeConfig runtimeConfig)
        {
            if (ConnectionManager.Client.CurrentRoom == null)
            {
                "Trying to start a match but there is no room, disconnecting".Log();
                ConnectionManager.Client.Disconnect();
                return;
            }
            
            var startParams = new QuantumRunner.StartParameters
            {
                RuntimeConfig = runtimeConfig,
                DeterministicConfig = DeterministicSessionConfigAsset.Instance.Config,
                GameMode = Photon.Deterministic.DeterministicGameMode.Multiplayer,
                InitialFrame = 0,
                PlayerCount = ConnectionManager.Client.CurrentRoom.MaxPlayers,
                LocalPlayerCount = 1,
                RecordingFlags = RecordingFlags.None,
                NetworkClient = ConnectionManager.Client,
            };

            var clientId =
                ClientIdProvider.CreateClientId(ClientIdProvider.Type.PhotonUserId, ConnectionManager.Client);
            QuantumRunner.StartGame(clientId, startParams);

            RoomConnectionDetails.Instance.maxPlayers = startParams.PlayerCount;
            RoomConnectionDetails.Instance.clientId = clientId;
            RoomConnectionDetails.Instance.hasPromptedRoomExit = false;
        }
        
        private void OnEnable()
        {
            eventSlot = new Slot<string>(GlobalNotifier.Instance);
            eventSlot.SubscribeOn(QuantumPlayerJoinedEvent.EVENT_NAME, OnPlayerJoined);
        }

        private void OnDisable()
        {
            eventSlot?.Dispose();
        }
    }
}
