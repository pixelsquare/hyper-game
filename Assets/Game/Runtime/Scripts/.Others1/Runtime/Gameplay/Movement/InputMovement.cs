using UnityEngine;

namespace Santelmo.Rinsurv
{
    public class InputMovement : MonoBehaviour, IAimDirection, IMoveEvent, IMovement
    {
        private IMovement _movement;

        private Vector2 _direction;
        private Vector2 _joystickInput;

        public event OnMoveStart OnMoveStart;
        public event OnMoveStop OnMoveStop;

        public bool IsMoving => _direction != Vector2.zero;

        public Vector2 AimDirection { get; set; } = Vector2.right;
        public Vector3 Delta { get; set; }

        public void SetDirection(Vector2 joystickInput)
        {
            _joystickInput = joystickInput;
        }

        private Vector2 _prevDirection;

        private void Update()
        {
            if (_joystickInput == Vector2.zero)
            {
                var velocity = Vector3.zero;
    
                velocity.x += Input.GetKey(KeyCode.A) ? -1 : 0;
                velocity.x += Input.GetKey(KeyCode.D) ? 1 : 0;
                velocity.y += Input.GetKey(KeyCode.S) ? -1 : 0;
                velocity.y += Input.GetKey(KeyCode.W) ? 1 : 0;
    
                _direction = velocity.normalized;
            }
            else
            {
                _direction = _joystickInput.normalized;
            }

            if (_prevDirection == Vector2.zero && IsMoving)
            {
                OnMoveStart?.Invoke();
            }
            
            if (_prevDirection != Vector2.zero && !IsMoving)
            {
                OnMoveStop?.Invoke();
            }

            _prevDirection = _direction;
            _movement.Delta = _direction;

            if (_direction != Vector2.zero)
            {
                AimDirection = _direction;
            }
        }

        private void Awake()
        {
            _movement = GetComponent<IMovement>();
        }

        private void OnEnable()
        {
            JoystickController._onJoystickDrag += SetDirection;
        }

        private void OnDisable()
        {
            JoystickController._onJoystickDrag -= SetDirection;
        }
    }
}
