using UnityEngine;

namespace Santelmo.Rinsurv
{
    [RequireComponent(typeof(Collider2D))]
    public class QuadtreeCollisionObject : MonoBehaviour
    {
        public void Awake()
        {
            QuadTreeInstance.Quadtree.Insert(gameObject);
        }

        public void OnDestroy()
        {
            QuadTreeInstance.Quadtree.Remove(gameObject);
        }
    }
}
