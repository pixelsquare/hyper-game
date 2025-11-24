using UniRx;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class InGamePauseHero : MonoBehaviour
    {
        [SerializeField] private RinawaImage _heroAvatarImage;
        [SerializeField] private GameObject _heroAvatarLoader;

        [SerializeField] private RinawaText _heroNameText;
        [SerializeField] private RinawaText _heroLevelText;

        [Header("Stats")]
        [SerializeField] private RinawaText _healthText;
        [SerializeField] private RinawaText _attackText;

        [Inject] private ReactiveTextureProperty _heroAvatarProp;
        [Inject] private IHeroLoadoutManager _heroLoadoutManager;

        private CompositeDisposable _compositeDisposable = new();

        private readonly ReactiveProperty<bool> _avatarLoaderActiveProp = new(false);

        private readonly ReactiveProperty<string> _heroNameProp = new(null);
        private readonly ReactiveProperty<int> _heroLevelProp = new(0);
        private readonly ReactiveProperty<int> _heroHealthProp = new(0);
        private readonly ReactiveProperty<int> _heroAttackProp = new(0);

        private void OnEnable()
        {
            _avatarLoaderActiveProp.Subscribe(x => _heroAvatarLoader.SetActive(x))
                                   .AddTo(_compositeDisposable);

            _heroNameProp.Subscribe(x => _heroNameText.text = x)
                         .AddTo(_compositeDisposable);

            _heroLevelProp.Subscribe(x => _heroLevelText.text = x.ToString())
                          .AddTo(_compositeDisposable);

            _heroHealthProp.Subscribe(x => _healthText.text = x.ToString())
                           .AddTo(_compositeDisposable);

            _heroAttackProp.Subscribe(x => _attackText.text = x.ToString())
                           .AddTo(_compositeDisposable);

            _heroAvatarProp.Subscribe(x =>
            {
                _heroAvatarImage.sprite = x;
                _heroAvatarImage.gameObject.SetActive(x != null);
            }, () =>
            {
                _avatarLoaderActiveProp.Value = true;
            }, () =>
            {
                _avatarLoaderActiveProp.Value = false;
            }).AddTo(_compositeDisposable);
        }

        private void OnDisable()
        {
            _compositeDisposable.Clear();
        }

        private void OnDestroy()
        {
            _compositeDisposable.Dispose();
            _compositeDisposable = null;
        }

        private void Start()
        {
            var activeHero = _heroLoadoutManager.ActiveHero;

            if (activeHero == null)
            {
                return;
            }

            _heroNameProp.Value = activeHero.Name;
            _heroAvatarProp.Value = activeHero.IconSpriteName;
            _heroLevelProp.Value = activeHero.HeroStats.Level;
            _heroHealthProp.Value = activeHero.HeroStats.Health;
            _heroAttackProp.Value = activeHero.HeroStats.Attack;
        }
    }
}
