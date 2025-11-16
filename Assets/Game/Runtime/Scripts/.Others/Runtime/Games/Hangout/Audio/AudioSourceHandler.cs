using UnityEngine;

namespace Kumu.Kulitan.Hangout
{
    public class AudioSourceHandler : MonoBehaviour
    {
        [SerializeField] private AudioSource mySource;
        [SerializeField] private AudioSourceType myType;

        public AudioSource Source => mySource;
        public AudioSourceType AudioType => myType;

        public enum AudioSourceType
        {
            Music,
            SoundEffect,
        }

        private void Start()
        {
            switch (myType)
            {
                case AudioSourceType.Music:
                    mySource.outputAudioMixerGroup = AudioSettingsApplicator.Instance.BGMGroup;
                    break;

                case AudioSourceType.SoundEffect:
                    mySource.outputAudioMixerGroup = AudioSettingsApplicator.Instance.SFXGroup;
                    break;
            }
        }
    }
}
