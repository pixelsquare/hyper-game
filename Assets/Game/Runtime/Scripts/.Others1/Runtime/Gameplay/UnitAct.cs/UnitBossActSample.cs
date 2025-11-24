using System;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    public class UnitBossActSample : MonoBehaviour, IUnitActState, IAimDirection
    {
        [Serializable]
        private struct Stats
        {
            public float _windupDuration;
            public float _attackDuration;
            public float _attackRadius;
            public float _proximityThreshold;
        }

        [SerializeField] private GameObject _attackRadiusIndicator;
        [SerializeField] private GameObject _chargeIndicator;
        [SerializeField] private ContactFilter2D _contactFilter;
        [SerializeField] private Stats _stats;

        public Vector2 AimDirection { get; set; }
        public int ActIndex { get; private set; }

        private IMovement _movement;
        private IUnitState _unitState;
        private IUnitAct[] _actQueue;
        private IUnitAct _currentAct;

        private void Init()
        {
            _actQueue = new IUnitAct[]
            {
                new UnitBossMove(_stats._proximityThreshold, transform, EnemyTarget.Instance.transform, _movement, this),
                new UnitBossAoeWindup(_stats._windupDuration, _stats._attackRadius, _attackRadiusIndicator, _chargeIndicator, this),
                new UnitBossAoeAttack(_stats._attackDuration, _stats._attackRadius, this, _contactFilter),
            };
            
            _currentAct = _actQueue[0];
        }

        private void NextAct()
        {
            if (_currentAct is IUnitActEnd unitActEnd)
            {
                unitActEnd.OnUnitActEnd();
            }
            
            if (++ActIndex == _actQueue.Length)
            {
                ActIndex = 0;
            }
            
            _currentAct = _actQueue[ActIndex];

            if (_currentAct is IUnitActStart unitActStart)
            {
                unitActStart.OnUnitActStart();
            }
        }

        private void Update()
        {
            if (!(_unitState.State == UnitState.Move
                || _unitState.State == UnitState.Knockback))
            {
                return;
            }
            
            if (_currentAct is IUnitActUpdate unitActUpdate)
            {
                unitActUpdate.OnUnitActUpdate();
            }

            if (_currentAct.IsFinished())
            {
                NextAct();
            }
        }

        private void Awake()
        {
            _unitState = GetComponent<IUnitState>();
            _movement = GetComponent<IMovement>();
        }

        private void Start()
        {
            Init();
        }
    }
}
