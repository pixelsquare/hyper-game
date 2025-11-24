using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public enum SpawnType
    {
        Mob,
        Elite,
        Boss,
    }
    
    public struct SpawnPrefabData
    {
        public UnitSpawn _prefab;
        public int _amount;
    }

    public class SpawnPrefabComparer : IEqualityComparer<SpawnPrefabData>
    {
        public bool Equals(SpawnPrefabData x, SpawnPrefabData y)
        {
            return x._prefab.GetHashCode() == y._prefab.GetHashCode();
        }

        public int GetHashCode(SpawnPrefabData obj)
        {
            return obj._prefab.GetHashCode();
        }
    }
    
    public class Spawner : MonoBehaviour, IGameplaySystem
    {
        [SerializeField] private int _mobMax = 1;        
        
        private DiContainer _diContainer;
        private LazyInject<WaveSpawner> _waveSpawner;
        private Dictionary<SpawnType, UnitSpawnTracker> _spawnTrackerTable;
        private Dictionary<UnitSpawn, IMonoSpawnPool<UnitSpawn>> _poolTable;
        private IMissionManager _missionManager;        

        public int GameplaySystemInitPriority => 0;

        public async UniTask GameplaySystemInitAsync()
        {
            Prepool();
        }

        [Inject]
        private void Construct(DiContainer diContainer, LazyInject<WaveSpawner> waveSpawner, IMissionManager missionManager)
        {
            _diContainer = diContainer;
            _waveSpawner = waveSpawner;
            _missionManager = missionManager;
        }

        private void OnDespawn(UnitSpawn unitSpawn)
        {
            var spawnType = unitSpawn.SpawnType;
            var spawnTracker = _spawnTrackerTable[spawnType];
            spawnTracker.OnDespawn(unitSpawn);
        }

        private void OnDeath(Transform transform)
        {
            if (transform.TryGetComponent<UnitSpawn>(out var spawn))
            {
                QuadTreeInstance.Quadtree.Remove(spawn.gameObject);
                spawn.Despawn();
            }
        }

        private void OnRequestSpawn(SpawnData[] spawnData)
        {
            foreach (var spawnDatum in spawnData)
            {
                var spawnType = spawnDatum.spawnType;
                var spawnTracker = _spawnTrackerTable[spawnType];
                
                foreach (var spawnPoint in spawnDatum.spawnPoints)
                {
                    if (!spawnTracker.CanSpawn())
                    {
                        break;
                    }
                    
                    var spawn = Spawn(spawnDatum.prefab, spawnPoint);
                    spawn.name = spawn.GetInstanceID().ToString();
                    spawn.Init(spawnDatum.spawnType);
                    
                    var spawnTransform = spawn.transform;
                    spawnTransform.position = spawnPoint;
                    spawnTransform.rotation = Quaternion.identity;
                    
                    QuadTreeInstance.Quadtree.Remove(spawn.gameObject);
                    spawnTracker.OnSpawn(spawn);
                }
            }
        }

        private UnitSpawn Spawn(UnitSpawn prefab, Vector3 spawnPoint)
        {
            var spawn = _poolTable[prefab].Spawn();
            spawn.transform.parent = transform;

            return spawn;
        }

        private void Prepool()
        {
            var spawnPrefabDataSet = new HashSet<SpawnPrefabData>(new SpawnPrefabComparer());
            
            foreach (var waveConfig in _missionManager.ActiveMissionLevel.GameLevel._enemyWaves)
            {
                if (waveConfig is IWaveInitial waveInitial)
                {
                    foreach (var wave in waveInitial.GetInitialWave(Vector2.zero))
                    {
                        var spawnPrefabData = new SpawnPrefabData()
                        {
                            _prefab = wave.prefab,
                            _amount = wave.spawnPoints.Length,
                        };

                        spawnPrefabDataSet.Add(spawnPrefabData);
                    }
                }

                if (waveConfig is IWaveInterval waveInterval)
                {
                    foreach (var wave in waveInterval.GetIntervalWave(Vector2.zero))
                    {
                        var spawnPrefabData = new SpawnPrefabData()
                        {
                            _prefab = wave.prefab,
                            _amount = wave.spawnPoints.Length,
                        };

                        spawnPrefabDataSet.Add(spawnPrefabData);
                    }
                }
            }

            foreach (var spawnPrefabData in spawnPrefabDataSet)
            {
                var prefab = spawnPrefabData._prefab;
                var unitSpawnPool = new UnitSpawnPool(prefab, _diContainer, OnDespawn);
                _poolTable.Add(prefab, unitSpawnPool);

                for (var i = spawnPrefabData._amount; i > -1; i--)
                {
                    unitSpawnPool.Prepool(transform);
                }
            }
        }

        private void Awake()
        {
            // Create quad tree instance, then attach to object
            gameObject.AddComponent<QuadTreeInstance>();
            
            _spawnTrackerTable = new Dictionary<SpawnType, UnitSpawnTracker>
            {
                { SpawnType.Mob, new MobSpawnTracker(_mobMax) },
                { SpawnType.Elite, new EliteSpawnTracker() },
                { SpawnType.Boss, new BossSpawnTracker() },
            };

            _poolTable = new Dictionary<UnitSpawn, IMonoSpawnPool<UnitSpawn>>();
        }

        private void OnEnable()
        {
            UnitEnemy._onUnitDeathGlobal += OnDeath;
            _waveSpawner.Value.OnRequestSpawnEvent += OnRequestSpawn;
        }

        private void OnDisable()
        {
            UnitEnemy._onUnitDeathGlobal -= OnDeath;
            _waveSpawner.Value.OnRequestSpawnEvent -= OnRequestSpawn;
        }
    }
}
