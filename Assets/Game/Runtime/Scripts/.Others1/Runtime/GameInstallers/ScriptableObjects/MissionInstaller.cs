using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    [CreateAssetMenu(menuName = "Santelmo/Installers/Mission Installer", fileName = "MissionInstaller")]
    public class MissionInstaller : ScriptableObjectInstaller
    {
        [SerializeField] private MissionDatabase _missionDatabase;

        public override void InstallBindings()
        {
            Container.BindInstance(_missionDatabase)
                     .AsSingle()
                     .WhenInjectedInto<MissionManager>()
                     .IfNotBound();

            Container.Bind<IMissionManager>()
                     .To<MissionManager>()
                     .AsSingle()
                     .IfNotBound();
        }
    }
}
