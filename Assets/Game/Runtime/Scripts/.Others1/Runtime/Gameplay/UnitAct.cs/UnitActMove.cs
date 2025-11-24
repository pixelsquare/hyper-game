using UnityEngine;

namespace Santelmo.Rinsurv
{
    public class UnitBossMove : IUnitAct, IUnitActUpdate, IUnitActEnd
    {
        private readonly IMovement _movement;
        private readonly IAimDirection _aimDirection;

        private readonly Transform _target;
        private readonly Transform _origin;
        private readonly float _proximityThreshold;

        public UnitBossMove(float proximityThreshold, Transform origin, Transform target, IMovement movement, IAimDirection aimDirection)
        {
            _proximityThreshold = proximityThreshold;            
            _origin = origin;
            _target = target;
            _aimDirection = aimDirection;
            _movement = movement;
        }

        public bool IsFinished()
        {
            return Vector2.Distance(_target.position, _origin.position) < _proximityThreshold;
        }

        public void OnUnitActUpdate()
        {
            _aimDirection.AimDirection = _movement.Delta = (_target.position - _origin.position).normalized;
        }

        public void OnUnitActEnd()
        {
            _movement.Delta = Vector2.zero;
        }
    }
}
