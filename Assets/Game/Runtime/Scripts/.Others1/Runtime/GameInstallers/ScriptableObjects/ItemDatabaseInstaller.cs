using Zenject;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    [CreateAssetMenu(menuName = "Santelmo/Installers/Item Database Installer", fileName = "ItemDatabaseInstaller")]
    public class ItemDatabaseInstaller : ScriptableObjectInstaller
    {
        [SerializeField] private ItemDatabase _itemDatabase;

        public override void InstallBindings()
        {
            Container.BindInstance(_itemDatabase)
                     .AsSingle();
        }
    }
}
