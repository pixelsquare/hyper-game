using System.Collections.Generic;
using Hangout;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;
using Quantum;
using UnityEngine;

namespace Kumu.Kulitan.Hangout
{
    public class InteractiveControllerUI : MonoBehaviour
    {
        [SerializeField] private GameObject gameUIObj;
        [SerializeField] private GameObject interactiveUIObj;

        [SerializeField] private Transform buttonParentT;
        [SerializeField] private InteractiveControllerButton interactiveButtonPrefab;

        [SerializeField] private EntityViewUpdater entityViewUpdater;

        private Slot<string> eventSlot;
        private EntityRef slotEntityRef;
        private InteractiveController interactiveController;
        private readonly StopInteractCommand stopInteractCommand = new();

        private readonly List<InteractiveControllerButton> interactiveButtonList = new();
        private readonly Dictionary<EntityRef, InteractiveController> interactiveObjectMap = new();

        public void StopInteracting()
        {
            stopInteractCommand.objectEntity = slotEntityRef;
            QuantumRunner.Default.Game.SendCommand(stopInteractCommand);
        }

        public void SetInteractiveUIActive(bool active)
        {
            gameUIObj.SetActive(!active);
            interactiveUIObj.SetActive(active);
        }

        private void HandlePlayerControllerInteract(IEvent<string> callback)
        {
            if (callback is not InteractiveControllerEventLocal controllerEvent)
            {
                return;
            }

            slotEntityRef = controllerEvent.slotEntityRef;
            var interactiveObjView = entityViewUpdater.GetView(slotEntityRef);

            if (!interactiveObjectMap.TryGetValue(slotEntityRef, out interactiveController))
            {
                interactiveController = interactiveObjView.GetComponentInParent<InteractiveController>();
                interactiveObjectMap.Add(slotEntityRef, interactiveController);
            }

            stopInteractCommand.objectEntity = slotEntityRef;
            SetInteractiveUIActive(controllerEvent.isInteracting);

            if (controllerEvent.isInteracting)
            {
                PopulateInteractiveButtons();
            }
            else
            {
                ResetAllButtons();
            }
        }

        private void PopulateInteractiveButtons()
        {
            foreach (var interactiveObj in interactiveController.InteractiveObjects)
            {
                if (interactiveObj == null)
                {
                    continue;
                }

                TryGetInteractiveButton(interactiveObj.ObjectGuid, out var interactiveButton);
;
                interactiveButton.Initialize(
                    interactiveObj.ObjectGuid,
                    interactiveObj.DisplayIcon,
                    () => interactiveObj.OnInteract(slotEntityRef));
            }
        }

        private void ResetAllButtons()
        {
            foreach (var interactiveButton in interactiveButtonList)
            {
                interactiveButton.Deinitialize();
            }
        }

        private bool TryGetInteractiveButton(AssetGuid objectGuid, out InteractiveControllerButton button)
        {
            foreach (var interactiveButton in interactiveButtonList)
            {
                if (interactiveButton.ObjectGuid.Equals(objectGuid) || !interactiveButton.gameObject.activeInHierarchy)
                {
                    button = interactiveButton;
                    return true;
                }
            }

            button = Instantiate(interactiveButtonPrefab, buttonParentT);
            interactiveButtonList.Add(button);
            return false;
        }

        private void Awake()
        {
            eventSlot = new Slot<string>(GlobalNotifier.Instance);
            eventSlot.SubscribeOn(InteractiveControllerEventLocal.EVENT_NAME, HandlePlayerControllerInteract);
        }

        private void OnDestroy()
        {
            eventSlot.Dispose();
        }   
    }
}
