using System.Collections.Generic;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;
using Quantum;
using UnityEngine;
using UnityEngine.EventSystems;
using Input = UnityEngine.Input;
using LayerMask = UnityEngine.LayerMask;

namespace Kumu.Kulitan.Hangout
{
    public class InteractiveUIController : MonoBehaviour
    {
        [SerializeField] private AnimationConfig animConfig;
        [SerializeField] private Camera uiCamera;
        [SerializeField] private GameObject interactiveUIPrefab;
        
        private readonly Dictionary<EntityRef, InteractiveObjectWorldUI> entityUIFacerMap = new();
        private Slot<string> eventSlot;
        
        private PointerEventData m_PointerEventData;
        private EventSystem m_EventSystem;

        private void OnObjectInstantiated(IEvent<string> callback)
        {
            var objectInitEvent = (InteractiveObjectInitEvent) callback;

            if (!entityUIFacerMap.TryGetValue(objectInitEvent.EntityRef, out var intWorldUI))
            {
                intWorldUI = objectInitEvent.InteractiveObject.Transform
                    .GetComponentInChildren<InteractiveObjectWorldUI>();

                if (intWorldUI == null)
                {
                    intWorldUI = Instantiate(interactiveUIPrefab, objectInitEvent.InteractiveObject.Transform).GetComponentInChildren<InteractiveObjectWorldUI>();
                }

                entityUIFacerMap.Add(objectInitEvent.EntityRef, intWorldUI);
            }

            var f = QuantumRunner.Default.Game.Frames.Verified;
            var interactiveObject = f.Get<Quantum.InteractiveObject>(objectInitEvent.InteractiveObject.EntityRef);
            var animData = UnityDB.FindAsset<InteractableAnimationAsset>(interactiveObject.animDataGuid.id);
            var icon = animData != null
                ? animConfig.InteractableIconTags[animData.InteractableTag.tag]
                : animConfig.InteractableIconTags["default"];
            
            intWorldUI.Initialize(icon, objectInitEvent.InteractiveObject, uiCamera);

            // interactiveUI should be disabled/hidden by default
            intWorldUI.Hide();
        }

        private void OnPlayerInProximity(IEvent<string> callback)
        {
            var playerNearbyEvent = (PlayerNearInteractiveObjEvent) callback;

            if (entityUIFacerMap.TryGetValue(playerNearbyEvent.EntityRef, out var intWorldUI))
            {
                if (playerNearbyEvent.IsInProximity)
                {
                    intWorldUI.Show();
                }
                else
                {
                    intWorldUI.Hide();
                }
            }
        }

        private void Awake()
        {
            eventSlot = new Slot<string>(GlobalNotifier.Instance);
            eventSlot.SubscribeOn(InteractiveObjectInitEvent.EVENT_NAME, OnObjectInstantiated);
            eventSlot.SubscribeOn(PlayerNearInteractiveObjEvent.EVENT_NAME, OnPlayerInProximity);
        }

        private void OnDestroy()
        {
            eventSlot.Dispose();
        }

        private void OnGUI()
        {
            Event mEvent = Event.current;

            if (mEvent != null && mEvent.isMouse)
            {
                //Get Mouse Up
                if (mEvent.type == EventType.MouseUp)
                {
                    if (m_EventSystem == null)
                    {
                        m_EventSystem = FindObjectOfType<EventSystem>();
                    }

                    m_PointerEventData = new PointerEventData(m_EventSystem);
                    m_PointerEventData.position = Input.mousePosition;

                    var results = new List<RaycastResult>();

                    m_EventSystem.RaycastAll(m_PointerEventData, results);

                    foreach (RaycastResult result in results)
                    {
                        if (result.gameObject.layer == LayerMask.NameToLayer("InteractiveUI"))
                        {
                            if (result.gameObject.TryGetComponent<InteractiveObjectWorldUI>(out var intWorldUI))
                            {
                                intWorldUI.OnButtonClicked();
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}
