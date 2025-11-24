using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Santelmo.Rinsurv
{
    public class JoystickController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [SerializeField] private float movementRange;
        [SerializeField] private float deadZone;
        [SerializeField] private RectTransform knob;
        [SerializeField] private RectTransform background;
        [SerializeField] private RectTransform inputArea;
        [SerializeField] private UnityEvent<Vector2> _onDrag;

        private Vector3 knobInitialPosition;
        private Vector3 backgroundInitialPosition;
        private Vector2 touchStartPosition;

        public delegate void OnJoystickDrag(Vector2 delta);

        public static event OnJoystickDrag _onJoystickDrag;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData == null)
            {
                throw new ArgumentNullException(nameof(eventData));
            }
            eventData.Use();
            
            RectTransformUtility.ScreenPointToLocalPointInRectangle(inputArea, eventData.position,
                eventData.pressEventCamera, out touchStartPosition);

            background.anchoredPosition = touchStartPosition;
            knob.anchoredPosition = touchStartPosition;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData == null)
            {
                throw new ArgumentNullException(nameof(eventData));
            }
            eventData.Use();

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(inputArea, eventData.position,
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

                _onJoystickDrag?.Invoke(delta);
                _onDrag.Invoke(delta);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData == null)
            {
                throw new ArgumentNullException(nameof(eventData));
            }
            eventData.Use();
            
            background.localPosition = backgroundInitialPosition;
            knob.localPosition = knobInitialPosition;
            _onJoystickDrag?.Invoke(Vector2.zero);
            _onDrag?.Invoke(Vector2.zero);
        }

        private void Start()
        {
            backgroundInitialPosition = background.localPosition;
            knobInitialPosition = knob.localPosition;
        }
    }
}
