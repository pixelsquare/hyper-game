using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;

namespace Kumu.Kulitan.Common
{
    public class AddressablePrefab : MonoBehaviour 
    {
        [SerializeField] private UnityEvent<GameObject> onSpawn;
        [SerializeField] private AssetReferenceGameObject prefabRef;
        [SerializeField] private bool spawnOnStart = true;

        private GameObject instance;

        public GameObject Instance => instance;

        public async void Spawn()
        {
            instance = await prefabRef.InstantiateAsync(transform.parent).Task;
            onSpawn?.Invoke(instance);
        }

        protected virtual void Start()
        {
            if (spawnOnStart)
            {
                Spawn();
            }
        }
    }
}
