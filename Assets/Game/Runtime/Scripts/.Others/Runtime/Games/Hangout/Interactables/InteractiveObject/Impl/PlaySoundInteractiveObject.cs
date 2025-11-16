using UnityEngine;

namespace Kumu.Kulitan.Hangout
{
    [RequireComponent(typeof(AudioSource))]
    public class PlaySoundInteractiveObject : InteractiveObject
    {
        [Header("Settings")]
        [SerializeField] private AudioClip audioClip;
        [SerializeField] private float volume = 1.0f;

        private AudioSource audioSource;

        public override void Play()
        {
            var isLooping = interactiveType == InteractiveType.Looping;

            PlaySound(audioClip, volume, isLooping);
        }

        public override void Stop()
        {
            StopSound();
        }

        private void PlaySound(AudioClip clip, float volume, bool isLooping)
        {
            audioSource.clip = clip;
            audioSource.volume = volume;
            audioSource.loop = isLooping;
            audioSource.Stop();

            if (isLooping)
            {
                audioSource.Play();
            }
            else
            {
                audioSource.PlayOneShot(clip);
            }
        }

        private void StopSound()
        {
            audioSource.Stop();
        }

        protected override void Awake()
        {
            audioSource = gameObject.GetComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.outputAudioMixerGroup = AudioSettingsApplicator.Instance.SFXGroup;
        }
    }
}
