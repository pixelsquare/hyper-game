using UniRx;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class PlayerAvatarLoader : MonoBehaviour
    {
        [SerializeField] private RinawaImage _playerAvatarImage;
        [SerializeField] private GameObject _avatarLoader;

        [Inject] private IHeroLoadoutManager _heroLoadoutManager;
        [Inject] private ReactiveTextureProperty _avatarProp;

        private CompositeDisposable _compositeDisposable = new();

        private readonly ReactiveProperty<bool> _loaderActiveProp = new(false);

        private void OnEnable()
        {
            _avatarProp.Subscribe(x =>
            {
                _playerAvatarImage.sprite = x;
                _playerAvatarImage.gameObject.SetActive(x != null);
            }, () =>
            {
                _loaderActiveProp.Value = true;
            }, () =>
            {
                _loaderActiveProp.Value = false;
            }).AddTo(_compositeDisposable);

            _loaderActiveProp.Subscribe(x => _avatarLoader.SetActive(x))
                             .AddTo(_compositeDisposable);
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
            _avatarProp.Value = _heroLoadoutManager.ActiveHero.IconSpriteName;
        }
    }
}
