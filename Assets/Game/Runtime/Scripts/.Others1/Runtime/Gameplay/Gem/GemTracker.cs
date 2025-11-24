using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Santelmo.Rinsurv
{
    public class GemTracker : MonoBehaviour, IMonoSpawnPool<Gem>
    {
        [SerializeField] private SerializableDictionary<int, GameObject> _dropTable;
        [SerializeField] private uint _max;

        public delegate void OnConsume(Gem gem);
        public event OnConsume OnConsumeEvent;
        public Gem Prefab { get; private set; }
        public IEnumerable<Gem> Pool => _gemPool;

        private uint _current;
        private uint _expStack;
        private List<Gem> _gemPool;

        private DiContainer _diContainer;
        private IAudioManager _audioManager;
        private LazyInject<PickupSystem> _pickupSystem;

        [Inject]
        private void Construct(DiContainer diContainer, IAudioManager audioManager, LazyInject<PickupSystem> pickupSystem)
        {
            _diContainer = diContainer;
            _audioManager = audioManager;
            _pickupSystem = pickupSystem;
        }

        private void OnPickup(GameObject pickupObject)
        {
            if (pickupObject.TryGetComponent<Gem>(out var gem))
            {
                OnConsumeEvent?.Invoke(gem);
                _current--;

                _audioManager.PlaySound(Sfx.GemPickup);
            }
        }

        private void OnUnitDeath(Transform transform)
        {
            if (!transform.TryGetComponent<GemDropper>(out var gemDropper))
            {
                return;
            }

            var value = (uint)Random.Range(gemDropper.MinValue, gemDropper.MaxValue);

            if (_current >= _max)
            {
                _expStack += value;
            }
            else
            {
                var finalValue = value + _expStack;
                Prefab = SelectPrefab(finalValue).GetComponent<Gem>();
                
                var position = transform.transform.position;
                
                var drop = Spawn();
                var dropTransform = drop.transform;
                dropTransform.position = position;
                dropTransform.rotation = Quaternion.identity;
                dropTransform.parent = this.transform;

                if (drop.TryGetComponent<Gem>(out var gem))
                {
                    gem.Init(finalValue);
                }
                if (drop.TryGetComponent<Pickup>(out var pickup))
                {
                    pickup.Init();
                }

                _expStack = 0;
                _current++;
            }
        }
        
        public Gem Spawn()
        {
            var instance = Pool.FirstOrDefault(x => !x.gameObject.activeInHierarchy && x.Type == Prefab.Type);

            if (instance)
            {
                instance.gameObject.SetActive(true);
            }
            else
            {
                instance = _diContainer.InstantiatePrefabForComponent<Gem>(Prefab);
                instance.GetComponent<Pickup>().OnObjectSpawn += gem => Despawn(gem.GetComponent<Gem>());
                _gemPool.Add(instance);
            }

            return instance;
        }
        
        public void Despawn(Gem spawn)
        {
            spawn.gameObject.SetActive(false);
        }

        private GameObject SelectPrefab(uint value)
        {
            foreach (var pair in _dropTable)
            {
                if (value > pair.Key)
                {
                    return pair.Value;
                }
            }

            return null;
        }

        private void Awake()
        {
            _gemPool = new List<Gem>();
        }

        private void OnEnable()
        {
            _pickupSystem.Value.OnPickupEvent += OnPickup;
            UnitEnemy._onUnitDeathGlobal += OnUnitDeath;
        }

        private void OnDisable()
        {
            _pickupSystem.Value.OnPickupEvent -= OnPickup;
            UnitEnemy._onUnitDeathGlobal -= OnUnitDeath;
        }
    }
}
