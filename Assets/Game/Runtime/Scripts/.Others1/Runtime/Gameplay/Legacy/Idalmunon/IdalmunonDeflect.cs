using System;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class IdalmunonDeflect : Legacy
    {
        [Serializable]
        private struct Stats
        {
            public float _knockbackRadius;
            public float _cooldown;
            public uint _invulnerableDuration;
            public float _healthRestore;
        }
        
        [SerializeField] private ParticleSystem _bubbleVfx;
        [SerializeField] private ParticleSystem _blastVfx;
        [SerializeField] private float _pushback;
        [SerializeField] private uint _maxLevel = 5;
        [SerializeField] private Stats[] _statsTable = new Stats[5];

        public override string LegacyId => "IdalmunonDeflect";
        public override uint MaxLevel => _maxLevel;

        private Collider2D[] _colliders;
        private Stats _stats;
        private UnitPlayerState _unitPlayerState;
        private Health _health;
        private bool _isCoolingDown;
        private float _cooldownTimer;
        private bool _isInvulnerable;
        private float _invulnerableTimer;

        private DiContainer _diContainer;
        private IAudioManager _audioManager;
        private LazyInject<UnitPlayer> _unitPlayer;

        public override void LevelUp()
        {
            ++_currentLevel;
            _stats = _statsTable[_currentLevel - 1];
        }

        public void Activate()
        {
            if (_isCoolingDown)
            {
                return;
            }

            _unitPlayerState.Revive();
            _health.RestorePercentage(_stats._healthRestore);
            _health.IsHittable = false;
            
            //blast effect
            if (_blastVfx != null)
            {
                var position = transform.position;
                var blastFx = _diContainer.InstantiatePrefab(_blastVfx, position, Quaternion.identity, null) 
                     ?? Instantiate(_blastVfx, position, Quaternion.identity).gameObject;
                blastFx.SetActive(true);
                Destroy(blastFx, 2);
            }
            
            //bubble effect
            if (_bubbleVfx != null)
            {
                _bubbleVfx.gameObject.SetActive(true);
            }

            var hits = Physics2D.OverlapCircleNonAlloc(transform.position, _stats._knockbackRadius, _colliders);

            for (var i = 0; i < hits; ++i)
            {
                var col = _colliders[i];

                if (!col.TryGetComponent<Health>(out var contactHealth))
                {
                    continue;
                }

                var direction = (col.transform.position - transform.position).normalized;
                col.transform.position += direction * _pushback;
            }

            _audioManager.PlaySound(Sfx.IdalmunonDeflect);

            _isCoolingDown = true;
            _cooldownTimer = 0;

            _isInvulnerable = true;
            _invulnerableTimer = 0;
        }

        [Inject]
        private void Construct(DiContainer diContainer, IAudioManager audioManager, LazyInject<UnitPlayer> unitPlayer)
        {
            _diContainer = diContainer;
            _audioManager = audioManager;
            _unitPlayer = unitPlayer;
        }

        private void Update()
        {
            if (_isCoolingDown)
            {
                _cooldownTimer += Time.deltaTime;

                if (_cooldownTimer >= _stats._cooldown)
                {
                    _isCoolingDown = false;
                }
            }

            if (_isInvulnerable)
            {
                _invulnerableTimer += Time.deltaTime;

                if (_invulnerableTimer >= _stats._invulnerableDuration)
                {
                    _isInvulnerable = false;
                    _health.IsHittable = true;
                    
                    if (_bubbleVfx != null)
                    {
                        _bubbleVfx.gameObject.SetActive(false);
                    }
                }
            }
        }

        private void Awake()
        {
            _currentLevel = 1;
        }

        private void Start()
        {
            _colliders = new Collider2D[50];
            _stats = _statsTable[_currentLevel - 1];
            _unitPlayerState = transform.GetComponentInParent<UnitPlayerState>();
            _health = transform.GetComponentInParent<Health>(); //todo: avoid direct reference of type Health

            _unitPlayer.Value.OnPlayerDeathEvent += Activate;
            Dispatcher.SendMessageData(GameEvents.Gameplay.OnEquipLegacy, this);
        }
    }
}
