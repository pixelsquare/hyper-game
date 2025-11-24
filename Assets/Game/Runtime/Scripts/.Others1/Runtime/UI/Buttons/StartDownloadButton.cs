using UniRx;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    using UIEvent = GameEvents.UserInterface;

    [RequireComponent(typeof(RinawaButton))]
    public class StartDownloadButton : BaseButton
    {
        [SerializeField] private RinawaText _startDownloadText;
        [SerializeField] private AudioSource _buttonTapAudioSource;

        // Bypassing initialization of audio manager. Defaults to 1 on fresh install.
        private float SfxVolume => float.TryParse(_saveManager.Load("SfxVolume"), out var volume) ? volume : 1.0f;

        [Inject] private ISaveManager _saveManager;

        private readonly ReactiveProperty<bool> _buttonTextActiveProp = new(true);

        protected override void OnButtonClicked()
        {
            PlayButtonTapSound();
            _buttonTextActiveProp.Value = false;
            ButtonLoaderActiveProp.Value = true;
            Dispatcher.SendMessage(UIEvent.OnStartDownloadAssets);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            _buttonTextActiveProp.Subscribe(x => _startDownloadText.gameObject.SetActive(x))
                                 .AddTo(_compositeDisposable);
        }

        private void PlayButtonTapSound()
        {
            _buttonTapAudioSource.volume = SfxVolume;
            _buttonTapAudioSource.Play();
        }
    }
}
