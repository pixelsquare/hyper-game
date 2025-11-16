using Quantum;
using UnityEngine;

namespace Kumu.Kulitan.Hangout
{
    [RequireComponent(typeof(InteractiveCameraActivator))]
    public class InteractiveObjectCameraListener : MonoBehaviour
    {
        [SerializeField] private InteractiveCameraActivator cameraActivator;
        [SerializeField] private EntityView entityView;

        private void OnObjectInteractedLocal(EventOnObjectInteractedLocal eventData)
        {
            if (entityView.EntityRef == eventData.objInteracted
                && cameraActivator)
            {
                cameraActivator.SetCameraActive(eventData.isInteracting);
            }
        }

        private void Awake()
        {
            if (cameraActivator == null)
            {
                cameraActivator = GetComponent<InteractiveCameraActivator>();
            }

            if (entityView == null)
            {
                entityView = GetComponentInParent<EntityView>();
            }
        }

        private void OnEnable()
        {
            QuantumEvent.Subscribe<EventOnObjectInteractedLocal>(this, OnObjectInteractedLocal);
        }

        private void OnDisable()
        {
            QuantumEvent.UnsubscribeListener(this);
        }
    }
}
