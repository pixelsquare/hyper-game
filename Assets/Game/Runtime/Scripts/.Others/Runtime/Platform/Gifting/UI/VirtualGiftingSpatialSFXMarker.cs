using Kumu.Kulitan.Hangout;
using UnityEngine;

namespace Kumu.Kulitan.Gifting
{
    public class VirtualGiftingSpatialSFXMarker : MonoBehaviour
    {
        [SerializeField] private AudioSource mySource;

        public AudioSource MySource => mySource;

        private void Start()
        {
            mySource.outputAudioMixerGroup = AudioSettingsApplicator.Instance.SFXGroup;
        }
    }
}
