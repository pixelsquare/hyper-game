using Quantum;
using UnityEngine;

namespace Kumu.Kulitan.Hangout
{
    [RequireComponent(typeof(PlayAnimationInteractiveObject))]
    public class InteractiveToggleInitializer : MonoBehaviour
    {
        [SerializeField] private EntityView controllerEntityView;
        [SerializeField] private PlayAnimationInteractiveObject interactiveObject;
        [SerializeField] private InteractPivot interactPivot;
        
        public void Init()
        {
            var controllerEntityRef = controllerEntityView.EntityRef;

            var f = QuantumRunner.Default.Game.Frames.Verified;

            if (f.TryGet<InteractiveToggle>(controllerEntityRef, out var interactiveToggle)
                && interactiveToggle.objGuid == interactiveObject.ObjectGuid
                && interactiveToggle.isOn)
            {
                interactiveObject.Play();
            }
        }

        private void OnValidate()
        {
            interactiveObject = GetComponent<PlayAnimationInteractiveObject>();
        }
    }
}
