using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class SoloSpawnPool : MonoBehaviour, IMonoSpawnPool<SelfDespawn>
    {
        [SerializeField] private SelfDespawn _spawnedObject;
        
        public SelfDespawn Prefab { get; private set; }
        public IEnumerable<SelfDespawn> Pool => _selfDespawnerPool;
        
        private List<SelfDespawn> _selfDespawnerPool;
        private DiContainer _diContainer;
        
        [Inject]
        private void Construct(DiContainer diContainer)
        {
            _diContainer = diContainer;
        }
        
        public SelfDespawn Spawn()
        {
            var instance = Pool.FirstOrDefault(x => !x.gameObject.activeInHierarchy);

            if (instance)
            {
                instance.gameObject.SetActive(true);
            }
            else
            {
                instance = _diContainer.InstantiatePrefabForComponent<SelfDespawn>(Prefab);
                _selfDespawnerPool.Add(instance);
            }

            return instance;
        }
        
        public void Despawn(SelfDespawn spawn)
        {
            spawn.gameObject.SetActive(false);
        }

        private void Awake()
        {
            Prefab = _spawnedObject;
            _selfDespawnerPool = new List<SelfDespawn>();
        }
    }
}
