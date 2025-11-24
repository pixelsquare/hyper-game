using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Santelmo.Rinsurv
{
    [CreateAssetMenu(menuName = "Santelmo/Installers/Popup Factory Installer", fileName = "PopupFactoryInstaller")]
    public class PopupFactoryInstaller : ScriptableObjectInstaller, IInitializable
    {
        [SerializeField] private AssetReferenceGameObject _genericPopupRef;
        [SerializeField] private AssetReferenceGameObject _itemComparisonPopupRef;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<PopupFactoryInstaller>()
                     .AsSingle();

            Container.Bind<Dictionary<PopupType, AssetReferenceGameObject>>()
                     .FromMethod(CreateMapping)
                     .AsSingle()
                     .WhenInjectedInto<PopupFactory>();

            Container.BindFactory<PopupType, Transform, BasePopup, PopupFactory>()
                     .AsSingle()
                     .IfNotBound();
        }

        public void Initialize()
        {
            Container.TryResolve<PopupFactory>()?
                   .Initialize();
        }

        private Dictionary<PopupType, AssetReferenceGameObject> CreateMapping()
        {
            var mapping = new Dictionary<PopupType, AssetReferenceGameObject>();
            mapping[PopupType.Generic] = _genericPopupRef;
            mapping[PopupType.ItemComparison] = _itemComparisonPopupRef;
            return mapping;
        }
    }
}
