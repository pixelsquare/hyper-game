using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class Movement : MonoBehaviour, IMotor2dPhysics, IMovement
    {
        private readonly static Vector3 RATIO = new(1, 1, 1); // todo : update accdg to isometric ratio

        [Header("References")]
        [SerializeField] private CircleCollider2D _circleCollider;
        [SerializeField] private CharacterMotor2D _characterMotor;

        [Header("Config")]
        [SerializeField] private float _speed = 1f;
        [SerializeField] private float _mass = 1f;

        public float Mass => _mass;
        public CircleCollider2D CircleCollider2D => _circleCollider;

        public Vector3 Delta { get; set; }

        private IMoveCondition _moveCondition;
        private IMoveSpeed _moveSpeed;
        private Vector2 _currentDirection;
        private float _previousVelocity;

        private LazyInject<UnitPlayer> _unitPlayer;

        [Inject]
        private void Construct(LazyInject<UnitPlayer> unitPlayer)
        {
            _unitPlayer = unitPlayer;
        }

        private void FixedUpdate()
        {
            if (_moveCondition.CanMove && Delta != Vector3.zero)
            {
                var velocity = Vector3.Scale(Delta, RATIO) * (_speed * Time.fixedDeltaTime);

                if (_characterMotor)
                {
                    _characterMotor.Move(velocity);
                }
            }
        }

        private void UpdateMoveSpeed(PlayerStats stats)
        {
            if (_moveSpeed != null)
            {
                _speed = _moveSpeed.MoveSpeed;
            }
        }

        private void Awake()
        {
            _moveCondition = GetComponent<IMoveCondition>();
            _moveSpeed = GetComponent<IMoveSpeed>();
        }

        private void OnEnable()
        {
            _unitPlayer.Value.OnModifyStatsEvent += UpdateMoveSpeed;
        }

        private void OnDisable()
        {
            _unitPlayer.Value.OnModifyStatsEvent -= UpdateMoveSpeed;
        }

        private void Start()
        {
            if (_moveSpeed != null)
            {
                _speed = _moveSpeed.MoveSpeed;
            }
        }

        private void Reset()
        {
            _circleCollider = GetComponent<CircleCollider2D>();
            _characterMotor = GetComponent<CharacterMotor2D>();
        }
    }
}
