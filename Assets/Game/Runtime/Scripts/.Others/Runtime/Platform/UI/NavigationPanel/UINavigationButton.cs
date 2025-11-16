using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Kumu.Kulitan.UI
{ 
    public class UINavigationButton : MonoBehaviour
    {
        [SerializeField] private bool isActive = false;     // < checks if button is active in scene
        [SerializeField] private UINavigationPanel navPanel;
        [SerializeField] private UINavigationController navController;

        [Header("Unity Events")]
        [SerializeField] private UnityEvent onActivated;
        [SerializeField] private UnityEvent onDeactivated;

        public UINavigationPanel NavPanel => navPanel;
        public Button NavButton => GetComponent<Button>();
        
        public void ActivateButton()
        {
            if (isActive)
            {
                return;
            }
            isActive = true;
            onActivated?.Invoke();
            navController.NavPanelActivated(this);
        }

        public void DeactivateButton()
        {
            if (!isActive)
            {
                return;
            }
            isActive = false;
            onDeactivated?.Invoke();
        }

        private void OnDestroy()
        {
            NavButton.onClick.RemoveAllListeners();
        }
    }
}
