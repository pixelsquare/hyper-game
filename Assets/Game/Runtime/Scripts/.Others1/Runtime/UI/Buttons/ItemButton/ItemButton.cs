using System;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Santelmo.Rinsurv
{
    [RequireComponent(typeof(RinawaButton))]
    public class ItemButton : BaseButton
    {
        [SerializeField] private RinawaImage _iconImage;
        [SerializeField] private RinawaImage _frameImage;
        [SerializeField] private GameObject _highlightImage;

        [SerializeField] private ItemButtonBasic _itemButtonBasic;
        [SerializeField] private ItemButtonAdvanced _itemButtonAdvanced;

        [SerializeField] private SerializableDictionary<EquipmentRarity, Sprite> _frameMap;

        public IItem Item { get; private set; }

        [Inject] private ReactiveTextureProperty _avatarProp;

        private Func<UniTaskVoid> _onButtonClicked;

        private readonly ReactiveProperty<int> _panelActiveIndexProp = new(0);
        private readonly ReactiveProperty<bool> _panelHighlightProp = new(false);
        private readonly ReactiveProperty<EquipmentRarity> _itemRarityProp = new(EquipmentRarity.None);

        public RinawaButton Setup(IItem item, UnityAction onButtonClicked = null)
        {
            Setup(item, async () =>
            {
                onButtonClicked?.Invoke();
                await UniTask.CompletedTask;
            });
            return _button;
        }

        public RinawaButton Setup(IItem item, int count, UnityAction onButtonClicked = null)
        {
            Setup(item, count, async () =>
            {
                onButtonClicked?.Invoke();
                await UniTask.CompletedTask;
            });
            return _button;
        }

        public RinawaButton Setup(IItem item, Func<UniTaskVoid> onButtonClickedAsync = null)
        {
            Setup(item, 0, onButtonClickedAsync);
            return _button;
        }

        public RinawaButton Setup(IItem item, int count, Func<UniTaskVoid> onButtonClickedAsync = null)
        {
            Item = item;

            _panelActiveIndexProp.Value = 0;
            _avatarProp.Value = null;
            _itemRarityProp.Value = EquipmentRarity.None;
            _onButtonClicked = onButtonClickedAsync;

            switch (item)
            {
                case null: return _button;
                case Emblem emblem:
                    _panelActiveIndexProp.Value = 2;
                    _itemButtonAdvanced.Setup(item);
                    _itemRarityProp.Value = emblem.Rarity;
                    break;
                default:
                    _panelActiveIndexProp.Value = 1;
                    _itemButtonBasic.Setup(count);
                    break;
            }

            _avatarProp.Value = item.IconSpriteName;
            return _button;
        }

        public void SetButtonSelected(bool isSelected)
        {
            _panelHighlightProp.Value = isSelected;
        }

        protected override void OnButtonClicked()
        {
            _onButtonClicked?.Invoke();
            SetButtonSelected(true);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            _panelActiveIndexProp.Subscribe(x =>
            {
                _itemButtonBasic.gameObject.SetActive(x == 1);
                _itemButtonAdvanced.gameObject.SetActive(x == 2);
            }).AddTo(_compositeDisposable);

            _panelHighlightProp.Subscribe(x => _highlightImage.gameObject.SetActive(x))
                               .AddTo(_compositeDisposable);

            _avatarProp.Subscribe(x =>
            {
                _iconImage.sprite = x;
                _iconImage.gameObject.SetActive(x != null);
            }, () =>
            {
                ButtonLoaderActiveProp.Value = true;
                ButtonInteractableProp.Value = false;
            }, () =>
            {
                ButtonLoaderActiveProp.Value = false;
                ButtonInteractableProp.Value = true;
            }).AddTo(_compositeDisposable);

            _itemRarityProp.Subscribe(x =>
            {
                if (_frameMap.TryGetValue(x, out var frameSprite))
                {
                    _frameImage.sprite = frameSprite;
                }
            }).AddTo(_compositeDisposable);
        }
    }
}
