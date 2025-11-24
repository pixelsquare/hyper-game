using PowerTools;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    public class UnitEnemyAnimator : MonoBehaviour
    {
        [SerializeField] private AnimationClip[] _moveSet;
        [SerializeField] private AnimationClip[] _hitSet;

        private IAimDirection _aimDirection;
        private IUnitState _unitState;
        private SpriteAnim _spriteAnimator;
        private AnimationClip _animationClip;
        
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
            
            _angleClamp = angleClamp;
            _state = state;

            if (_state == UnitState.Knockback)
            {
                var animationClip = _hitSet[angleClamp];
                _spriteAnimator.Play(animationClip);
            }
            else if (_state == UnitState.Move)
            {
                var animationClip = _moveSet[angleClamp];
                _spriteAnimator.Play(animationClip);            
            }
            else if (_state == UnitState.Spawn)
            {
                var animationClip = _moveSet[angleClamp];
                _spriteAnimator.Play(animationClip);
            }
        }

        private void Awake()
        {
            _spriteAnimator = GetComponent<SpriteAnim>();
            _aimDirection = GetComponentInParent<IAimDirection>();
            _unitState = GetComponentInParent<IUnitState>();
        }
    }
}
