using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class HealthBarInstaller : MonoInstaller
    {
        [SerializeField] private Transform _parentTransform;
        [SerializeField] private HealthBar _healthBarPrefab;

        public override void InstallBindings()
        {
            Container.Bind<IHealthBarManager>()
                     .To<HealthBarManager>()
                     .AsSingle()
                     .WithArguments(_healthBarPrefab, _parentTransform)
                     .IfNotBound();
        }
    }
}
