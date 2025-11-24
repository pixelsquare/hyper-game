using Santelmo.Rinsurv.Backend;
using Zenject;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    [CreateAssetMenu(menuName = "Santelmo/Installers/Service Installer", fileName = "ServiceInstaller")]
    public class ServiceInstaller : ScriptableObjectInstaller
    {
        public override void InstallBindings()
        {
            InstallServices();
            InstallModules();
        }

        private void InstallServices()
        {
            Container.Bind<IAuthService>()
                     .To<FirebaseLoginService>()
                     .AsSingle()
                     .IfNotBound();

            Container.Bind<ICloudSyncService>()
                     .To<NullCloudSyncService>()
                     .AsSingle()
                     .IfNotBound();
        }

        private void InstallModules()
        {
            Container.Bind<IItemDataModule>()
                     .To<ItemDataModule>()
                     .AsSingle()
                     .IfNotBound();

            Container.Bind<LoadoutModule>()
                     .AsSingle()
                     .IfNotBound();

            Container.Bind<IInventoryModule>()
                     .To<InventoryModule>()
                     .AsCached()
                     .IfNotBound();

            Container.Bind<ICurrencyModule>()
                     .To<InventoryModule>()
                     .AsCached()
                     .IfNotBound();

            Container.Bind<EquipmentModule>()
                     .AsSingle()
                     .IfNotBound();
        }
    }
}
