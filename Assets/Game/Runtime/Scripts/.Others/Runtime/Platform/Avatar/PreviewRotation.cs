using UnityEngine;
using UnityEngine.EventSystems;

namespace Kumu.Kulitan.Avatar
{
    /// <summary>
    /// Rotates an object based on input.
    /// Must be attached to an object with a collider.
    /// Scene camera must be using a PhysicsRaycaster.
    /// </summary>
    public class PreviewRotation : MonoBehaviour, IDragHandler
    {
        [SerializeField] private float rotationSpeed = 5f;

        public void OnDrag(PointerEventData eventData)
        {
            float xAxisRotation = eventData.delta.x * rotationSpeed;
            transform.Rotate(Vector3.up, -xAxisRotation);
        }        
    }
}
