using PowerTools;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    public class UnitBossAnimator : MonoBehaviour
    {
        [SerializeField] private SerializableDictionary<int, AnimationClip[]> _animTable;
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private IAimDirection _aimDirection;
        private IUnitActState _unitActState;
        private IUnitState _unitState;
        private SpriteAnim _spriteAnimator;

        private int _angleClamp;
        private int _actIndex;
        private UnitState _state;
        private static readonly int _hitProp = Shader.PropertyToID("_Hit");

        private void OnUnitStateChange(UnitState prev, UnitState next)
        {
            var value = next == UnitState.Knockback ? 1 : 0;
            _spriteRenderer.material.SetFloat(_hitProp, value);
        }

        private void Update()
        {
            var angleClamp = GameplayAnimationUtility.DirectionToIndex(_aimDirection.AimDirection);
            var actIndex = _unitActState.ActIndex;
            
            if (_angleClamp == angleClamp && _actIndex == actIndex)
            {
                return;
            }
            
            _angleClamp = angleClamp;
            _actIndex = actIndex;
            
            var clip = _animTable[actIndex][angleClamp];
                        
            _spriteAnimator.Play(clip);
        }

        private void Awake()
        {
            _spriteAnimator = GetComponent<SpriteAnim>();
            _aimDirection = GetComponentInParent<IAimDirection>();
            _unitActState = GetComponentInParent<IUnitActState>();
            _unitState = GetComponentInParent<IUnitState>();
        }

        private void OnEnable()
        {
            _unitState.OnUnitStateChange += OnUnitStateChange;
        }

        private void OnDisable()
        {
            _unitState.OnUnitStateChange -= OnUnitStateChange;
        }
    }
}
