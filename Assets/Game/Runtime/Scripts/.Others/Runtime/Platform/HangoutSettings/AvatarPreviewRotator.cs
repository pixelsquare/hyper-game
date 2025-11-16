using UnityEngine;
using UnityEngine.EventSystems;

namespace Kumu.Kulitan.UI
{
    public class AvatarPreviewRotator : MonoBehaviour, IDragHandler
    {
        [SerializeField] private float rotationSpeedX = 15f;
        [SerializeField] private Transform avatarTransform;

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.dragging)
            {
                avatarTransform.Rotate(Vector3.up * rotationSpeedX * -eventData.delta.x * Time.deltaTime);
            }
        }
    }
}
