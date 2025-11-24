using Zenject;

namespace Santelmo.Rinsurv
{
    public class ShopHud : BaseHud
    {
        [Inject] private IAudioManager _audioManager;

        private void OnEnable()
        {
            _audioManager.PlaySound(Bgm.Shop);
        }
    }
}
