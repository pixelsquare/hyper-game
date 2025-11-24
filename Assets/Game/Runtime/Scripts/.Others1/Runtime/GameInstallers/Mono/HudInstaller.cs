using Zenject;

namespace Santelmo.Rinsurv
{
    public class HudInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IHudManager>()
                     .To<HudManager>()
                     .AsSingle()
                     .WithArguments(transform.parent)
                     .IfNotBound();
        }
    }
}
