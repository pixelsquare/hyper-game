using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class KahangianProjectile : MonoBehaviour, IProjectile
    {
        [SerializeField] private float _radius;
        [SerializeField] private Transform _vfxPivot;

        public Vector2 Direction { get; set; }
        public float Distance { get; set; } = 4f;
        public float Speed { get; set; }
        public uint Damage { get; set; }
        public uint Pierce { get; set; }

        public Transform ReturnTarget;
        public float HexDuration;
        public float KnockbackValue;
        public float AdditionalDamageToHexed;
        public float AdditionalSize;
        public HexIndicator IndicatorPrefab;
        private HexIndicatorSpawnPoolTracker _hexIndicatorSpawnPoolTracker;
        public event OnProjectileExpire OnProjectileExpire;

        private List<GameObject> _forwardMotionHits;
        private int _direction;
        private float _forwardMotionCompletionPct;
        private float _forwardDistance;

        private DiContainer _diContainer;
        private LazyInject<HexSystem> _hexSystem;

        public void OnHit(Transform hitTransform)
        {
            if (hitTransform.TryGetComponent<IHittable>(out var hittable))
            {
                var realDamage = Damage;
                
                if (_hexSystem.Value.IsEnemyHexed<KahangianHex>(hitTransform))
                {
                    realDamage = Damage + (uint)(Damage * AdditionalDamageToHexed);
                }

                hittable.Hit(realDamage, transform.position);
                var hex = _diContainer.Instantiate<KahangianHex>(new object[]
                        { hitTransform, HexDuration, KnockbackValue, IndicatorPrefab, _hexIndicatorSpawnPoolTracker });
                _hexSystem.Value.OnHexApplyEvent(hex);
            }
        }

        [Inject]
        private void Construct(DiContainer diContainer, LazyInject<HexSystem> hexSystem)
        {
            _diContainer = diContainer;
            _hexSystem = hexSystem;
            _hexIndicatorSpawnPoolTracker = hexSystem.Value.GetComponent<HexIndicatorSpawnPoolTracker>();
        }
        
        private void LateUpdate()
        {
            var angleClamp = GameplayAnimationUtility.DirectionToIndex(Direction);

            if (_direction > 0)
            {
                _vfxPivot.transform.eulerAngles = new Vector3(0, 0, (angleClamp * 45f) + 270f);
            }
            else
            {
                _vfxPivot.transform.eulerAngles = new Vector3(0, 0, (angleClamp * 45f) + 90f);
            }
        }

        private void OnEnable()
        {
            _direction = 1;
            _forwardMotionHits = new List<GameObject>();

            Distance = 4f;
            _forwardMotionCompletionPct = 0;
            _forwardDistance = Distance;
            
            transform.localScale = Vector3.one;
        }

        private void Update()
        {
            var dir = _direction > 0 ? Direction : (Vector2)(ReturnTarget.position - transform.position);
            dir.Normalize();

            var delta = dir * Speed;
            transform.position += (Vector3)delta;

            if (_direction > 0)
            {
                if (Distance > 0)
                {
                    Distance -= delta.magnitude;

                    if (AdditionalSize == 0)
                    {
                        return;
                    }
                    
                    _forwardMotionCompletionPct = Mathf.Clamp((_forwardDistance - Distance) / _forwardDistance, 0, 1);

                    var additionalSizeByPct = _forwardMotionCompletionPct * AdditionalSize;

                    transform.localScale = new Vector3(1 + additionalSizeByPct, 1 + additionalSizeByPct, 1 + additionalSizeByPct);

                    if (Distance <= 0)
                    {
                        transform.localScale = Vector3.one + Vector3.one * AdditionalSize;
                    }
                }
                else
                {
                    _direction = -1;
                    _forwardMotionHits.Clear();
                }
            }
            else if (Vector3.Distance(ReturnTarget.position, transform.position) < Speed)
            {
                OnProjectileExpire?.Invoke(this);
            }
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            OnHit(col.transform);

            if (!_forwardMotionHits.Contains(col.transform.gameObject))
            {
                OnHit(col.transform);
                _forwardMotionHits.Add(col.transform.gameObject);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _radius);
        }
    }
}
