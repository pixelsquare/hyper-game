using UnityEngine;

namespace Santelmo.Rinsurv
{
    public class UnitBossAoeWindup : IUnitAct, IUnitActStart, IUnitActUpdate, IUnitActEnd, IUnitActDuration
    {   
        private readonly float _radius;
        private readonly float _duration;
        private readonly Transform _indicatorsPivot;
        private readonly GameObject _areaIndicator;
        private readonly GameObject _chargeIndicator;
        private readonly IAimDirection _aimDirection;

        private float _elapse;

        public float UnitActDuration => _duration;

        public bool IsFinished()
        {
            return _elapse < 0f;
        }

        public UnitBossAoeWindup(float duration, float radius, GameObject areaIndicator, GameObject chargeIndicator, IAimDirection aimDirection)
        {
            _radius = radius;
            _duration = duration;
            _areaIndicator = areaIndicator;
            _chargeIndicator = chargeIndicator;
            _indicatorsPivot = _areaIndicator.transform.parent.parent;
            _aimDirection = aimDirection;
        }

        public void OnUnitActStart()
        {
            _elapse = _duration;
            
            _areaIndicator.SetActive(true);
            _areaIndicator.transform.localScale = new Vector3(_radius, _radius, _radius);
            
            _chargeIndicator.SetActive(true);
            _chargeIndicator.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);

            _indicatorsPivot.transform.localEulerAngles = new Vector3(0, 0,
                GameplayAnimationUtility.Angle360(new Vector2(0, 1), _aimDirection.AimDirection));
        }

        public void OnUnitActUpdate()
        {
            _elapse -= Time.deltaTime;

            var percentCharged = (_duration - _elapse) / _duration;
            var updatedChargeScale = _radius * percentCharged;
            _chargeIndicator.transform.localScale = new Vector3(updatedChargeScale, updatedChargeScale, updatedChargeScale);
        }

        public void OnUnitActEnd()
        {
            _areaIndicator.SetActive(false);
            _chargeIndicator.SetActive(false);
        }
    }
}
