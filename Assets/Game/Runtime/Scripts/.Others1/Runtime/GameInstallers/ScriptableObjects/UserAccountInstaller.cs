using Zenject;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    [CreateAssetMenu(menuName = "Santelmo/Installers/User Account Installer", fileName = "UserAccountInstaller")]
    public class UserAccountInstaller : ScriptableObjectInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IUserAccountManager>()
                     .To<UserAccountManager>()
                     .AsSingle();
        }
    }
}
