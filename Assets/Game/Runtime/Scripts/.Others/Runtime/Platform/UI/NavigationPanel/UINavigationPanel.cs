using System;
using Kumu.Extensions;
using Kumu.Kulitan.Common;
using UnityEngine;
using UnityEngine.Events;

namespace Kumu.Kulitan.UI
{
    public class UINavigationPanel : MonoBehaviour
    {
        [Tooltip("Ensure that only one panel is set to true")]
        [SerializeField] private bool activateOnStart = false; // 
        
        [SerializeField] private string panelName = "PanelName";
        [SerializeField] private bool isActive = false;

        [SerializeField] private UnityEvent onActivated;
        [SerializeField] private UnityEvent onDeactivated;
        
        private UIPanelActivatedEvent activatedEvent;
        private UIPanelDeactivatedEvent deactivatedEvent;
        
        public void ActivatePanel()
        {
            if(isActive)
            {
                return;
            }
            isActive = true;
            GlobalNotifier.Instance.Trigger(activatedEvent);
            onActivated?.Invoke();
        }

        public void DeactivatePanel()
        {
            if (!isActive)
            {
                return;
            }
            isActive = false;
            GlobalNotifier.Instance.Trigger(deactivatedEvent);
            onDeactivated?.Invoke();
        }
        
        private void Awake()
        {
            activatedEvent = new UIPanelActivatedEvent(panelName);
            deactivatedEvent = new UIPanelDeactivatedEvent(panelName);
        }

        private void Start()
        {
            if (activateOnStart)
            {
                ActivatePanel();
            }
        }
    }
}
