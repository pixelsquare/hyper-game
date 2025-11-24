using UniRx;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    using UIEvent = GameEvents.UserInterface;

    public class HeroLoadoutAvatar : MonoBehaviour
    {
        [SerializeField] private RinawaImage _avatarImage;
        [SerializeField] private GameObject _avatarLoader;

        [Header("Stats")]
        [SerializeField] private RinawaText _heroNameText;
        [SerializeField] private RinawaText _heroLevelText;
        [SerializeField] private RinawaText _heroHealthText;
        [SerializeField] private RinawaText _heroAttackText;

        [Inject] private ReactiveTextureProperty _avatarProp;

        private CompositeDisposable _compositeDisposable = new();

        private readonly ReactiveProperty<bool> _avatarLoaderActiveProp = new(false);
        private readonly ReactiveProperty<string> _heroNameProp = new(null);
        private readonly ReactiveProperty<string> _heroLevelProp = new(null);
        private readonly ReactiveProperty<string> _heroHealthProp = new(null);
        private readonly ReactiveProperty<string> _heroAttackProp = new(null);

        private void HandleActiveHeroSelected(IMessage message)
        {
            if (message.Data is not Hero hero)
            {
                return;
            }

            _avatarProp.Value = hero.IconSpriteName;
            _heroNameProp.Value = hero.Name;
            _heroLevelProp.Value = hero.HeroStats.Level.ToString();
            _heroHealthProp.Value = hero.HeroStats.Health.ToString();
            _heroAttackProp.Value = hero.HeroStats.Attack.ToString();
        }

        private void OnEnable()
        {
            Dispatcher.AddListener(UIEvent.OnActiveHeroSelected, HandleActiveHeroSelected, true);

            _avatarProp.Subscribe(x =>
            {
                _avatarImage.sprite = x;
                _avatarImage.gameObject.SetActive(x != null);
            }, () =>
            {
                _avatarLoaderActiveProp.Value = true;
            }, () =>
            {
                _avatarLoaderActiveProp.Value = false;
            }).AddTo(_compositeDisposable);

            _avatarLoaderActiveProp.Subscribe(x => _avatarLoader.SetActive(x))
                                   .AddTo(_compositeDisposable);

            _heroNameProp.Subscribe(x => _heroNameText.text = x)
                         .AddTo(_compositeDisposable);

            _heroLevelProp.Subscribe(x => _heroLevelText.text = x)
                          .AddTo(_compositeDisposable);

            _heroHealthProp.Subscribe(x => _heroHealthText.text = x)
                           .AddTo(_compositeDisposable);

            _heroAttackProp.Subscribe(x => _heroAttackText.text = x)
                           .AddTo(_compositeDisposable);
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
