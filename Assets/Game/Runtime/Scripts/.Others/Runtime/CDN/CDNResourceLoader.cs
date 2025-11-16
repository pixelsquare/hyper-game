using System;
using UnityEngine;
using UnityEngine.Events;

#if ADDRESSABLES_ENABLED
using UnityEngine.AddressableAssets;
#endif

namespace Kumu.Kulitan.CDN
{
    [Serializable]
    public class OnModelSpawnedEvent : UnityEvent<GameObject>
    {
    }

    public class CDNResourceLoader : MonoBehaviour
    {
        [SerializeField] private OnModelSpawnedEvent OnModelSpawned = null;
        [SerializeField] private UnityEvent OnModelDestroyed = null;

#if ADDRESSABLES_ENABLED
        [SerializeField] private AssetReference modelObj = null;

#if UNITY_EDITOR
        [SerializeField] private bool drawGizmos = true;
        private MeshFilter cachedMeshFilter = null;
        private SkinnedMeshRenderer cachedSkinnedRenderer = null;
#endif

        private async void Start()
        {
            var obj = await modelObj.LoadAssetAsync<GameObject>().Task;
            obj = Instantiate(obj, transform.parent);
            OnModelSpawned?.Invoke(obj);
        }

        private void OnDestroy()
        {
            modelObj.ReleaseAsset();
            OnModelDestroyed?.Invoke();
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (modelObj?.editorAsset is not GameObject editorAssetObj || !drawGizmos || Application.isPlaying)
            {
                return;
            }

            if (cachedMeshFilter == null && cachedSkinnedRenderer == null)
            {
                // Get component once until we get mesh unless there's no mesh.
                cachedMeshFilter = editorAssetObj.GetComponentInChildren<MeshFilter>();
                cachedSkinnedRenderer = editorAssetObj.GetComponentInChildren<SkinnedMeshRenderer>();
            }

            if (cachedSkinnedRenderer != null) // Prioritize skinned renderer over mesh filter.
            {
                Gizmos.DrawMesh(cachedSkinnedRenderer.sharedMesh, transform.position, transform.rotation);
            }
            else if (cachedMeshFilter != null)
            {
                Gizmos.DrawMesh(cachedMeshFilter.sharedMesh, transform.position, transform.rotation);
            }
        }
#endif
#endif
    }
}
