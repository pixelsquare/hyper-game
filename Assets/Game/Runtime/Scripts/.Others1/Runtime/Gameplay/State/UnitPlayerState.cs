using UnityEngine;

namespace Santelmo.Rinsurv
{
    using AppStateEvent = GameEvents.AppState;

    public class UnitPlayerState : MonoBehaviour, IUnitState, IMoveCondition
    {
        [SerializeField] private float _spawnDuration;
        [SerializeField] private float _deathDuration;

        public UnitState State { get; private set; }
        public bool CanMove => State is UnitState.Move or UnitState.Idle;

        public event OnUnitStateChange OnUnitStateChange;

        private IDamageEvent _damageBroadcaster;
        private IDeathEvent _deathEvent;
        private InputMovement _inputMovement;

        private float _stateTime;
        private UnitPlayer _unitPlayer;

        public void Revive()
        {
            var prev = State;
            State = UnitState.Spawn;
            OnUnitStateChange?.Invoke(prev, UnitState.Spawn);
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

        private void OnUnitDeath()
        {
            var prev = State;
            State = UnitState.Death;
            _stateTime = _deathDuration;
            Dispatcher.SendMessage(GameEvents.Gameplay.OnGameLose);
            OnUnitStateChange?.Invoke(prev, State);
        }

        private void Update()
        {
            if (State == UnitState.Death)
            {
                Elapse(UnitState.None);
            }
            else if (State == UnitState.Spawn)
            {
                Elapse(UnitState.Idle);
            }
            else if (_inputMovement.IsMoving)
            {
                State = UnitState.Move;
            }
            else
            {
                State = UnitState.Idle;
            }
        }

        private void Awake()
        {
            _damageBroadcaster = GetComponent<IDamageEvent>();
            _inputMovement = GetComponent<InputMovement>();
            _unitPlayer = GetComponent<UnitPlayer>();
        }

        private void OnEnable()
        {
            _unitPlayer.OnPlayerDeathEvent += OnUnitDeath;
        }

        private void OnDisable()
        {
            _unitPlayer.OnPlayerDeathEvent -= OnUnitDeath;
        }

        private void Start()
        {
            State = UnitState.Spawn;
            _stateTime = _spawnDuration; // todo update this when pooling is integrated
            OnUnitStateChange?.Invoke(UnitState.None, UnitState.Spawn);
        }
    }
}
