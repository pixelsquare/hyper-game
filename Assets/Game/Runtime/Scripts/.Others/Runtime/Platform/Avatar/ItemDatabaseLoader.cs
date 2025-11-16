using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;

namespace Kumu.Kulitan.Avatar
{
    public class ItemDatabaseLoader : MonoBehaviour
    {
        [SerializeField] private UnityEvent onLoaded;
        [SerializeField] private AssetReferenceT<ItemDatabase> dbRef;

        /// <summary>
        /// Used by Fsm.
        /// </summary>
        public async void Load()
        {
            await ItemDatabase.Load(dbRef);
            onLoaded.Invoke();
        }
    }
}
