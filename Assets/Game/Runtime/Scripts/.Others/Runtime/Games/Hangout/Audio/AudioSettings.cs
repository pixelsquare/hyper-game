using UnityEngine;
using UnityEngine.UI;

namespace Kumu.Kulitan.Hangout
{
    public class AudioSettings : MonoBehaviour
    {
        [SerializeField] private Slider bgmSlider;
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private Slider micSlider;

        public void SaveToPrefs()
        {
            PlayerPrefs.SetFloat(AudioSettingsApplicator.BGM_VOLUME_KEY, bgmSlider.value);
            PlayerPrefs.SetFloat(AudioSettingsApplicator.SFX_VOLUME_KEY, sfxSlider.value);
            PlayerPrefs.SetFloat(AudioSettingsApplicator.VOICE_VOLUME_KEY, micSlider.value);

            AudioSettingsApplicator.Instance.SetVolume(AudioSettingsApplicator.BGM_VOLUME_KEY, bgmSlider.value);
            AudioSettingsApplicator.Instance.SetVolume(AudioSettingsApplicator.SFX_VOLUME_KEY, sfxSlider.value);
            AudioSettingsApplicator.Instance.SetVolume(AudioSettingsApplicator.VOICE_VOLUME_KEY, micSlider.value);
        }

        private void ReadCurrentSettings()
        {
            ProcessKey(AudioSettingsApplicator.BGM_VOLUME_KEY, bgmSlider);
            ProcessKey(AudioSettingsApplicator.SFX_VOLUME_KEY, sfxSlider);
            ProcessKey(AudioSettingsApplicator.VOICE_VOLUME_KEY, micSlider);
        }

        private void ProcessKey(string key, Slider pairedSlider)
        {
            if (PlayerPrefs.HasKey(key))
            {
                pairedSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat(key));
            }
            else
            {
                if (key == AudioSettingsApplicator.VOICE_VOLUME_KEY)
                {
                    pairedSlider.SetValueWithoutNotify(100);
                    PlayerPrefs.SetFloat(key, 100);
                }
                else
                {
                    pairedSlider.SetValueWithoutNotify(1);
                    PlayerPrefs.SetFloat(key, 1);
                }
            }
        }

        private void Start()
        {
            ReadCurrentSettings();
        }
    }
}
