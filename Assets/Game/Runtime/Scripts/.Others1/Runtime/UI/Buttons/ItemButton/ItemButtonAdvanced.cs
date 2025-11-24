using UniRx;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    public class ItemButtonAdvanced : MonoBehaviour
    {
        [SerializeField] private RinawaImage _iconImage;
        [SerializeField] private RinawaText _rarityText;
        [SerializeField] private RinawaText _valueText;

        private CompositeDisposable _compositeDisposable = new();

        private readonly ReactiveProperty<Sprite> _iconProp = new(null);
        private readonly ReactiveProperty<EquipmentRarity> _rarityProp = new(EquipmentRarity.None);
        private readonly ReactiveProperty<int> _valueProp = new(0);

        public void Setup(IItem item)
        {
            if (item is not Emblem emblem)
            {
                return;
            }

            var equipmentProps = emblem.EquipmentProperties;
            _iconProp.Value = emblem.IconTypeSprite;
            _rarityProp.Value = emblem.Rarity;
            _valueProp.Value = equipmentProps.Level;
        }

        private void OnEnable()
        {
            _iconProp.Subscribe(x =>
            {
                _iconImage.sprite = x;
                _iconImage.gameObject.SetActive(x != null);
            }).AddTo(_compositeDisposable);

            _rarityProp.Subscribe(x =>
            {
                var rarity = x.ToString();
                _rarityText.text = rarity;
                _rarityText.gameObject.SetActive(!string.IsNullOrEmpty(rarity));
            }).AddTo(_compositeDisposable);

            _valueProp.Subscribe(x =>
            {
                var value = x.ToString();
                _valueText.text = value;
                _valueText.gameObject.SetActive(!string.IsNullOrEmpty(value));
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
    }
}
