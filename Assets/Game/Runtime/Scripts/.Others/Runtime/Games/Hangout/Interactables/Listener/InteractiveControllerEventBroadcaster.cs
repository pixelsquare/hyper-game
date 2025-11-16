using Kumu.Kulitan.Common;
using Quantum;
using UnityEngine;

namespace Kumu.Kulitan.Hangout
{
    public class InteractiveControllerEventBroadcaster : MonoBehaviour
    {
        [SerializeField] private EntityView entityView;
        
        private void OnObjectInteracted(EventOnObjectInteracted eventData)
        {
            if (entityView.EntityRef != eventData.objInteracted)
            {
                return;
            }
            
            var notifierData = new InteractiveControllerEvent()
            {
                isInteracting = eventData.isInteracting,
                slotEntityRef = eventData.objInteracted,
            };
            
            GlobalNotifier.Instance.Trigger(notifierData);
        }

        private void OnObjectInteractedLocal(EventOnObjectInteractedLocal eventData)
        {
            if (entityView.EntityRef != eventData.objInteracted)
            {
                return;
            }
            
            var notifierData = new InteractiveControllerEventLocal()
            {
                isInteracting = eventData.isInteracting,
                slotEntityRef = eventData.objInteracted,
            };

            GlobalNotifier.Instance.Trigger(notifierData);
        }
        
        private void OnEnable()
        {
            QuantumEvent.Subscribe<EventOnObjectInteracted>(this, OnObjectInteracted);
            QuantumEvent.Subscribe<EventOnObjectInteractedLocal>(this, OnObjectInteractedLocal);
        }

        private void OnDisable()
        {
            QuantumEvent.UnsubscribeListener(this);
        }
    }
}
