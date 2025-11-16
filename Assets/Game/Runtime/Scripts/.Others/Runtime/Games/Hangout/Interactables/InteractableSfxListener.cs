using Kumu.Kulitan.Hangout;
using Quantum;
using UnityEngine;

namespace Kumu.Kulitan.Avatar
{
    public class InteractableSfxListener : MonoBehaviour
    {
        [SerializeField] private EntityView entityView;
        [SerializeField] private AudioSource audioSource;

        private void OnObjectInteracted(EventOnObjectInteracted eventData)
        {
            if (eventData.isInteracting
                && eventData.objInteracted == entityView.EntityRef)
            {
                audioSource.Play();
            }
        }

        private void OnEnable()
        {
            QuantumEvent.Subscribe<EventOnObjectInteracted>(this, OnObjectInteracted);
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
