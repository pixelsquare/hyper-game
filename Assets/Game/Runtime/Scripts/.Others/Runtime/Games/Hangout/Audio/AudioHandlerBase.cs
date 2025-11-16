using UnityEngine;

namespace Kumu.Kulitan.Hangout
{
    public class AudioHandlerBase : MonoBehaviour
    {
        [SerializeField] private AudioSource[] sources;
        private int nextSource;

        protected AudioSource PlayAudioClip(AudioClip clip, float volumeOverride = 1f)
        {
            if (sources.Length == 0)
            {
                Debug.LogWarning($"No audio source(s) specified for {gameObject.name}");
                return null;
            }

            var activeSource = sources[nextSource];

            activeSource.Stop();
            activeSource.volume = volumeOverride;
            activeSource.clip = clip;
            activeSource.Play();

            nextSource++;

            if (nextSource >= sources.Length)
            {
                nextSource = 0;
            }

            return activeSource;
        }
    }
}
