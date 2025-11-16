using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Kumu.Kulitan.Avatar
{
    [CreateAssetMenu(menuName = "Kumu/Ube/Avatar/Config/Eye")]
    public class AvatarEyeConfig : AvatarItemConfig, IAvatarItemWearable, IAvatarItemTintable
    {
        [SerializeField] private AssetReference materialRef;
        [SerializeField] private AssetReference texBaseRef;
        [SerializeField] private AssetReference texMaskRef;
        
        public async Task WearAvatarItem(IAvatarItemModelHandle colorizer)
        {
            var material = await AvatarAddressablesUtility.LoadAddressable<Material>(materialRef);
            var texBase = await AvatarAddressablesUtility.LoadAddressable<Texture>(texBaseRef);
            var texMask = await AvatarAddressablesUtility.LoadAddressable<Texture>(texMaskRef);
            colorizer.SetMaterial(material);
            colorizer.SetTexture(texBase, texMask);
        }

        public async Task RemoveAvatarItem(IAvatarItemModelHandle colorizer)
        {
            await AvatarAddressablesUtility.Release<Texture>(texBaseRef);
            await AvatarAddressablesUtility.Release<Texture>(texMaskRef);
        }

        public void TintAvatarItem(IAvatarItemModelHandle colorizer, Color color)
        {
            colorizer.SetColor(color);
        }

        public override string GetTypeCode()
        {
            return "E";
        }

        public override async Task<bool> TryDownloadAddressables()
        {
            var mat = await Addressables.DownloadDependenciesAsync(materialRef).Task;
            var texBase = await Addressables.DownloadDependenciesAsync(texBaseRef).Task;
            var texMask = await Addressables.DownloadDependenciesAsync(texMaskRef).Task;

            if (mat == null
                || texBase == null
                || texMask == null)
            {
                return false;
            }

            return true;
        }
    }
}
