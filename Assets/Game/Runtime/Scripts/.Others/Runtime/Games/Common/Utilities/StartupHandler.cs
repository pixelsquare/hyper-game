using AppleAuth;
using AppleAuth.Native;
using Kumu.Extensions;
using Kumu.Kulitan.Backend;
using Kumu.Kulitan.Tracking;
using Newtonsoft.Json.Utilities;
using UnityEngine;

namespace Kumu.Kulitan.Common
{
    /// <summary>
    /// Handles necessary initialization for the entire application.
    /// </summary>
    public class StartupHandler : MonoBehaviour
    {
        /// <summary>
        /// Used by FSM.
        /// </summary> 
        public void LoadSignUpScene()
        {
            SceneLoadingManager.Instance.LoadSceneAsAdditive(SceneNames.SIGNUP_SCENE);
            GlobalNotifier.Instance.Trigger(new UserJourneyEvent(UserJourneyEvent.Checkpoint.LandingPage));
        }

        public void Initialize()
        {
            Application.targetFrameRate = 30;

            AotHelper.EnsureList<Currency>();
            AotHelper.EnsureList<LobbyConfig>();
            AotHelper.EnsureDictionary<string, AgoraRtcToken>();
            AotHelper.EnsureDictionary<string, AgoraRtmToken>();

            Services.Initialize();
            Validators.Initialize();

            InitializeAppleAuth();

#if ENABLE_LOGS
            SRDebug.Init();
#endif
        }

        /// <summary>
        /// Used by FSM.
        /// </summary>
        public bool IsDeviceRootedOrJailbroken()
        {
            return RootJailbreakChecker.IsDeviceRootedOrJailbroken();
        }
        
        /// <summary>
        /// Used by FSM.
        /// </summary>
        public void ApplicationQuit()
        {
            Application.Quit();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
#endif
        }

        // must be initialized after services
        private void InitializeAppleAuth()
        {
            // If the current platform is supported
            if (AppleAuthManager.IsCurrentPlatformSupported)
            {
                "Apple auth initialized".Log();
                // Creates a default JSON deserializer, to transform JSON Native responses to C# instances
                var deserializer = new PayloadDeserializer();
                // Creates an Apple Authentication manager with the deserializer
                FirebaseAuthService.appleAuthManager = new AppleAuthManager(deserializer);
                return;
            }
        }

        private void Update()
        {
            if(FirebaseAuthService.appleAuthManager != null)
            {
                FirebaseAuthService.appleAuthManager.Update();
            }
        }
    }
}
