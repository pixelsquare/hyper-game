using UnityEngine;

namespace Santelmo.Rinsurv
{
    public class SampleProjectile : MonoBehaviour, IProjectile
    {
        [SerializeField] private float _radius;
        [SerializeField] private LayerMask _hitLayers;
        
        public Vector2 Direction { get; set; }
        public float Distance { get; set; } = 4f;
        public float Speed { get; set; }
        public uint Damage { get; set; }
        public uint Pierce { get; set; }

        private void Update()
        {
            
            var delta = Direction * Speed;
            var origin = transform.position;
            var hit = Physics2D.CircleCast(origin, _radius, Direction, Speed, _hitLayers);
            
            if (hit)
            {
                OnHit(hit.transform);
                Destroy(gameObject);
            }
            
            transform.position += (Vector3) delta;
            
            if (Distance > 0)
            {
                Distance -= delta.magnitude;
            }
            else
            {
                Destroy(gameObject);
            }
            
        }

        public void OnHit(Transform hitTransform)
        {
            if (hitTransform.TryGetComponent<IHittable>(out var hittable))
            {
                var delta = Direction * Speed;
                hittable.Hit(Damage, transform.position);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _radius);
        }
    }
}
