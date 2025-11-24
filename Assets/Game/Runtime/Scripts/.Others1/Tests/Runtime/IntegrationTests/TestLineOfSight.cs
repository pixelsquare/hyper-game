using UnityEngine;

namespace Santelmo.Rinsurv.Tests
{
    public class TestDot : MonoBehaviour
    {
        [SerializeField] private Transform _a;
        [SerializeField] private Transform _b;
        [SerializeField] private float _angle;
    
        private bool InsideLineOfSight(Transform x, Transform y)
        {
            var direction = (y.position - x.position).normalized;
            var dot = Vector2.Dot(x.up, direction);
            var cone = Mathf.Cos(_angle * Mathf.Deg2Rad / 2f);
            return dot > cone;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            foreach (var direction in ProjectileUtility.GetConeDirections(_a.up, 2, _angle))
            {
                Gizmos.DrawRay(_a.position, direction * 10f);
            }
                
            Gizmos.color = Color.cyan;
            foreach (var direction in ProjectileUtility.GetConeDirections(_b.up, 2, _angle))
            {
                Gizmos.DrawRay(_b.position, direction * 10f);
            }

            Gizmos.color = InsideLineOfSight(_a, _b) ? Color.green : Color.red;
            Gizmos.DrawSphere(_b.position, .25f);
        
            Gizmos.color = InsideLineOfSight(_b, _a) ? Color.green : Color.red;
            Gizmos.DrawSphere(_a.position, .25f);
        }
    }
}
