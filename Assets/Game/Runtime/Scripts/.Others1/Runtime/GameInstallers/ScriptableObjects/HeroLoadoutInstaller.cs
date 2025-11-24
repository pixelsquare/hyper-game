using Zenject;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    [CreateAssetMenu(menuName = "Santelmo/Installers/Hero Loadout Installer", fileName = "HeroLoadoutInstaller")]
    public class HeroLoadoutInstaller : ScriptableObjectInstaller
    {
        [SerializeField] private HeroDatabase _heroDatabase;

        public override void InstallBindings()
        {
            Container.BindInstance(_heroDatabase)
                     .AsSingle();

            Container.Bind<IHeroLoadoutManager>()
                     .To<HeroLoadoutManager>()
                     .AsSingle()
                     .IfNotBound();
        }
    }
}
