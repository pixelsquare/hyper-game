using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;

namespace Kumu.Kulitan.Gifting
{
    public class VirtualGiftDatabaseLoader : MonoBehaviour
    {
        [SerializeField] private UnityEvent onLoaded;
        [SerializeField] private AssetReferenceT<VirtualGiftDatabase> dbRef;
        private static bool doneLoading;
        
        public static bool DoneLoading => doneLoading;

        public async void Load()
        {
            doneLoading = false;
            await VirtualGiftDatabase.Load(dbRef);
            onLoaded?.Invoke();
            doneLoading = true;
        }

        public void OnDestroy()
        {
            doneLoading = false;
        }
    }
}
