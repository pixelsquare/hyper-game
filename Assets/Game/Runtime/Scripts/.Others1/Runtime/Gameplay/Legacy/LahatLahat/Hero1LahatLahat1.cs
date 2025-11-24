using System;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class Hero1LahatLahat1 : Legacy, IWeaponHit, ILegacyLayerApply, IModifyCooldown
    {
        [Serializable]
        private struct Stats
        {
            public float _damageModifier;
            public float _interval;
            public int _hits;
            public float _hitRate;
            public float _angle;
            public float _range;
        }

        [SerializeField] private Stats[] _statsTable;
        [SerializeField] private ContactFilter2D _contactFilter;
        [SerializeField] private Transform _animationPivot;
        [SerializeField] private Animator _animator;

        public override string LegacyId => "Hero1LahatLahat1";
        public override uint MaxLevel => (uint)_statsTable.Length;

        public event OnWeaponHit OnWeaponHit;

        private Stats _stats;

        private float _interval;
        private int _hits;
        private float _hitRate;
        private float _coneThreshold;
        private uint _totalDamage;
        private Collider2D[] _hitboxes;
        private IAimDirection _aimDirection;
        private int _animParamIsOnId;
        private int _animParamLegacyLayerId;

        private IAudioManager _audioManager;
        private LazyInject<UnitPlayer> _unitPlayer;
        private LazyInject<LegacySlotTracker> _legacySlotTracker;

        public override void LevelUp()
        {
            if (_currentLevel < MaxLevel)
            {
                _stats = _statsTable[_currentLevel++];
                _coneThreshold = Mathf.Cos(_stats._angle * Mathf.Deg2Rad / 2f);
                var playerStats = GetComponentInParent<UnitPlayer>().Stats; // todo [optimize]
                _totalDamage = playerStats.CalcTotalDamage(_stats._damageModifier);
            }
        }

        [Inject]
        private void Construct(IAudioManager audioManager, LazyInject<UnitPlayer> unitPlayer, LazyInject<LegacySlotTracker> legacySlotTracker)
        {
            _audioManager = audioManager;
            _unitPlayer = unitPlayer;
            _legacySlotTracker = legacySlotTracker;
        }

        private void Pulse()
        {
            var hitAmount = Physics2D.OverlapCircle(transform.position, _stats._range, _contactFilter, _hitboxes);
            var aim = _aimDirection.AimDirection;

            for (var i = 0; i < hitAmount; ++i)
            {
                var collider = _hitboxes[i];
                var targetTransform = collider.transform;

                if (!collider.TryGetComponent<IHittable>(out var hittable) || !hittable.IsHittable)
                {
                    continue;
                }

                var direction = (targetTransform.position - transform.position).normalized;
                var dot = Vector2.Dot(aim, direction);

                if (dot > _coneThreshold)
                {
                    hittable.Hit(_totalDamage, transform.position);
                    OnWeaponHit?.Invoke(targetTransform);
                }
            }
        }

        private void OnModifyStats(PlayerStats stats)
        {
            _totalDamage = stats.CalcTotalDamage(_stats._damageModifier);
        }

        private void OnEquipLegacy(ILegacy legacy)
        {
            if (legacy.LegacySlot == LegacySlot.LahatLahat && legacy.LegacyId != LegacyId)
            {
                gameObject.SetActive(false);
            }

            if (legacy is IWeaponHitEffect hitEffect)
            {
                OnWeaponHit += hitEffect.OnHitEffect;
            }

            if (legacy is ILegacyLayerEffect layerEffect)
            {
                _animator.SetInteger(_animParamLegacyLayerId, (int)layerEffect.LegacyEffectLayer);
            }
        }
        
        public void OnCooldownModified(float cooldownReduction)
        {
            for (int i = 0; i<_statsTable.Length; i++)
            {
                var oldStat = _statsTable[i];

                _statsTable[i] = new Stats
                {
                    _damageModifier = oldStat._damageModifier,
                    _interval = oldStat._interval - oldStat._interval * cooldownReduction,
                    _hits = oldStat._hits,
                    _hitRate = oldStat._hitRate,
                    _angle = oldStat._angle,
                    _range = oldStat._range,
                };
            }
            
            _stats = _statsTable[_currentLevel - 1];
        }

        private void OnModifyCooldown(IMessage message)
        {
            var cooldownReduction = (float)message.Data;
            OnCooldownModified(cooldownReduction);
        }

        public void ApplyLegacyLayer(LegacyLayer legacyLayer)
        {
            _animator.SetInteger(_animParamLegacyLayerId, (int)legacyLayer);
        }

        private void Update()
        {
            if (_interval > 0f)
            {
                _interval -= Time.deltaTime;

                if (_interval < 0f) // signifies that the attack sequence will begin next frame
                {
                    _audioManager.PlaySound(Sfx.Hero1LahatLahat1);
                }
            }
            else if (_hitRate > 0f)
            {
                _hitRate -= Time.deltaTime;
            }
            else if (_hits-- > 0)
            {
                _animationPivot.up = _aimDirection.AimDirection;
                _animator.SetBool(_animParamIsOnId, true);
                Pulse();
                _hitRate = _stats._hitRate;
            }
            else
            {
                _animator.SetBool(_animParamIsOnId, false);
                _interval = _stats._interval;
                _hits = _stats._hits;
            }
        }

        private void Awake()
        {
            _aimDirection = GetComponentInParent<IAimDirection>();
        }

        private void OnEnable()
        {
            _unitPlayer.Value.OnModifyStatsEvent += OnModifyStats;
            _legacySlotTracker.Value.OnEquipLegacyEvent += OnEquipLegacy;
            Dispatcher.AddListener(GameEvents.Gameplay.ModifyCooldownReduction, OnModifyCooldown);
        }

        private void OnDisable()
        {
            _unitPlayer.Value.OnModifyStatsEvent -= OnModifyStats;
            _legacySlotTracker.Value.OnEquipLegacyEvent -= OnEquipLegacy;
            Dispatcher.RemoveListener(GameEvents.Gameplay.ModifyCooldownReduction, OnModifyCooldown, true);
        }

        private void Start()
        {
            _animParamIsOnId = Animator.StringToHash(GameConstants.AnimatorParameters.IsOn);
            _animParamLegacyLayerId = Animator.StringToHash(GameConstants.AnimatorParameters.LegacyLayer);
            _animator.SetInteger(_animParamLegacyLayerId, (int)Rinsurv.LegacyLayer.None);
            _hitboxes = new Collider2D[GameConstants.GameValues.HitCollisions];
            LevelUp();
            Dispatcher.SendMessageData(GameEvents.Gameplay.OnEquipLegacy, this);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;
        }

        private void OnDrawGizmos()
        {
            if (_statsTable.Length == 0)
            {
                return;
            }

            var position = transform.position;
            var up = transform.up;
            var stats = _statsTable[0];

            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(position, up * stats._range);

            var coneDirections = ProjectileUtility.GetConeDirections(up, 2, stats._angle);
            foreach (var direction in coneDirections)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawRay(position, direction * stats._range);
            }
        }
    }
}
