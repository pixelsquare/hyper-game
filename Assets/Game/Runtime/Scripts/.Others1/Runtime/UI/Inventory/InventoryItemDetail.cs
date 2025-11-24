using UniRx;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    public class InventoryItemDetail : MonoBehaviour
    {
        [SerializeField] private GameObject _emptyOverlay;
        [SerializeField] private GameObject _itemUpgradePanel;
        [SerializeField] private GameObject _sourcesPanel;
        [SerializeField] private RinawaText _itemDescriptionText;
        [SerializeField] private RinawaText _itemFlavorText;

        [SerializeField] private ItemDetailBasic _itemDetailBasic;
        [SerializeField] private ItemDetailAdvanced _itemDetailAdvanced;

        private CompositeDisposable _compositeDisposable = new();

        private readonly ReactiveProperty<int> _panelActiveIndexProp = new(0);
        private readonly ReactiveProperty<string> _itemDescriptionProp = new(null);
        private readonly ReactiveProperty<string> _itemFlavorProp = new(null);

        public void Setup(IItem item)
        {
            _panelActiveIndexProp.Value = 0;
            _itemDescriptionProp.Value = null;
            _itemFlavorProp.Value = null;

            switch (item)
            {
                case null: return;
                case Emblem:
                    _panelActiveIndexProp.Value = 2;
                    _itemDetailAdvanced.Setup(item);
                    break;
                default:
                    _panelActiveIndexProp.Value = 1;
                    _itemDetailBasic.Setup(item);
                    break;
            }

            _itemDescriptionProp.Value = item.Description;
            _itemFlavorProp.Value = item.FlavorText;
        }

        private void OnEnable()
        {
            _panelActiveIndexProp.Subscribe(x =>
            {
                _emptyOverlay.SetActive(x == 0);
                _itemDetailBasic.gameObject.SetActive(x == 1);
                _itemDetailAdvanced.gameObject.SetActive(x == 2);
                _itemUpgradePanel.SetActive(x == 2);
                _sourcesPanel.SetActive(x is 1 or 2);
            }).AddTo(_compositeDisposable);

            _itemDescriptionProp.Subscribe(x =>
            {
                _itemDescriptionText.text = x;
                _itemDescriptionText.gameObject.SetActive(!string.IsNullOrEmpty(x));
            }).AddTo(_compositeDisposable);

            _itemFlavorProp.Subscribe(x =>
            {
                _itemFlavorText.text = x;
                _itemFlavorText.gameObject.SetActive(!string.IsNullOrEmpty(x));
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
