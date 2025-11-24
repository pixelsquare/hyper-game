using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class PopupUiInstaller : MonoInstaller
    {
        [SerializeField] private GameObject _popupOverlay;
    
        public override void InstallBindings()
        {
            Container.BindInstance(_popupOverlay)
                     .AsSingle()
                     .WhenInjectedInto<PopupManager>()
                     .NonLazy();
            
            Container.Bind<IPopupManager>()
                     .To<PopupManager>()
                     .AsSingle()
                     .WithArguments(transform.parent)
                     .IfNotBound();
        }
    }
}
