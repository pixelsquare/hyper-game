using Kumu.Kulitan.Hangout;
using Quantum;
using UnityEngine;

namespace Kumu.Kulitan.Minigame
{
    public class FinishLineSfxListener : MonoBehaviour
    {
        [SerializeField] private EntityView entityView;
        [SerializeField] private AudioSource audioSource;

        private void OnFinishLineLocal(EventOnFinishLineLocal eventData)
        {
            audioSource.Play();
        }

        private void OnEnable()
        {
            QuantumEvent.Subscribe<EventOnFinishLineLocal>(this, OnFinishLineLocal);
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
