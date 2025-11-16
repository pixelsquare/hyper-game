using UnityEngine;
using UnityEngine.EventSystems;

namespace Kumu.Kulitan.Common
{
    public class TouchController : MonoBehaviour, IDragHandler
    {
        [SerializeField] private VectorData valueMapping;
        private bool hasDrag;
        private Vector2 touchDelta;
        
        public void OnDrag(PointerEventData eventData)
        {
            if (eventData == null)
            {
                throw new System.ArgumentNullException(nameof(eventData));
            }
            
            eventData.Use();
            touchDelta = eventData.delta;

            hasDrag = true;
        }

        private void LateUpdate()
        {
            if(hasDrag)
            {
                valueMapping.SetValue(touchDelta);
                hasDrag = false;
            }
            else
            {
                valueMapping.SetValue(Vector2.zero);
            }
        }
    }
}
