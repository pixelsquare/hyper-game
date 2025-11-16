using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;
using Quantum;
using UnityEngine;

namespace Kumu.Kulitan.Hangout
{
    public class PropAudioHandler : MonoBehaviour
    {
        [SerializeField] private EntityView objectEntityView;
        [SerializeField] private AudioSource soundSource;

        void Start()
        {
            QuantumEvent.Subscribe<EventOnObjectInteracted>(this, OnObjectInteractedEvent);
        }

        private void OnObjectInteractedEvent(EventOnObjectInteracted callback)
        {
            if (callback.objInteracted == objectEntityView.EntityRef)
            {
                soundSource.Stop();
                soundSource.Play();
            }
        }

        protected virtual void OnDestroy()
        {
            QuantumEvent.UnsubscribeListener(this);
        }
    }
}
