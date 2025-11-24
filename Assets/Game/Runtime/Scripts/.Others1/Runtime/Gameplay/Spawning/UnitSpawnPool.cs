using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class UnitSpawnPool : IMonoSpawnPool<UnitSpawn>
    {
        public UnitSpawn Prefab { get; }
        public IEnumerable<UnitSpawn> Pool => _pool;

        private readonly List<UnitSpawn> _pool;
        private readonly DiContainer _container;
        private readonly UnityAction<UnitSpawn> _onDespawn;

        public UnitSpawnPool(UnitSpawn prefab, DiContainer container, UnityAction<UnitSpawn> onDespawn)
        {
            Prefab = prefab;
            _container = container;
            _pool = new List<UnitSpawn>();
            _onDespawn = onDespawn;
        }

        public UnitSpawn Spawn()
        {
            var spawn = Pool.FirstOrDefault(x => !x.gameObject.activeInHierarchy);

            if (spawn)
            {
                spawn.gameObject.SetActive(true);
            }
            else
            {
                spawn = _container.InstantiatePrefabForComponent<UnitSpawn>(Prefab);
                spawn.OnDespawnEvent += Despawn;
                _pool.Add(spawn);
            }

            return spawn;
        }

        public UnitSpawn Prepool(Transform parent)
        {
            var spawn = _container.InstantiatePrefabForComponent<UnitSpawn>(Prefab);
            spawn.gameObject.SetActive(false);
            spawn.OnDespawnEvent += Despawn;
            spawn.transform.parent = parent;
            _pool.Add(spawn);
            return spawn;
        }

        public void Despawn(UnitSpawn spawn)
        {
            spawn.gameObject.SetActive(false);
            _onDespawn?.Invoke(spawn);
        }
    }
}
