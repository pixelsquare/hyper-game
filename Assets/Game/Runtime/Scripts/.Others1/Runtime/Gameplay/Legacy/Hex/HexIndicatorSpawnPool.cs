using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class HexIndicatorSpawnPool : IMonoSpawnPool<HexIndicator>
    {
        public HexIndicator Prefab { get; private set; }
        public IEnumerable<HexIndicator> Pool => _indicatorPool;
        
        private List<HexIndicator> _indicatorPool;
        private DiContainer _diContainer;
        private readonly UnityAction<HexIndicator> _onDespawn;
        
        public HexIndicatorSpawnPool(HexIndicator prefab, DiContainer container, UnityAction<HexIndicator> onDespawn)
        {
            Prefab = prefab;
            _diContainer = container;
            _indicatorPool = new List<HexIndicator>();
            _onDespawn = onDespawn;
        }

        public HexIndicator Spawn()
        {
            var instance = Pool.FirstOrDefault(x => !x.gameObject.activeInHierarchy && x.Type == Prefab.Type);

            if (instance)
            {
                instance.gameObject.SetActive(true);
            }
            else
            {
                instance = _diContainer.InstantiatePrefabForComponent<HexIndicator>(Prefab);
                _indicatorPool.Add(instance);
            }

            return instance;
        }
        
        public void Despawn(HexIndicator spawn)
        {
            spawn.gameObject.SetActive(false);
        }
    }
}
