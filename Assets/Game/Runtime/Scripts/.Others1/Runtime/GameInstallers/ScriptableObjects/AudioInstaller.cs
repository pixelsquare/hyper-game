using Zenject;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    [CreateAssetMenu(menuName = "Santelmo/Installers/Audio Installer", fileName = "AudioInstaller")]
    public class AudioInstaller : ScriptableObjectInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IAudioManager>()
                     .To<AudioManager>()
                     .AsSingle()
                     .IfNotBound();
        }
    }
}
