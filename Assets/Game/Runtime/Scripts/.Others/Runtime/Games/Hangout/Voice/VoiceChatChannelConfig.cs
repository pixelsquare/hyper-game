using agora_gaming_rtc;
using Kumu.Kulitan.Common;
using UnityEngine;

namespace Kumu.Kulitan.Hangout
{
    public class VoiceChatChannelConfig : MonoBehaviour
    {
        private void SetVoiceChannelAudioConfig()
        {
            //Will allow switching from Loudspeaker or bluetooth earbuds
            AudioVideoChatHelper.AllowAudioVolumeIndication(200, 4, true);
            //AudioVideoChatHelper.PlayAudioOnSpeakerphone(true);
            // Taken from: https://docs.agora.io/en/Voice/faq/no_music_Unity_objects
            AudioVideoChatHelper.GetRTCEngine().SetParameters("{\"che.audio.keep.audiosession\":true}");
        }

        private void SetVoiceChannelProfile()
        {
            AudioVideoChatHelper.GetRTCEngine().SetChannelProfile(CHANNEL_PROFILE.CHANNEL_PROFILE_COMMUNICATION);
            AudioVideoChatHelper.GetRTCEngine().SetClientRole(CLIENT_ROLE_TYPE.CLIENT_ROLE_AUDIENCE);
            AudioVideoChatHelper.GetRTCEngine().SetAudioProfile(AUDIO_PROFILE_TYPE.AUDIO_PROFILE_MUSIC_STANDARD_STEREO,
                AUDIO_SCENARIO_TYPE.AUDIO_SCENARIO_GAME_STREAMING);
            AudioVideoChatHelper.GetRTCEngine().SetDefaultAudioRouteToSpeakerphone(true);
            AudioVideoChatHelper.GetRTCEngine().SetEnableSpeakerphone(true);
        }

        private void Start()
        {
            AudioVideoChatHelper.GetRTCEngine().OnAudioRouteChanged += CheckRoute;
            
            SetVoiceChannelProfile();
            SetVoiceChannelAudioConfig();
        }

        private void CheckRoute(AUDIO_ROUTE route)
        {
            AudioVideoChatHelper.GetRTCEngine().SetEnableSpeakerphone(true);
        }
    }
}
