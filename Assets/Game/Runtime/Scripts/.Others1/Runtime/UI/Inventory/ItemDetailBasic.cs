using UniRx;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class ItemDetailBasic : MonoBehaviour
    {
        [SerializeField] private GameObject _avatarLoader;
        [SerializeField] private RinawaImage _avatarImage;
        [SerializeField] private RinawaText _itemNameText;
        [SerializeField] private RinawaText _itemTypeText;

        [Inject] private ReactiveTextureProperty _itemAvatarProp;

        private CompositeDisposable _compositeDisposable = new();

        private readonly ReactiveProperty<bool> _avatarLoaderProp = new(false);
        private readonly ReactiveProperty<string> _itemNameProp = new(null);
        private readonly ReactiveProperty<string> _itemTypeProp = new(null);

        public void Setup(IItem item)
        {
            _itemAvatarProp.Value = item.IconSpriteName;
            _itemNameProp.Value = item.Name;
            _itemTypeProp.Value = item.ItemTypeName;
        }

        private void OnEnable()
        {
            _avatarLoaderProp.Subscribe(x => _avatarLoader.SetActive(x))
                             .AddTo(_compositeDisposable);

            _itemAvatarProp.Subscribe(x =>
            {
                _avatarImage.sprite = x;
                _avatarImage.gameObject.SetActive(x != null);
            }, () =>
            {
                _avatarLoaderProp.Value = true;
            }, () =>
            {
                _avatarLoaderProp.Value = false;
            });

            _itemNameProp.Subscribe(x =>
            {
                _itemNameText.text = x;
                _itemNameText.gameObject.SetActive(!string.IsNullOrEmpty(x));
            }).AddTo(_compositeDisposable);

            _itemTypeProp.Subscribe(x =>
            {
                _itemTypeText.text = x;
                _itemTypeText.gameObject.SetActive(!string.IsNullOrEmpty(x));
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

            _itemAvatarProp.Dispose();
            _itemAvatarProp = null;
        }
    }
}
