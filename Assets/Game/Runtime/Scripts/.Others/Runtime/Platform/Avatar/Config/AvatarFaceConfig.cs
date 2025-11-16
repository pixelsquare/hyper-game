using System.Threading.Tasks;
using Kumu.Kulitan.Avatar;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Kumu.Kulitan.Avatar
{
    public abstract class AvatarFaceConfig : AvatarItemConfig, IAvatarItemWearable
    {
        [SerializeField] protected AssetReference materialRef;
        [SerializeField] protected AssetReferenceTexture textureRef;
        
        public async Task WearAvatarItem(IAvatarItemModelHandle colorizer)
        {
            var material = await AvatarAddressablesUtility.LoadAddressable<Material>(materialRef);
            var texture = await AvatarAddressablesUtility.LoadAddressable<Texture>(textureRef);

            colorizer.SetMaterial(material);
            colorizer.SetTexture(texture);
        }

        public async Task RemoveAvatarItem(IAvatarItemModelHandle colorizer)
        {
            await AvatarAddressablesUtility.Release<Texture>(textureRef);
        }

        public override async Task<bool> TryDownloadAddressables()
        {
            var mat = await Addressables.DownloadDependenciesAsync(materialRef).Task;
            var tex = await Addressables.DownloadDependenciesAsync(textureRef).Task;

            if (mat == null
                || tex == null)
            {
                return false;
            }

            return true;
        }
    }
}
