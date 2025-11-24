using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public delegate void OnHexIndicatorSpawn(HexReturnData returnData);
    
    public class HexIndicatorSpawnPoolTracker : MonoBehaviour
    {
        private uint _amount;
        private DiContainer _diContainer;
        private Dictionary<HexIndicator, IMonoSpawnPool<HexIndicator>> _poolTable;

        public static string RequestHexIndicatorSpawn = "RequestHexIndicatorSpawn";

        public static OnHexIndicatorSpawn OnHexIndicatorSpawn;

        [Inject]
        private void Construct(DiContainer diContainer)
        {
            _diContainer = diContainer;
        }

        private void OnDespawn(HexIndicator unitSpawn)
        {
        }

        private HexIndicator Spawn(HexIndicator prefab)
        {
            if (_poolTable.TryGetValue(prefab, out var oldPool))
            {
                return oldPool.Spawn();
            }

            var newPool = new HexIndicatorSpawnPool(prefab, _diContainer, OnDespawn);
            _poolTable.Add(prefab, newPool);
            var spawn = newPool.Spawn();

            return spawn;
        }

        private void Awake()
        {
            _poolTable = new Dictionary<HexIndicator, IMonoSpawnPool<HexIndicator>>();
        }

        private void OnEnable()
        {
            Dispatcher.AddListener(RequestHexIndicatorSpawn, RequestHexIndicator);
        }

        private void OnDisable()
        {
            Dispatcher.RemoveListener(RequestHexIndicatorSpawn, RequestHexIndicator, true);
        }

        private void RequestHexIndicator(IMessage message)
        {
            var sentData = (HexSendData)message.Data;
            var spawned = Spawn(sentData._hexPrefab);

            var returnData = new HexReturnData
            {
                _enemyTransform = sentData._enemyTransform,
                _hexInstance = spawned
            };
            
            OnHexIndicatorSpawn?.Invoke(returnData);
        }
    }

    public struct HexReturnData
    {
        public Transform _enemyTransform;
        public HexIndicator _hexInstance;
    }
    
    public struct HexSendData
    {
        public Transform _enemyTransform;
        public HexIndicator _hexPrefab;
    }
}
