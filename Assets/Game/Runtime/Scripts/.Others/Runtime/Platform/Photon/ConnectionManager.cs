using System.Collections.Generic;
using Kumu.Extensions;
using Kumu.Kulitan.Backend;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.UI;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;

namespace Kumu.Kulitan.Multiplayer
{
    // TODO: remove singleton and connection callbacks here
    public class ConnectionManager : SingletonMonoBehaviour<ConnectionManager>, IConnectionCallbacks
    {
        public enum PhotonEventCode : byte
        {
            StartGame = 110
        }

        private bool ignoreDisconnectCallback;
        
        [SerializeField] private string devNoAuthAppId;
        [SerializeField] private string devAppId;
        [SerializeField] private string stagingAppId;
        [SerializeField] private string releaseAppId;
        [SerializeField] private string defaultRegionCode = "hk";
        [SerializeField] private UnityEvent onConnectedToMaster = null;

        public bool IgnoreDisconnectCallback
        {
            get => ignoreDisconnectCallback;
            set
            {
                if (value == false && ignoreDisconnectCallback)
                {
                    CheckConnection();
                }
                ignoreDisconnectCallback = value;
            }
        }

        public static event UnityAction OnConnectedToServer = delegate { };
        public static event UnityAction OnDisconnect = delegate { };
        public static event UnityAction OnDisconnectedFromServerByClient = delegate { };

        public static bool IsConnected => Client.IsConnectedAndReady;
        
        public static QuantumLoadBalancingClient Client { get; private set; }
        public LevelConfigScriptableObject CurrentLevelConfig { get; set; }

        private AppSettings photonAppSettings;

        public void ConnectToServer()
        {
            if (string.IsNullOrEmpty(photonAppSettings.AppIdRealtime.Trim()))
            {
                Debug.LogError("App ID cannot be null, please configure an AppId in PhotonServerSettings");
            }
            
            $"Connecting to server...".Log();

            if (!Client.IsConnected && !Client.ConnectUsingSettings(photonAppSettings))
            {
                Debug.LogError($"Failed to connect with app settings: '{photonAppSettings.ToStringFull()}");
                var popup = PopupManager.Instance.OpenErrorPopup("Error", "Failed to connect to server", "Retry");
                popup.OnClosed += ConnectToServer;
                
                //ensure client is disconnected before attempting reconnect
                Client.Disconnect();
            }
        }

        public void DisconnectFromGame()
        {
            Client.Disconnect();
            QuantumRunner.ShutdownAll(true);
        }

        #region ConnectionCallbacks

        public void OnConnected()
        {
        }

        public void OnConnectedToMaster()
        {
            Debug.Log($"{Client.UserId} Connected to server: {Client.MasterServerAddress} | {Client.CloudRegion}");
            GlobalNotifier.Instance.Trigger(new ConnectedToServerEvent());
            OnConnectedToServer?.Invoke(); // TODO: convert usages to event
            onConnectedToMaster?.Invoke();
        }

        public void OnDisconnected(DisconnectCause cause)
        {
            if (IgnoreDisconnectCallback)
            {
                return;
            }
            
            $"Disconnected: {cause}".Log();
            OnDisconnect?.Invoke(); // TODO: convert usages to event
            GlobalNotifier.Instance.Trigger(new DisconnectedFromServerEvent());

            switch (cause)
            {
                case DisconnectCause.DisconnectByClientLogic:
                    OnDisconnectedFromServerByClient?.Invoke();
                    break;
            }
        }

        public void OnRegionListReceived(RegionHandler regionHandler)
        {
        }

        public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
        {
        }

        public void OnCustomAuthenticationFailed(string debugMessage)
        {
        }

        #endregion

        /// <summary>
        /// Used by FSM.
        /// </summary>
        public void InitializePhotonSettings()
        {
            photonAppSettings = PhotonServerSettings.CloneAppSettings(PhotonServerSettings.Instance.AppSettings);
#if PHOTON_RELEASE
            photonAppSettings.AppIdRealtime = releaseAppId;
            $"Connecting to photon PROD {releaseAppId}".Log();
#elif PHOTON_STAGING
            photonAppSettings.AppIdRealtime = stagingAppId;
            $"Connecting to photon STG {stagingAppId}".Log();
#elif !USES_MOCKS
            photonAppSettings.AppIdRealtime = devAppId;
            $"Connecting to photon DEV {devAppId}".Log();
#else
            photonAppSettings.AppIdRealtime = devNoAuthAppId;
            $"Connecting to photon DEV-NOAUTH {devAppId}".Log();
#endif
            photonAppSettings.FixedRegion = defaultRegionCode;
    
            InitializePhotonUserProfile();
            
            Client.AddCallbackTarget(this);
        }

        private void InitializePhotonUserProfile()
        {
            var userProfile = UserProfileLocalDataManager.Instance.GetLocalUserProfile();

            Client = new QuantumLoadBalancingClient(photonAppSettings.Protocol)
            {
                UserId = userProfile.accountId,
                NickName = userProfile.nickName
            };
#if !USES_MOCKS
            Client.AuthValues = CreateAuthValues(userProfile.accountId, BackendUtil.DeviceUniqueIdentifier);
#endif
        }

        private AuthenticationValues CreateAuthValues(string userId, string deviceId)
        {
            var authValues = new AuthenticationValues();
            authValues.AuthType = CustomAuthenticationType.Custom;
            authValues.AddAuthParameter("user", userId);
            authValues.AddAuthParameter("pass", deviceId);
            authValues.UserId = userId;

            return authValues;
        }

        private void CheckConnection()
        {
            ConnectToServer();
        }

        private void Update()
        {
            Client?.Service();
        }
    }
}
