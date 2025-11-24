using UnityEngine;

namespace Santelmo.Rinsurv
{
    public class KahangianHex : IHex, IHexApply, IHexRemove, IHexRefresh
    {
        public float Duration { get; }
        public long Id { get; }

        public Transform Transform => _transform;

        private readonly HexIndicator _indicatorPrefab;
        private readonly Transform _transform;

        private HexIndicator _indicator;
        private readonly UnitEnemy _unitEnemy;
        private readonly float _knockbackValue;
        private float _previousKnockbackValue;
        private HexIndicatorSpawnPoolTracker _hexIndicatorSpawnPoolTracker;

        public KahangianHex(Transform target, float duration, float knockbackValue, HexIndicator indicatorPrefab, HexIndicatorSpawnPoolTracker hexIndicatorSpawnPoolTracker)
        {
            _transform = target;
            Duration = duration;
            _indicatorPrefab = indicatorPrefab;
            _unitEnemy = target.GetComponent<UnitEnemy>();
            _knockbackValue = knockbackValue;
            _hexIndicatorSpawnPoolTracker = hexIndicatorSpawnPoolTracker;
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

        public void OnHexActivate()
        {
        }

        public void OnHexApply()
        {
            var sendData = new HexSendData
            {
                _enemyTransform = _transform,
                _hexPrefab = _indicatorPrefab
            };
                
            Dispatcher.SendMessageData(HexIndicatorSpawnPoolTracker.RequestHexIndicatorSpawn, sendData);
            ApplyExtraKnockback(_knockbackValue);
        }

        public void OnHexRemove()
        {
            if (_transform // todo [jef] : double-check event sequence
              && _transform.TryGetComponent<IDeathEvent>(out var deathListener))
            {
                _indicator.gameObject.SetActive(false);
                _indicator.transform.parent = _hexIndicatorSpawnPoolTracker.transform;
            }

            RemoveExtraKnockback();
        }

        public void OnHexRefresh(IHex newHex)
        {
        }

        public void ApplyExtraKnockback(float newKnockbackValue)
        {
            var newStats = new EnemyStats
            {
                _extraKnockback = newKnockbackValue
            };

            _unitEnemy.ModifyStats(newStats);
            _previousKnockbackValue = newKnockbackValue;
        }

        private void RemoveExtraKnockback()
        {
            var newStats = new EnemyStats
            {
                _extraKnockback = -_previousKnockbackValue
            };

            _unitEnemy.ModifyStats(newStats);
        }
    }
}
