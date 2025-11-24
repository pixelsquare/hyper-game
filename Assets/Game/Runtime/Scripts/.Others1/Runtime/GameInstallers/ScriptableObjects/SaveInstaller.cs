using Zenject;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    [CreateAssetMenu(menuName = "Santelmo/Installers/Save Installer", fileName = "SaveInstaller")]
    public class SaveInstaller : ScriptableObjectInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<ISaveManager>()
                     .To<SaveManager>()
                     .AsSingle()
                     .IfNotBound();
        }
    }
}
