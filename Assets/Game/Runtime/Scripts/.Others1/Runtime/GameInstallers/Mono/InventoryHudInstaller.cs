using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class InventoryHudInstaller : MonoInstaller
    {
        [SerializeField] private InventoryHud _inventoryHud;
        [SerializeField] private InventoryItemPopulator _inventoryItemPopulator;
        [SerializeField] private InventoryItemDetail _inventoryItemDetail;

        public override void InstallBindings()
        {
            Container.BindInstance(_inventoryHud)
                     .AsSingle()
                     .IfNotBound();

            Container.BindInstance(_inventoryItemPopulator)
                     .AsSingle()
                     .IfNotBound();

            Container.BindInstance(_inventoryItemDetail)
                     .AsSingle()
                     .IfNotBound();
        }
    }
}
