using Zenject;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    [CreateAssetMenu(menuName = "Santelmo/Installers/Asset Loader Installer", fileName = "AssetLoaderInstaller")]
    public class AssetLoaderInstaller : ScriptableObjectInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IAssetLoaderManager>()
                     .To<AssetLoaderManager>()
                     .AsSingle()
                     .IfNotBound();
        }
    }
}
