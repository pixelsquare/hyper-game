using UnityEngine;

namespace Kumu.Kulitan.Hangout
{
    [RequireComponent(typeof(Rigidbody))]
    public class LocalPhysicsObject : MonoBehaviour
    {
        private Rigidbody rb = null;

        [SerializeField] private LocalPhysicsObjectContainer localPhysicsObjectContainer = null;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void Update()
        {
            if (localPhysicsObjectContainer != null && !localPhysicsObjectContainer.IsLocalPhysicsObjectWithinBounds(rb.position))
            {
                rb.Sleep();
                rb.MovePosition(localPhysicsObjectContainer.transform.position);
                rb.velocity = Vector3.zero;
            }
        }
    }
}
