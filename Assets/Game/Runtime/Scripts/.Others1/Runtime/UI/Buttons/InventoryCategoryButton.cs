using UniRx;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    using UIEvent = GameEvents.UserInterface;

    [RequireComponent(typeof(RinawaButton))]
    public class InventoryCategoryButton : BaseButton
    {
        [SerializeField] private GameObject _outline;
        [SerializeField] private GameObject _categryLabelObj;
        [SerializeField] private Sprite _activeIconSprite;
        [SerializeField] private Sprite _inactiveIconSprite;
        [SerializeField] private InventoryCategoryType _inventoryCategory;

        [Inject] private InventoryHud _inventoryHud;

        private readonly ReactiveProperty<bool> _isSelectedProp = new(false);

        public void SetButtonSelected(bool isSelected)
        {
            _isSelectedProp.Value = isSelected;
        }

        protected override void OnButtonClicked()
        {
            SetButtonSelected(true);
            BroadcastButtonClickedEvent();
        }

        private void BroadcastButtonClickedEvent(float delay = 0f)
        {
            Dispatcher.SendMessage(this, UIEvent.OnInventoryFilterSelected, _inventoryCategory, delay);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            _isSelectedProp.Subscribe(x =>
            {
                _outline.SetActive(x);
                _categryLabelObj.SetActive(x);
                _image.sprite = x ? _activeIconSprite : _inactiveIconSprite;
            }).AddTo(_compositeDisposable);
        }

        protected override void Start()
        {
            base.Start();

            if (_inventoryHud.InitialCategoryType != _inventoryCategory)
            {
                return;
            }

            SetButtonSelected(true);
            BroadcastButtonClickedEvent();
        }
    }
}
