using Quantum;
using UnityEngine;
using UnityEngine.Events;

namespace Kumu.Kulitan.Interactables
{
    public class InteractiveObjectViewEvent : MonoBehaviour
    {
        [SerializeField] private EntityView entityView;
        [SerializeField] private UnityEvent onInteractStart;
        [SerializeField] private UnityEvent onInteractStop;
        [SerializeField] private UnityEvent<bool> onInteractToggle;
        [SerializeField] private UnityEvent<bool> onInteractToggleNot;

        private void OnInteract(EventOnObjectInteracted eventData)
        {
            if (eventData.objInteracted != entityView.EntityRef)
            {
                return;
            }
            
            var isInteracting = eventData.isInteracting;
            onInteractToggle.Invoke(isInteracting);
            onInteractToggleNot.Invoke(!isInteracting);

            if (isInteracting)
            {
                onInteractStart.Invoke();
            }
            else
            {
                onInteractStop.Invoke();
            }
        }

        private void OnEnable()
        {
            QuantumEvent.Subscribe<EventOnObjectInteracted>(this, OnInteract);
        }

        private void OnDisable()
        {
            QuantumEvent.UnsubscribeListener(this);
        }
    }
}
