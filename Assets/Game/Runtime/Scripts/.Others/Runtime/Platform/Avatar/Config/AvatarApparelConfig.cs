using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Kumu.Kulitan.Hangout;

namespace Kumu.Kulitan.Avatar
{
    public abstract class AvatarApparelConfig : AvatarItemConfig, IAvatarItemWearable
    {
        [SerializeField] protected AssetReference meshRef;
        [SerializeField] protected AssetReference materialRef;
        [SerializeField] protected AssetReferenceTexture textureRef;
        [SerializeField] protected AudioClipConfig audioClipConfig;
        
        public AudioClipConfig ClipConfig => audioClipConfig;
                    
        public async Task WearAvatarItem(IAvatarItemModelHandle colorizer)
        {
            var mesh = await AvatarAddressablesUtility.LoadAddressable<Mesh>(meshRef);
            var material = await AvatarAddressablesUtility.LoadAddressable<Material>(materialRef);
            var texture = await AvatarAddressablesUtility.LoadAddressable<Texture>(textureRef);

            colorizer.SetMesh(mesh);
            colorizer.SetMaterial(material);
            colorizer.SetTexture(texture);
        }

        public async Task RemoveAvatarItem(IAvatarItemModelHandle colorizer)
        {
            colorizer.SetMesh(null);
            await AvatarAddressablesUtility.Release<Mesh>(meshRef);
            await AvatarAddressablesUtility.Release<Texture>(textureRef);
        }

        public override async Task<bool> TryDownloadAddressables()
        {
            var mesh = await Addressables.DownloadDependenciesAsync(meshRef).Task;
            var mat = await Addressables.DownloadDependenciesAsync(materialRef).Task;
            var tex = await Addressables.DownloadDependenciesAsync(textureRef).Task;

            if (mesh == null
                || mat == null
                || tex == null)
            {
                return false;
            }

            return true;
        }
    }
}
