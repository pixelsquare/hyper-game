using Quantum;
using UnityEngine;

namespace Kumu.Kulitan.Hangout
{
    public class DisplacerObject : InteractPivot
    {
        [SerializeField] private Animation animation;
        
        private Transform modelTransform;
        private Transform originalFollow;
        
        private void OnInteract(EventOnRideInteract eventData)
        {
            if (eventData.entityObject != entityView.EntityRef)
            {
                return;
            }
                        
            if (eventData.isInteracting)
            {
                animation.Play();
            }
            else
            {
                animation.clip.SampleAnimation(gameObject, 0f);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            QuantumEvent.Subscribe<EventOnRideInteract>(this, OnInteract);
        }
    }
}
