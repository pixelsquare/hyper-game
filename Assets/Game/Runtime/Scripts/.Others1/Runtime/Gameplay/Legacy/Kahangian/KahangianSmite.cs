using System;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class KahangianSmite : Legacy, IModifyCooldown, IModifyThunderLegacies
    {
        [SerializeField] private Stats[] _statsTable;
        [SerializeField] private Vector2 _viewBoxDims;
        [SerializeField] private ContactFilter2D _contactFilter;
        [SerializeField] private float _vfxWidth;
        [SerializeField] private SoloSpawnPool _lightningPool;
        [SerializeField] private SoloSpawnPool _boltPool;
        
        public override string LegacyId => "KahangianSmite";
        public override uint MaxLevel => (uint)_statsTable.Length;


        private Stats _stats;
        private Collider2D[] _colliders;
        private uint _totalDamage;
        private float _duration;
        private float _additionalDamageToHexed;
        private UnitPlayer _unitPlayer;
        private IAudioManager _audioManager;
        private LazyInject<HexSystem> _hexSystem;

        public override void LevelUp()
        {
            if (_currentLevel == MaxLevel)
            {
                return;
            }

            _stats = _statsTable[_currentLevel];
            ++_currentLevel;
        }
        
        [Inject]
        private void Construct(IAudioManager audioManager, LazyInject<UnitPlayer> unitPlayer, LazyInject<HexSystem> hexSystem)
        {
            _audioManager = audioManager;
            _unitPlayer = unitPlayer.Value;
            _hexSystem = hexSystem;
        }

        private void Pulse()
        {
            var enemyCountInBox = Physics2D.OverlapBox(transform.position, _viewBoxDims, 0, _contactFilter, _colliders);
            var targetCount = _stats._targetCount;

            if (targetCount == 0 || enemyCountInBox == 0)
            {
                return;
            }
            
            if (enemyCountInBox < _stats._targetCount)
            {
                targetCount = enemyCountInBox;
            }
            
            var indexArray = Enumerable.Range(0, enemyCountInBox).ToArray();
            var rnd = new System.Random();

            //shuffle
            for (int i = 0; i < indexArray.Length; i++)
            {
                int randomIndex = rnd.Next(indexArray.Length);
                (indexArray[randomIndex], indexArray[i]) = (indexArray[i], indexArray[randomIndex]);
            }

            var enemiesSmote = new GameObject[targetCount];
            GameObject previousEnemy = null;
            
            for (int i = 0; i < enemiesSmote.Length; i++)
            {
                enemiesSmote[i] = _colliders[indexArray[i]].gameObject;
                
                if (!enemiesSmote[i].TryGetComponent<IHittable>(out var hittable))
                {
                    continue;
                }
                
                var realDamage = _totalDamage;
                
                if (_hexSystem.Value.IsEnemyHexed<KahangianHex>(enemiesSmote[i].transform))
                {
                    realDamage = _totalDamage + (uint)(_totalDamage * _additionalDamageToHexed);
                    
                    Debug.Log($"{enemiesSmote[i].name} is hexed! dealing {realDamage} instead of {_totalDamage}");
                }
                
                hittable.Hit(realDamage, transform.position);
                
                CreateLines(previousEnemy, enemiesSmote[i]);
                previousEnemy = enemiesSmote[i];
            }
            
            _audioManager.PlaySound(Sfx.KahangianSmite);
        }

        private void CreateLines(GameObject sourceEnemy, GameObject targetEnemy)
        {
            if (sourceEnemy == null) //main lightning from sky
            {
                var indicator = _lightningPool.Spawn();
                
                var indicatorTransform = indicator.transform;
                indicatorTransform.position = targetEnemy.transform.position;
                indicatorTransform.rotation = Quaternion.identity;
                indicatorTransform.parent = null;
                indicatorTransform.localScale = new Vector3(1, 1, 1);
                
                indicator.SetReturnParent(transform);
                indicator.gameObject.SetActive(true);
            }
            else
            {
                var sourcePosition = new Vector3(sourceEnemy.transform.position.x, sourceEnemy.transform.position.y, sourceEnemy.transform.position.z);
                
                var enemyDirection = new Vector2(targetEnemy.transform.position.x - sourcePosition.x, 
                    targetEnemy.transform.position.y - sourcePosition.y);
                
                var indicator = _boltPool.Spawn();
                
                var indicatorTransform = indicator.transform;
                indicatorTransform.position = sourcePosition;
                indicatorTransform.rotation = Quaternion.identity;
                indicatorTransform.parent = null;
                indicatorTransform.localScale = new Vector3(_vfxWidth, (targetEnemy.transform.position - sourcePosition).magnitude, 1);
                
                indicator.SetReturnParent(transform);
                indicator.gameObject.SetActive(true);

                var angle = Vector2.Angle(Vector2.up, enemyDirection);
                if (enemyDirection.x > 0)
                {
                    angle = 360 - angle;
                }
            
                indicatorTransform.localEulerAngles = new Vector3(0, 0, angle);
            }
        }
        
        public void OnCooldownModified(float cooldownReduction)
        {
            for (int i = 0; i<_statsTable.Length; i++)
            {
                var oldStat = _statsTable[i];

                _statsTable[i] = new Stats
                {
                    _damage = oldStat._damage,
                    _cooldown = oldStat._cooldown - oldStat._cooldown * cooldownReduction,
                    _targetCount = oldStat._targetCount,
                };
            }
            
            _stats = _statsTable[_currentLevel];
        }

        private void OnModifyCooldown(IMessage message)
        {
            var cooldownReduction = (float)message.Data;
            OnCooldownModified(cooldownReduction);
        }

        public void OnIncreaseDamageToHexed(float additionalDamagePct)
        {
            _additionalDamageToHexed = additionalDamagePct;
        }
        
        private void OnModifyThunderDamage(IMessage message)
        {
            var additionalDamagePct = (float)message.Data;
            OnIncreaseDamageToHexed(additionalDamagePct);
        }

        private void Update()
        {
            if (_duration < 0f)
            {
                _duration = _stats._cooldown;
                Pulse();
            }
            else
            {
                _duration -= Time.deltaTime;
            }
        }

        private void OnModifyStats(PlayerStats stats)
        {
            _totalDamage = stats.CalcTotalDamage(_stats._damage);
        }

        private void Start()
        {
            LevelUp();
            _colliders = new Collider2D[50];
            Dispatcher.SendMessageData(GameEvents.Gameplay.OnEquipLegacy, this);
        }

        private void OnEnable()
        {
            _unitPlayer.OnModifyStatsEvent += OnModifyStats;
            Dispatcher.AddListener(GameEvents.Gameplay.ModifyCooldownReduction, OnModifyCooldown);
            Dispatcher.AddListener(GameEvents.Gameplay.ModifyThunderLegacyDamage, OnModifyThunderDamage);
        }

        private void OnDisable()
        {
            _unitPlayer.OnModifyStatsEvent -= OnModifyStats;
            Dispatcher.RemoveListener(GameEvents.Gameplay.ModifyCooldownReduction, OnModifyCooldown, true);
            Dispatcher.RemoveListener(GameEvents.Gameplay.ModifyThunderLegacyDamage, OnModifyThunderDamage, true);
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;

            Gizmos.DrawWireCube(transform.position, new Vector3(_viewBoxDims.x, _viewBoxDims.y, 0));
        }

        [Serializable]
        private struct Stats
        {
            public float _damage;
            public float _cooldown;
            public int _targetCount;
        }
    }
}
