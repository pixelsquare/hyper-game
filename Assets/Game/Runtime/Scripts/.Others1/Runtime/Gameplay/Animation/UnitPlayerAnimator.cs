using PowerTools;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    public class UnitPlayerAnimator : MonoBehaviour
    {
        [SerializeField] private AnimationClip[] _idleSet;
        [SerializeField] private AnimationClip[] _moveSet;

        private IAimDirection _aimDirection;
        private IUnitState _unitState;
        private SpriteAnim _spriteAnimator;
        private AnimationClip _animationClip;
        private UnitPlayer _unitPlayer;
        
        private int _angleClamp;
        private UnitState _state;

        private void Update()
        {
            var angleClamp = GameplayAnimationUtility.DirectionToIndex(_aimDirection.AimDirection);
            var state = _unitState.State;

            if (_angleClamp == angleClamp
                && _state == state)
            {
                return;
            }

            _state = state;
            _angleClamp = angleClamp;
            
            var speed = (_unitPlayer.Stats._movespeed + _unitPlayer.Stats._extraMoveSpeed) / _unitPlayer.Stats._movespeed;

            if (_unitState.State == UnitState.Idle)
            {
                var animationClip = _idleSet[angleClamp];
                _spriteAnimator.Play(animationClip, speed);
            }
            else if (_unitState.State == UnitState.Move)
            {
                var animationClip = _moveSet[angleClamp];
                _spriteAnimator.Play(animationClip, speed);
            }
        }

        private void Awake()
        {
            _spriteAnimator = GetComponent<SpriteAnim>();
            _aimDirection = GetComponentInParent<IAimDirection>();
            _unitState = GetComponentInParent<IUnitState>();
            _unitPlayer = GetComponentInParent<UnitPlayer>();
        }
    }
}
