using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Kumu.Kulitan.Avatar
{ 
    public class MainPartIcon : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private MainPartSelection mainPartSelection;
        [SerializeField] private PartSelectionScriptableObject partsCollection;
        [SerializeField] private UnityEvent onClicked;

        public PartSelectionScriptableObject PartsCollection => partsCollection;

        public void Toggle(bool isOn)
        {
            if (isOn)
            {
                mainPartSelection.Select(this);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            onClicked?.Invoke();
        }
    }
}
