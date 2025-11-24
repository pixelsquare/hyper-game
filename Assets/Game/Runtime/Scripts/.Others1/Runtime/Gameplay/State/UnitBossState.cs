using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class UnitBossState : MonoBehaviour, IUnitState, IMoveCondition
    {
        [SerializeField] private float _spawnDuration;
        [SerializeField] private float _deathDuration;
        [SerializeField] private CharacterMotor2D _characterMotor; // todo [optimize]: consider using interfaces instead

        public bool CanMove => State == UnitState.Move || State == UnitState.Knockback;
        public UnitState State { get; private set; }

        public event OnUnitStateChange OnUnitStateChange;

        private IDamageEvent _damageBroadcaster;
        private IKnockback _knockback;
        private IDeathEvent _deathEvent;
        private ISpawnEvent _spawnEvent;
        private IAudioManager _audioManager;

        private float _stateTime;
        private Vector3 _knockbackDelta;
        private Vector3 _knockbackOrigin;

        [Inject]
        public void Construct(IAudioManager audioManager)
        {
            _audioManager = audioManager;
        }

        private void Elapse(UnitState next)
        {
            if (_stateTime > 0f)
            {
                _stateTime -= Time.deltaTime;
            }
            else
            {
                var prev = State;
                State = next;
                OnUnitStateChange?.Invoke(prev, next);
            }
        }

        private void OnUnitDamage(uint damage, uint currenthealth, uint maxhealth, Vector3 damageOrigin)
        {
            _knockbackOrigin = transform.position;
            _knockbackDelta = (_knockbackOrigin - damageOrigin).normalized * _knockback.KnockbackDistance;

            var prev = State;
            State = UnitState.Knockback;
            _stateTime = _knockback.KnockbackDuration;
            _audioManager.PlaySound(Sfx.EnemyHit);
            OnUnitStateChange?.Invoke(prev, State);
        }

        private void OnUnitDeath()
        {
            var prev = State;
            State = UnitState.Death;
            _stateTime = _deathDuration;
            _audioManager.PlaySound(Sfx.BossDeath);
            OnUnitStateChange?.Invoke(prev, State);
        }

        private void OnUnitSpawn(UnitSpawn unitSpawn)
        {
            var prev = UnitState.None;
            State = UnitState.Spawn;
            _stateTime = _spawnDuration;
            OnUnitStateChange?.Invoke(prev, State);
        }

        private void Update()
        {
            switch (State)
            {
                case UnitState.Spawn:
                case UnitState.Knockback:
                    Elapse(UnitState.Move);
                    break;
                case UnitState.Death:
                    Elapse(UnitState.Spawn);
                    break;
                case UnitState.Move:
                case UnitState.Idle:
                default:
                    break;
            }
        }

        private void FixedUpdate()
        {
            if (State == UnitState.Knockback)
            {
                _characterMotor.Move(_knockbackDelta * Time.fixedDeltaTime);
            }
        }

        private void Awake()
        {
            _damageBroadcaster = GetComponent<IDamageEvent>();
            _knockback = GetComponent<IKnockback>();
            _deathEvent = GetComponent<IDeathEvent>();
            _spawnEvent = GetComponent<ISpawnEvent>();
        }

        private void OnEnable()
        {
            _damageBroadcaster.OnUnitDamage += OnUnitDamage;
            _deathEvent.OnUnitDeath += OnUnitDeath;
            _spawnEvent.OnSpawnEvent += OnUnitSpawn;
        }

        private void OnDisable()
        {
            _damageBroadcaster.OnUnitDamage -= OnUnitDamage;
            _deathEvent.OnUnitDeath -= OnUnitDeath;
            _spawnEvent.OnSpawnEvent -= OnUnitSpawn;
        }
    }
}
