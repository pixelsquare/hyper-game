using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class IdalmunonHex : IHex, IHexApply, IHexRemove, IHexRefresh
    {
        public float Duration { get; }
        public long Id { get; }

        public Transform Transform => _transform;

        private float _radius;
        private readonly uint _damage;
        private readonly HexIndicator _indicatorPrefab;
        private readonly Transform _transform;
        private readonly ContactFilter2D _contactFilter;
        private HexIndicator _indicator;
        private readonly SelfDespawn _radiusDisplay;
        private float _damageMitigation;
        private float _previousDamageMitigation;
        private readonly UnitEnemy _unitEnemy;
        private HexIndicatorSpawnPoolTracker _hexIndicatorSpawnPoolTracker;
        private IMonoSpawnPool<SelfDespawn> _selfDespawnSpawner;
        private Transform _returnParent;
        
        private LazyInject<HexSystem> _hexSystem;

        public IdalmunonHex(float duration, float radius, uint damage, LayerMask layerMask, Transform transform,
                            HexIndicator indicatorPrefab, HexIndicatorSpawnPoolTracker hexIndicatorSpawnPoolTracker, IMonoSpawnPool<SelfDespawn> selfDespawner, 
                            Transform returnParent, float damageMitigation = 0, SelfDespawn radiusPrefab = null)
        {
            Duration = duration;
            _radius = radius;
            _damage = damage;
            _transform = transform;
            _indicatorPrefab = indicatorPrefab;
            _hexIndicatorSpawnPoolTracker = hexIndicatorSpawnPoolTracker;
            _returnParent = returnParent;
            _selfDespawnSpawner = selfDespawner;
            _radiusDisplay = radiusPrefab;
            _unitEnemy = transform.GetComponent<UnitEnemy>();
            _damageMitigation = damageMitigation;
            _contactFilter = new ContactFilter2D
            {
                useLayerMask = true,
                layerMask = layerMask
            };

            Id = _transform.GetInstanceID() + (long)LegacyLayer.Idalmunon;

            HexIndicatorSpawnPoolTracker.OnHexIndicatorSpawn += OnHexIndicatorSpawned;
        }

        private void OnHexIndicatorSpawned(HexReturnData returndata)
        {
            if (returndata._enemyTransform != _transform)
            {
                return;
            }
            
            _indicator = returndata._hexInstance;
            var position = _transform.position;
            
            var indicatorTransform = _indicator.transform;
            indicatorTransform.position = position;
            indicatorTransform.rotation = Quaternion.identity;
            indicatorTransform.parent = _transform;
                
            _indicator.gameObject.SetActive(true);
            _indicator.name = _transform.gameObject.GetInstanceID().ToString();
            
            HexIndicatorSpawnPoolTracker.OnHexIndicatorSpawn -= OnHexIndicatorSpawned;
        }

        public void ApplyDamageMitigation(float newDamageMitigation)
        {
            var newStats = new EnemyStats
            {
                _damageMitigation = newDamageMitigation
            };

            _unitEnemy.ModifyStats(newStats);
            _previousDamageMitigation = newDamageMitigation;
        }

        public void OnHexActivate()
        {
            var hits = new Collider2D[50]; // todo set to max enemy count
            var amount = Physics2D.OverlapCircle(_transform.position, _radius, _contactFilter, hits);

            for (var i = 0; i < amount; ++i)
            {
                var hit = hits[i];

                if (!hit.TryGetComponent<IHittable>(out var hittable) 
                 || !hittable.IsHittable)
                {
                    return;
                }

                if (hit.TryGetComponent<IHexDamageBroadcaster>(out var hexDamager))
                {
                    hexDamager.BroadcastHexDamage(_damage);
                }

                if (!(hexDamager is UnitPlayer))
                {
                    hittable.Hit(_damage, _transform.position);
                }

                if (_radiusDisplay != null)
                {
                    var position = _transform.position;
                    var indicator = _selfDespawnSpawner.Spawn();

                    var indicatorTransform = indicator.transform;
                    indicatorTransform.position = position;
                    indicatorTransform.rotation = Quaternion.identity;
                    indicatorTransform.parent = null;
                    indicatorTransform.localScale = new Vector3(_radius, _radius, _radius);
                    indicator.SetReturnParent(_returnParent);
                    indicator.gameObject.SetActive(true);
                }
            }
        }

        public void OnHexApply()
        {
            if (_transform.TryGetComponent<IDeathEvent>(out var deathListener))
            {
                deathListener.OnUnitDeath += OnDeathThis;

                var sendData = new HexSendData
                {
                    _enemyTransform = _transform,
                    _hexPrefab = _indicatorPrefab
                };
                
                Dispatcher.SendMessageData(HexIndicatorSpawnPoolTracker.RequestHexIndicatorSpawn, sendData);
                ApplyDamageMitigation(_damageMitigation);
            }
        }

        public void OnHexRemove()
        {
            if (_transform // todo [jef] : double-check event sequence
              && _transform.TryGetComponent<IDeathEvent>(out var deathListener))
            {
                deathListener.OnUnitDeath -= OnDeathThis;
                _indicator.gameObject.SetActive(false);
                _indicator.transform.parent = _hexIndicatorSpawnPoolTracker.transform;

                RemoveDamageMitigation();
            }
        }

        public void OnHexRefresh(IHex newHex)
        {
            if (!(newHex is IdalmunonHex))
            {
                return;
            }
            
            var idalHex = (IdalmunonHex)newHex;
            _damageMitigation = idalHex._damageMitigation;
            _radius = idalHex._radius;

            RemoveDamageMitigation();
            ApplyDamageMitigation(_damageMitigation);
        }

        [Inject]
        private void Construct(DiContainer diContainer, LazyInject<HexSystem> hexSystem)
        {
            _hexSystem = hexSystem;
        }

        private void RemoveDamageMitigation()
        {
            var newStats = new EnemyStats
            {
                _damageMitigation = -_previousDamageMitigation
            };

            _unitEnemy.ModifyStats(newStats);
        }

        private void OnDeathThis()
        {
            _indicator.gameObject.SetActive(false);
            _indicator.transform.parent = _hexIndicatorSpawnPoolTracker.transform;
            
            _hexSystem.Value.OnHexRemoveEvent(this);
            OnHexActivate();
        }
    }
}
