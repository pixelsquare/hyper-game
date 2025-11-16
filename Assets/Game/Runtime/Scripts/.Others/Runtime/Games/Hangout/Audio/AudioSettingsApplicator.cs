using Kumu.Kulitan.Common;
using UnityEngine;
using UnityEngine.Audio;

namespace Kumu.Kulitan.Hangout
{
    public class AudioSettingsApplicator : SingletonMonoBehaviour<AudioSettingsApplicator>
    {
        public const string BGM_VOLUME_KEY = "BgmVolumeKey";
        public const string SFX_VOLUME_KEY = "SfxVolumeKey";
        public const string VOICE_VOLUME_KEY = "VoiceVolumeKey";

        [SerializeField] private string bgmChannelVolumeParam;
        [SerializeField] private string sfxChannelVolumeParam;

        [SerializeField] private AudioMixer mixer;
        [SerializeField] private AudioMixerGroup bgmGroup;
        [SerializeField] private AudioMixerGroup sfxGroup;

        public AudioMixer Mixer => mixer;
        public AudioMixerGroup BGMGroup => bgmGroup;
        public AudioMixerGroup SFXGroup => sfxGroup;

        public void SetVolume(string key, float value)
        {
            if (key == VOICE_VOLUME_KEY)
            {
                SetVoiceVolume(value);
            }
            else
            {
                var param = GetParamByKey(key);
                SetMixerVolumeByParam(param, value);
            }
        }

        public void SetInitialVoiceVolume()
        {
            SetInitialVolume(VOICE_VOLUME_KEY);
        }

        private void SetInitialVolume(string key)
        {
            if (key == VOICE_VOLUME_KEY)
            {
                var percentage = 100f;

                if (PlayerPrefs.HasKey(VOICE_VOLUME_KEY))
                {
                    percentage = PlayerPrefs.GetFloat(VOICE_VOLUME_KEY, percentage);
                }

                SetVoiceVolume(percentage);
            }
            else
            {
                var param = GetParamByKey(key);

                if (PlayerPrefs.HasKey(key))
                {
                    SetMixerVolumeByParam(param, PlayerPrefs.GetFloat(key));
                }
                else
                {
                    SetMixerVolumeByParam(param, 1);
                }
            }
        }

        private void SetMixerVolumeByParam(string param, float value)
        {
            var finalAttenuation = Mathf.Log(value) * 20f;
            mixer.SetFloat(param, finalAttenuation);
        }

        private void SetVoiceVolume(float value)
        {
            if (!AudioVideoChatHelper.IsInitialized)
            {
                return;
            }

            AudioVideoChatHelper.SetVolume(Mathf.RoundToInt(value));
        }

        private string GetParamByKey(string key)
        {
            switch (key)
            {
                case BGM_VOLUME_KEY:
                    return bgmChannelVolumeParam;

                case SFX_VOLUME_KEY:
                    return sfxChannelVolumeParam;
            }

            return "";
        }

        private void ReapplySettings(bool devicewaschanged)
        {
            SetInitialVolume(BGM_VOLUME_KEY);
            SetInitialVolume(SFX_VOLUME_KEY);

            var handlers = FindObjectsOfType<AudioSourceHandler>();
            foreach (var ash in handlers)
            {
                if (ash.AudioType == AudioSourceHandler.AudioSourceType.Music)
                {
                    ash.Source.Play();
                }
            }

            if (AudioVideoChatHelper.GetRTCEngine() != null)
            {
                AudioVideoChatHelper.GetRTCEngine().SetDefaultAudioRouteToSpeakerphone(true);
                AudioVideoChatHelper.GetRTCEngine().SetEnableSpeakerphone(true);
            }
        }

        private void Start()
        {
            SetInitialVolume(BGM_VOLUME_KEY);
            SetInitialVolume(SFX_VOLUME_KEY);

            UnityEngine.AudioSettings.OnAudioConfigurationChanged += ReapplySettings;
        }

        private void OnDestroy()
        {
            UnityEngine.AudioSettings.OnAudioConfigurationChanged -= ReapplySettings;
        }
    }
}
