using UnityEngine;

namespace Santelmo.Rinsurv
{
    public class EnemyWaypointBasic : MonoBehaviour, IAimDirection
    {
        private IMovement _movement;        
        private Transform _target;

        public Vector2 AimDirection { get; set; } = Vector2.up;

        private void Update()
        {
            AimDirection = _movement.Delta = (_target.position - transform.position).normalized;
        }

        private void Start()
        {
            _target = EnemyTarget.Instance.transform; // todo: convert to DI
        }

        private void Awake()
        {
            _movement = GetComponent<IMovement>();
        }
    }
}
