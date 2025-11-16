using UnityEngine;

namespace Kumu.Kulitan.Hangout
{
    public class AnimationLinkedAudio : AudioHandlerBase
    {
        [SerializeField] private AudioClip[] audioClips;
        [SerializeField] private OrderType orderType;
        private int nextClip;

        //todo - unused, but may be utilized down the line
        public AudioSource PlayAudio()
        {
            if (audioClips.Length == 0)
            {
                Debug.LogWarning($"No audio clip(s) specified for {gameObject.name}");
                return null;
            }

            switch (orderType)
            {
                case OrderType.Random:
                    return PlayAudioClip(audioClips[Random.Range(0, audioClips.Length)]);

                case OrderType.Sequential:
                    var clip = audioClips[nextClip];

                    nextClip += 1;
                    if (nextClip >= audioClips.Length)
                    {
                        nextClip = 0;
                    }
                    return PlayAudioClip(clip);;
                
                default:
                    Debug.LogError($"{gameObject.name}[AnimationLinkedAudio]: Unrecognized order type.");
                    return null;
            }
        }

        public AudioSource PlayAudioByClip(AudioClip clip, float volumeOverride = 1f)
        {
            if (clip == null)
            {
                return null;
            }

            return PlayAudioClip(clip, volumeOverride);
        }

        //todo - unused, but may be utilized down the line
        public AudioSource PlayAudioByIndex(int index)
        {
            return PlayAudioClip(audioClips[index]);
        }

        //todo - unused, but may be utilized down the line
        public void ResetCounter()
        {
            nextClip = 0;
        }

        public enum OrderType
        {
            Random,
            Sequential
        }
    }
}
