using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    [RequireComponent(typeof(RinawaButton))]
    public class PlaySfxButton : BaseButton
    {
        [SerializeField] private Sfx _sfxType;
        [Inject] private IAudioManager _audioManager;

        protected override void OnButtonClicked()
        {
            _audioManager?.PlaySound(_sfxType);
        }
    }
}
