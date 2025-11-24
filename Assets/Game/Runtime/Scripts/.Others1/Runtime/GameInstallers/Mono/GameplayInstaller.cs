using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class GameplayInstaller : MonoInstaller
    {
        [SerializeField] private GameObject _joystickControllerPrefab;

        public override void InstallBindings()
        {
            Container.Bind<JoystickController>()
                     .FromComponentInNewPrefab(_joystickControllerPrefab)
                     .AsSingle()
                     .OnInstantiated<JoystickController>(OnJoystickInstantiated)
                     .NonLazy()
                     .IfNotBound();
        }

        private void OnJoystickInstantiated(InjectContext context, JoystickController joystick)
        {
            joystick.transform.parent.SetSiblingIndex(0);
        }
    }
}
