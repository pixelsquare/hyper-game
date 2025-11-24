using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Santelmo.Rinsurv
{
    public class LegacySelectCard : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private RinawaText _label;

        public UnityAction _onSelect;

        public void Init(string label, UnityAction onSelect)
        {
            _label.text = label;
            _onSelect = onSelect;
            gameObject.SetActive(true);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _onSelect.Invoke();
        }
    }
}
