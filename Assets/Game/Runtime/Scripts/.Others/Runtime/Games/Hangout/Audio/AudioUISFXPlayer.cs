using UnityEngine;

namespace Kumu.Kulitan.Hangout
{
    public class AudioUISFXPlayer : MonoBehaviour
    {
        [SerializeField] private AudioUISFXConfig sfxConfig;
        [SerializeField] private AudioSource[] sfxSources;
        private int nextAvailableSource;

        public void PlayUISFX(AudioUISFXConfig.SFXClipCode code)
        {
            if (sfxConfig.ClipDictionary.ContainsKey(code))
            {
                UseAvailableSource(sfxConfig.ClipDictionary[code]);
            }
        }

        public void PlayUISFXClip(AudioClip clip)
        {
            UseAvailableSource(clip);
        }

        private void UseAvailableSource(AudioClip clip)
        {
            sfxSources[nextAvailableSource].Stop();
            sfxSources[nextAvailableSource].PlayOneShot(clip);

            nextAvailableSource += 1;
            if (nextAvailableSource >= sfxSources.Length)
            {
                nextAvailableSource = 0;
            }
        }
    }
}
