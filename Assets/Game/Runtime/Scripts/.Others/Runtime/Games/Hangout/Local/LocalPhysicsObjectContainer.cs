using UnityEngine;

namespace Kumu.Kulitan.Hangout
{
    [RequireComponent(typeof(BoxCollider))]
    public class LocalPhysicsObjectContainer : MonoBehaviour
    {
        private BoxCollider boxCollider = null;

        private void Awake()
        {
            boxCollider = GetComponent<BoxCollider>();
        }

        public bool IsLocalPhysicsObjectWithinBounds(Vector3 point)
        {
            return boxCollider.bounds.Contains(point);
        }
    }
}
