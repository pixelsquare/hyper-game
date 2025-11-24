using UniRx;
using UnityEngine;
using UnityEngine.Video;
using Zenject;

namespace Santelmo.Rinsurv
{
    using UIEvent = GameEvents.UserInterface;

    public class HeroLoadoutDetail : MonoBehaviour
    {
        [SerializeField] private RinawaText _heroDescriptionText;
        [SerializeField] private GameObject _heroEquippedObj;
        [SerializeField] private GameObject _heroUnequippedObj;

        [SerializeField] private VideoPlayer _heroPreviewPlayer;
        [SerializeField] private GameObject _heroPreviewOverlay;
        [SerializeField] private GameObject _heroPreviewPanel;

        [Inject] private IHeroLoadoutManager _heroLoadoutManager;

        private CompositeDisposable _compositeDisposable = new();

        private readonly ReactiveProperty<string> _heroDescriptionProp = new(null);
        private readonly ReactiveProperty<bool> _heroEquippedProp = new(false);
        private readonly ReactiveProperty<VideoClip> _heroPreviewClipProp = new(null);

        private void HandleActiveHeroSelected(IMessage message)
        {
            if (message.Data is not Hero hero)
            {
                return;
            }

            _heroDescriptionProp.Value = hero.Description;
            _heroPreviewClipProp.Value = hero.AttackPreviewClip;
            _heroEquippedProp.Value = _heroLoadoutManager.ActiveHero.Id.Equals(hero.Id);
        }

        private void OnEnable()
        {
            Dispatcher.AddListener(UIEvent.OnActiveHeroSelected, HandleActiveHeroSelected, true);

            _heroDescriptionProp.Subscribe(x => _heroDescriptionText.text = x)
                                .AddTo(_compositeDisposable);

            _heroEquippedProp.Subscribe(x =>
            {
                _heroEquippedObj.SetActive(x);
                _heroUnequippedObj.SetActive(!x);
            }).AddTo(_compositeDisposable);

            _heroPreviewClipProp.Subscribe(x =>
            {
                _heroPreviewPlayer.clip = x;
                _heroPreviewPanel.SetActive(x != null);
                _heroPreviewOverlay.SetActive(x == null);
            }).AddTo(_compositeDisposable);
        }

        private void OnDisable()
        {
            Dispatcher.RemoveListener(UIEvent.OnActiveHeroSelected, HandleActiveHeroSelected, true);
            _compositeDisposable.Clear();
        }

        private void OnDestroy()
        {
            _compositeDisposable.Dispose();
            _compositeDisposable = null;
        }
    }
}
