using UnityEngine;

namespace Santelmo.Rinsurv
{
    public class UnitBossAoeAttack : IUnitAct, IUnitActStart, IUnitActUpdate, IUnitActDuration
    {   
        private readonly float _radius;
        private readonly float _duration;
        private readonly IAimDirection _aimDirection;
        private readonly ContactFilter2D _contactFilter;

        private float _elapse;

        public float UnitActDuration => _duration;

        public bool IsFinished()
        {
            return _elapse < 0f;
        }

        public UnitBossAoeAttack(float duration, float radius, IAimDirection aimDirection, ContactFilter2D contactFilter)
        {
            _radius = radius;
            _duration = duration;           
            _aimDirection = aimDirection;
            _contactFilter = contactFilter;
        }

        public void OnUnitActStart()
        {
            _elapse = _duration;

            var results = new Collider2D[10];
            var hits = Physics2D.OverlapCircle(_aimDirection.AimDirection, _radius, _contactFilter, results);

            for (var i = 0; i < hits; i++)
            {
                var hit = results[i];
                
                if (!hit)
                {
                    Debug.Log("hit nothing");
                    return;
                }
                
                if (!hit.TryGetComponent<Health>(out var contactHealth))
                {
                    Debug.Log("unit has no health");
                    return;
                }                 
                     
                if (!contactHealth.IsHittable)
                {
                    Debug.Log("unit is unhittable");
                    return;
                }

                contactHealth.Hit(20, _aimDirection.AimDirection); //TODO: apply actual damage value                
            }
        }

        public void OnUnitActUpdate()
        {
            _elapse -= Time.deltaTime;
        }
    }
}
