using System;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class IdalmunonPassive4 : Legacy
    {
        [Serializable]
        private struct Stats
        {
            public float _percentageHeal;
        }

        [SerializeField] private GameObject _vfx;
        [SerializeField] private uint _healAmount;
        [SerializeField] private uint _maxLevel = 5;
        [SerializeField] private Stats[] _statsTable = new Stats[5];

        public override string LegacyId => "IdalmunonPassive4";
        public override uint MaxLevel => _maxLevel;

        private Stats _stats;
        private UnitPlayer _unitPlayer;
        private Health _health;

        private DiContainer _diContainer;

        public override void LevelUp()
        {
            _stats = _statsTable[_currentLevel];
            ++_currentLevel;
        }

        [Inject]
        private void Construct(DiContainer diContainer)
        {
            _diContainer = diContainer;
        }

        private void Activate(uint damage)
        {
            //heal here
            _health.RestoreAmount((uint)(_healAmount * _stats._percentageHeal));

            //temporary vfx to show activation
            var position = transform.position;
            var hitIndicator = _diContainer.InstantiatePrefab(_vfx, position, Quaternion.identity, null) 
                 ?? Instantiate(_vfx, position, Quaternion.identity);
            hitIndicator.SetActive(true);
            Destroy(hitIndicator.gameObject, 0.5f);
        }

        private void Awake()
        {
            _unitPlayer = transform.GetComponentInParent<UnitPlayer>();
            _health = transform.GetComponentInParent<Health>();
        }

        private void OnEnable()
        {
            _unitPlayer.OnPlayerHexDamageEvent += Activate;
        }

        private void OnDisable()
        {
            _unitPlayer.OnPlayerHexDamageEvent -= Activate;
        }

        private void Start()
        {
            LevelUp();
            Dispatcher.SendMessageData(GameEvents.Gameplay.OnEquipLegacy, this);
        }

        private void OnValidate()
        {
            if (_statsTable.Length != MaxLevel)
            {
                _statsTable = new Stats[MaxLevel];
            }
        }
    }
}
