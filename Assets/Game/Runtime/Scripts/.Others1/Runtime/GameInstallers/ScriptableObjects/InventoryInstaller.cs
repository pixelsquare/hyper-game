using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    [CreateAssetMenu(menuName = "Santelmo/Installers/Inventory Installer", fileName = "InventoryInstaller")]
    public class InventoryInstaller : ScriptableObjectInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IInventoryManager>()
                     .To<InventoryManager>()
                     .AsSingle();
        }
    }
}
