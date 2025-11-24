using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class OverlayUiInstaller : MonoInstaller
    {
        [SerializeField] private GameObject _overlayPrefab;

        public override void InstallBindings()
        {
            Container.Bind<Overlay>()
                     .FromComponentInNewPrefab(_overlayPrefab)
                     .UnderTransform(transform.parent)
                     .AsSingle()
                     .OnInstantiated<Overlay>((ctx, overlay) => overlay.gameObject.SetActive(false))
                     .WhenInjectedInto<OverlayManager>()
                     .NonLazy();

            Container.Bind<IOverlayManager>()
                     .To<OverlayManager>()
                     .AsSingle()
                     .IfNotBound();
        }
    }
}
