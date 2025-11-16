using UnityEngine;
using UnityEngine.EventSystems;

namespace Kumu.Kulitan.Common
{
    public class JoystickController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [SerializeField] private VectorData valueMapping;
        [SerializeField] private float movementRange;
        [SerializeField] private float deadZone;
        [SerializeField] private RectTransform knob;
        [SerializeField] private RectTransform background;

        private Vector3 knobInitialPosition;
        private Vector3 backgroundInitialPosition;
        private Vector2 touchStartPosition;

        private RectTransform cachedRectTransform;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData == null)
            {
                throw new System.ArgumentNullException(nameof(eventData));
            }
            eventData.Use();
            
            RectTransformUtility.ScreenPointToLocalPointInRectangle(cachedRectTransform, eventData.position,
                eventData.pressEventCamera, out touchStartPosition);

            background.anchoredPosition = touchStartPosition;
            knob.anchoredPosition = touchStartPosition;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData == null)
            {
                throw new System.ArgumentNullException(nameof(eventData));
            }
            eventData.Use();

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(cachedRectTransform, eventData.position,
                eventData.pressEventCamera, out var dragPosition))
            {
                var delta = dragPosition - touchStartPosition;
                delta = Vector2.ClampMagnitude(delta, movementRange);
                knob.anchoredPosition = touchStartPosition + delta;
                delta.x = Mathf.InverseLerp(0, movementRange, Mathf.Abs(delta.x)) * Mathf.Sign(delta.x);
                delta.y = Mathf.InverseLerp(0, movementRange, Mathf.Abs(delta.y)) * Mathf.Sign(delta.y);
                if (delta.magnitude <= deadZone)
                {
                    delta = Vector2.zero;
                }
                valueMapping.SetValue(delta);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData == null)
            {
                throw new System.ArgumentNullException(nameof(eventData));
            }
            eventData.Use();
            
            background.localPosition = backgroundInitialPosition;
            knob.localPosition = knobInitialPosition;
            valueMapping.SetValue(Vector2.zero);
        }

        private void Awake()
        {
            backgroundInitialPosition = background.localPosition;
            knobInitialPosition = knob.localPosition;
            cachedRectTransform = GetComponent<RectTransform>();
            
            valueMapping.Reset();
        }
    }
}
