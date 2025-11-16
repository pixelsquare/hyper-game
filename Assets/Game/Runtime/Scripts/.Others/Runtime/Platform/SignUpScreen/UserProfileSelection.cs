using Kumu.Kulitan.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Kumu.Kulitan.UI
{
    [RequireComponent(typeof(Button))]
    public class UserProfileSelection : MonoBehaviour
    {
        [SerializeField] private bool isSelected = false;
        [SerializeField] private GameObject iconHighlight;

        private Button button;
        private UserProfileSelectionClickedEvent clickedEvent;
        
        public void ToggleSelect(bool isButtonSelected)
        {
            isSelected = isButtonSelected;
            iconHighlight.SetActive(isSelected);
        }

        private void OnButtonClicked()
        {
            GlobalNotifier.Instance.Trigger(clickedEvent);
        }

        private void OnEnable()
        {
            button.onClick.AddListener(OnButtonClicked);
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(OnButtonClicked);
        }

        private void Awake()
        {
            clickedEvent = new UserProfileSelectionClickedEvent(this);
            button = GetComponent<Button>();
        }
    }
}
