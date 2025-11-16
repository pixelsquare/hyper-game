using Kumu.Kulitan.Hangout;
using Quantum;
using UnityEngine;

namespace Kumu.Kulitan.Minigame
{
    public class BoulderSfxListener : MonoBehaviour
    {
        [SerializeField] private EntityView entityView;
        [SerializeField] private AudioSource audioSource;

        private void OnBoulderhit(EventOnBoulderHit eventData)
        {
            if (eventData.boulderEntity == entityView.EntityRef)
            {
                audioSource.Play();
            }
        }

        private void OnEnable()
        {
            QuantumEvent.Subscribe<EventOnBoulderHit>(this, OnBoulderhit);
        }

        private void OnDisable()
        {
            QuantumEvent.UnsubscribeListener(this);
        }
        
        private void Start()
        {
            audioSource.outputAudioMixerGroup = AudioSettingsApplicator.Instance.SFXGroup;
        }
    }
}
