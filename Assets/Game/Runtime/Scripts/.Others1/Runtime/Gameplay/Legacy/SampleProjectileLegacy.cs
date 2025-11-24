using System;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class SampleProjectileLegacy : Legacy
    {
        [Serializable]
        private class Stats
        {
            public uint _damage;
            public uint _amount;
            public float _interval;
            public float _projectileSpeed;
        }

        [SerializeField] private SampleProjectile _projectilePrefab;
        [SerializeField] private Stats _stats;

        public override string LegacyId => "SampleProjectileLegacy";
        public override string LegacyName => "Sample Projectile Legacy";
        
        public override LegacySlot LegacySlot => LegacySlot.LahatLahat;
        public override uint MaxLevel => 1;
        public override uint CurrentLevel => 1;

        private float _interval;
        private uint _amount;

        private IAimDirection _aimDirection;
        private DiContainer _diContainer;

        public override void LevelUp()
        {
        }

        [Inject]
        private void Construct(DiContainer diContainer)
        {
            _diContainer = diContainer;
        }

        private void Update()
        {
            if (_interval > 0)
            {
                _interval -= Time.deltaTime;
            }
            else if (_amount > 0)
            {
                var position = transform.position;
                var projectile = _diContainer.InstantiatePrefabForComponent<SampleProjectile>(_projectilePrefab, position, Quaternion.identity, null) 
                ?? Instantiate(_projectilePrefab, position, Quaternion.identity);
                projectile.Speed = _stats._projectileSpeed;
                projectile.Direction = _aimDirection.AimDirection;
                projectile.Damage = _stats._damage;
                --_amount;
            }
            else
            {
                _interval = _stats._interval;
                _amount = _stats._amount;
            }
        }

        private void Awake()
        {
            _aimDirection = transform.parent.GetComponent<IAimDirection>();
        }

        private void Start()
        {
            Dispatcher.SendMessageData(GameEvents.Gameplay.OnEquipLegacy, this);
        }
    }
}
