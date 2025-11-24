using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class ItemDetailAdvanced : MonoBehaviour
    {
        [SerializeField] private ItemStatPopulator _itemStatPopulator;

        [SerializeField] private GameObject _avatarLoader;
        [SerializeField] private RinawaImage _avatarImage;
        [SerializeField] private RinawaImage _frameImage;
        [SerializeField] private RinawaImage _iconTypeImage;
        [SerializeField] private RinawaText _itemNameText;
        [SerializeField] private RinawaText _itemTypeText;
        [SerializeField] private RinawaText _itemRarityText;
        [SerializeField] private RinawaText _itemQualityText;

        [SerializeField] private SerializableDictionary<EquipmentRarity, Sprite> _frameMap;

        [Inject] private ReactiveTextureProperty _itemAvatarProp;

        private CompositeDisposable _compositeDisposable = new();

        private readonly ReactiveProperty<bool> _avatarLoaderProp = new(false);
        private readonly ReactiveProperty<string> _nameProp = new(null);
        private readonly ReactiveProperty<string> _typeProp = new(null);
        private readonly ReactiveProperty<Sprite> _iconTypeProp = new(null);

        private readonly ReactiveProperty<EquipmentRarity> _rarityProp = new(EquipmentRarity.None);
        private readonly ReactiveProperty<string> _qualityProp = new(null);

        public void Setup(IItem item)
        {
            var itemStats = new List<ItemStats>();
            GameUtil.GetItemStatsNonAlloc(item, itemStats);
            Setup(item, itemStats);
        }

        public void Setup(IItem item, IEnumerable<ItemStats> itemStats)
        {
            if (item is not Emblem emblem)
            {
                return;
            }

            var equipmentProps = emblem.EquipmentProperties;

            _itemAvatarProp.Value = item.IconSpriteName;
            _nameProp.Value = equipmentProps.Level > 0 ? $"{item.Name} +{equipmentProps.Level}" : item.Name;
            _typeProp.Value = item.ItemTypeName;
            _iconTypeProp.Value = emblem.IconTypeSprite;
            _rarityProp.Value = emblem.Rarity;
            _qualityProp.Value = $"{((EquipmentQuality)equipmentProps.Quality).ToString()} Quality";
            _itemStatPopulator.Setup(itemStats);
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

            _nameProp.Subscribe(x =>
            {
                _itemNameText.text = x;
                _itemNameText.gameObject.SetActive(!string.IsNullOrEmpty(x));
            }).AddTo(_compositeDisposable);

            _typeProp.Subscribe(x =>
            {
                _itemTypeText.text = x;
                _itemTypeText.gameObject.SetActive(!string.IsNullOrEmpty(x));
            }).AddTo(_compositeDisposable);

            _iconTypeProp.Subscribe(x =>
            {
                _iconTypeImage.sprite = x;
                _iconTypeImage.gameObject.SetActive(x != null);
            }).AddTo(_compositeDisposable);

            _rarityProp.Subscribe(x =>
            {
                var rarity = x.ToString();
                _itemRarityText.text = rarity;
                _itemRarityText.gameObject.SetActive(!string.IsNullOrEmpty(rarity));

                if (_frameMap.TryGetValue(x, out var frameSprite))
                {
                    _frameImage.sprite = frameSprite;
                }
            }).AddTo(_compositeDisposable);

            _qualityProp.Subscribe(x =>
            {
                _itemQualityText.text = x;
                _itemQualityText.gameObject.SetActive(!string.IsNullOrEmpty(x));
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
