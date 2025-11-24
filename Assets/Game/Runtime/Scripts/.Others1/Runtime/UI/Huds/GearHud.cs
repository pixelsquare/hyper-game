using Zenject;

namespace Santelmo.Rinsurv
{
    public class GearHud : BaseHud
    {
        [Inject] private IAudioManager _audioManager;

        private void OnEnable()
        {
            _audioManager.PlaySound(Bgm.Equipment);
        }
    }
}
