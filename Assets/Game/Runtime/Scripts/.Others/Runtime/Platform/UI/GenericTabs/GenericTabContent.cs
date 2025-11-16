using UnityEngine;
using UnityEngine.Events;

namespace Kumu.Kulitan.UI
{
    public class GenericTabContent : MonoBehaviour
    {
        [SerializeField] private UnityEvent onPanelActivated;
        [SerializeField] private UnityEvent onPanelDeactivated;
        
        public void SetContentActive(bool isActive)
        {
            if (isActive)
            {
                onPanelActivated?.Invoke();
                return;
            }
            onPanelDeactivated?.Invoke();
        }
    }
}

