using Hangout;
using Kumu.Kulitan.Common;
using Quantum;
using UnityEngine;

namespace Kumu.Kulitan.Hangout
{
    [RequireComponent(typeof(EntityComponentInteractiveObject))]
    public class InteractiveObjectSlot : MonoBehaviour, IInteractiveObject
    {
        [SerializeField] private EntityView entityView;

        public Transform Transform => transform;
        public EntityRef EntityRef => entityView.EntityRef;

#if UNITY_EDITOR
        private EntityComponentInteractiveObject interactiveObjectComponent;
#endif

        private InteractiveObjectView interactiveObjectView;
        private readonly StartInteractCommand interactCommand = new();

        public void OnTryInteract()
        {
            QuantumRunner.Default.Game.SendCommand(interactCommand);
        }

        public bool IsAvailable()
        {
            if (QuantumRunner.Default == null || !QuantumRunner.Default.IsRunning)
            {
                return false;
            }

            var f = QuantumRunner.Default.Game.Frames.Verified;
            var interactiveObject = f.Get<Quantum.InteractiveObject>(entityView.EntityRef);
            return interactiveObject.isAvailable;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (interactiveObjectComponent == null)
            {
                interactiveObjectComponent = GetComponent<EntityComponentInteractiveObject>();
            }

            Gizmos.DrawIcon(transform.position + interactiveObjectComponent.Prototype.exitPoint.ToUnityVector3(), "sv_icon_dot9_pix16_gizmo");
        }
#endif

        private void HandleEntityInitialized(QuantumGame game)
        {
            interactCommand.objectEntity = entityView.EntityRef;
            GlobalNotifier.Instance.Trigger(new InteractiveObjectInitEvent(interactiveObjectView.ModelEntityRef, this));
            entityView.OnEntityInstantiated.RemoveListener(HandleEntityInitialized);
        }       

        private void Awake()
        {
            interactiveObjectView = GetComponentInParent<InteractiveObjectView>();
        }

        private void OnEnable()
        {
            entityView.OnEntityInstantiated.AddListener(HandleEntityInitialized);
        }

        private void OnDisable()
        {
            QuantumEvent.UnsubscribeListener(this);
        }
    }
}
