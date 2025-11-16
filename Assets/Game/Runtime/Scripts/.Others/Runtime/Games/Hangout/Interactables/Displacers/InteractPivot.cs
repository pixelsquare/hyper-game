using Cinemachine;
using Quantum;
using UnityEngine;

namespace Kumu.Kulitan.Hangout
{
    public class InteractPivot : MonoBehaviour
    {
        [SerializeField] private Transform pivot;
        [SerializeField] protected EntityView entityView;
        [SerializeField] private EntityViewUpdater entityViewUpdater;
        [SerializeField] private CinemachineFreeLook followCamera;
        
        private Transform modelTransform;
        private Transform originalFollow;

        public void Init(QuantumGame quantumGame)
        {
            var f = quantumGame.Frames.Verified;

            if (!f.TryGet<Quantum.InteractiveObject>(entityView.EntityRef, out var interactiveObject))
            {
                return;
            }

            if (interactiveObject.isAvailable)
            {
                return;
            }

            var initData = new EventOnObjectInteracted()
            {
                objInteracted = entityView.EntityRef,
                playerEntity = interactiveObject.interactingPlayer,
                isInteracting = true
            };
            
            OnInteract(initData);
        }
        
        public void OnInteract(EventOnObjectInteracted eventData)
        {
            if (entityView.EntityRef != eventData.objInteracted)
            {
                return;
            }

            if (eventData.isInteracting)
            {
                OnStartInteract(eventData);
            }
            else
            {
                OnStopInteract(eventData);
            }
        }

        private void OnInteractLocal(EventOnObjectInteractedLocal eventData)
        {
            if (entityView.EntityRef != eventData.objInteracted)
            {
                return;
            }

            if (eventData.isInteracting)
            {
                originalFollow = followCamera.Follow;
                followCamera.Follow = modelTransform;
                followCamera.LookAt = modelTransform;
            }
            else
            {
                followCamera.Follow = originalFollow;
                followCamera.LookAt = originalFollow;
            }
        }

        private void OnStartInteract(EventOnObjectInteracted eventData)
        {
            var actorView = entityViewUpdater.GetView(eventData.playerEntity);
            if (actorView == null)
            {
                return;
            }

            // todo: replace with a quicker way to get the player model
            modelTransform = actorView.GetComponent<PlayerView>().ModelTransform; 
                
            modelTransform.SetParent(pivot);
            modelTransform.localPosition = Vector3.zero;
            modelTransform.localRotation = Quaternion.identity;            
        }
        
        private void OnStopInteract(EventOnObjectInteracted eventData)
        {
            var actorView = entityViewUpdater.GetView(eventData.playerEntity);

            var f = QuantumRunner.Default.Game.Frames.Verified;
            
            if (!actorView || !f.Exists(eventData.playerEntity))
            {
                Destroy(modelTransform.gameObject);
            }
            else
            {
                modelTransform.SetParent(actorView.transform);
                modelTransform.localPosition = Vector3.zero;
                modelTransform.localRotation = Quaternion.identity;
            }
            
            modelTransform = null;
        }
        
        private void Start()
        {
            followCamera = FindObjectOfType<CinemachineFreeLook>();
            entityViewUpdater = FindObjectOfType<EntityViewUpdater>();
        }
        
        protected virtual void OnEnable()
        {
            QuantumEvent.Subscribe<EventOnObjectInteracted>(this, OnInteract);
            QuantumEvent.Subscribe<EventOnObjectInteractedLocal>(this, OnInteractLocal);
        }

        protected virtual void OnDisable()
        {
            QuantumEvent.UnsubscribeListener(this);
        }
    }
}
