using agora_gaming_rtc;
using Kumu.Extensions;

/// <summary>
/// Wrapper for audio and video chat 
/// Currently uses Agora Video SDK version 3.3.1.71
/// </summary>

namespace Kumu.Kulitan.Common
{
    public static class AudioVideoChatHelper
    {
        private static IRtcEngine Engine;

        public static IRtcEngine GetRTCEngine()
        {
            if (Engine != null)
            {
                return Engine;
            }

            Debug.LogError("[AudioVideoChatHelper] Trying to get Engine before it is initialized, returning null");
            return null;
        }

        public static event OnUserOfflineHandler OnUserLeft;

        public static bool IsInitialized { get; private set; }

        public static IAudioEffectManager GetAudioEffectManager()
        {
            return Engine.GetAudioEffectManager();
        }

        public static void InitializeEngine(string appId)
        {
            Engine = IRtcEngine.GetEngine(appId);
            IsInitialized = true;
        }

        public static void QueryActiveEngineInstance()
        {
            IRtcEngine.QueryEngine();
        }

        public static void DestroyActiveEngine()
        {
            IRtcEngine.Destroy();
        }

        public static void GetErrorCode(int code)
        {
            IRtcEngine.GetErrorDescription(code);
        }

        public static void LeaveActiveChannel()
        {
            Engine.LeaveChannel();
        }

        public static void PlayerJoinChannel(string channelName, string info, uint uid)
        {
            Engine.JoinChannel(channelName, info, uid);
        }

        public static void PlayerJoinChannelWithToken(string token, string channelName, string info, uint uid)
        {
            Engine.JoinChannel(token, channelName, info, uid, new ChannelMediaOptions());
        }

        public static void RenewToken(string newToken)
        {
            Engine.RenewToken(newToken);
        }

        public static void EnableSpatialAudio(bool isTrue)
        {
            Engine.EnableSoundPositionIndication(isTrue);
        }

        public static void AllowWebSdkInteroperability(bool isTrue)
        {
            Engine.EnableWebSdkInteroperability(isTrue);
        }

        public static void AllowAudioVolumeIndication(int interval, int smooth, bool isTrue)
        {
            Engine.EnableAudioVolumeIndication(interval, smooth, isTrue);
        }

        public static void AdjustVolume(int volume)
        {
            Engine.AdjustRecordingSignalVolume(volume);
        }

        public static void MuteOtherUser(uint uid, bool mute)
        {
            if (HasEngineError(Engine?.MuteRemoteAudioStream(uid, mute)))
            {
                "Failed to mute other user.".LogError();
            }
        }

        public static void SetVolume(int volume)
        {
            if (HasEngineError(Engine?.AdjustAudioMixingVolume(volume)))
            {
                "Failed to set mixing volume.".LogError();
            }

            if (HasEngineError(Engine?.AdjustPlaybackSignalVolume(volume)))
            {
                "Failed to set playback volume.".LogError();
            }
        }

        public static int GetVolume()
        {
            var result = Engine?.GetAudioMixingPlayoutVolume();

            if (result != null)
            {
                return HasEngineError(result) ? 0 : (int)result;
            }

            return 0;
        }

        public static void ForceSettings()
        {
            if (GetRTCEngine() != null)
            {
                GetRTCEngine().SetChannelProfile(CHANNEL_PROFILE.CHANNEL_PROFILE_COMMUNICATION);
                GetRTCEngine().SetClientRole(CLIENT_ROLE_TYPE.CLIENT_ROLE_AUDIENCE);
                GetRTCEngine().SetAudioProfile(AUDIO_PROFILE_TYPE.AUDIO_PROFILE_MUSIC_STANDARD_STEREO,
                        AUDIO_SCENARIO_TYPE.AUDIO_SCENARIO_GAME_STREAMING);
                GetRTCEngine().SetDefaultAudioRouteToSpeakerphone(true);
                GetRTCEngine().SetEnableSpeakerphone(true);
            }
        }

        private static bool HasEngineError(int? errorCode)
        {
            if (errorCode >= (int)ERROR_CODE.ERROR_OK)
            {
                return false;
            }

            Debug.LogError(errorCode switch
            {
                null => "Null Engine Reference.",
                (int)ERROR_CODE.ERROR_NOT_INIT_ENGINE => "Engine not initialized!",
                (int)ERROR_CODE.ERROR_NO_DEVICE_PLUGIN => "No Device Plugin",
                (int)ERROR_CODE.ERROR_INVALID_ARGUMENT => "Invalid Argument!",
                _ => $"Unhandled engine error. [Code: {errorCode}]"
            });

            return true;
        }
    }
}
