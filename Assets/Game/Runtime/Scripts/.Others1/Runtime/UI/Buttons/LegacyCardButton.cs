using UniRx;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    using UIEvent = GameEvents.UserInterface;

    public class LegacyCardButton : BaseButton
    {
        [SerializeField] private RinawaImage _iconImage;
        [SerializeField] private RinawaImage _outlineImage;
        [SerializeField] private RinawaImage _progressFillImage;
        [SerializeField] private RinawaImage _backgroundImage;
        [SerializeField] private RinawaText _domainText;
        [SerializeField] private RinawaText _nameText;
        [SerializeField] private RinawaText _typeText;
        [SerializeField] private RinawaText _activeText;
        [SerializeField] private RinawaText _descriptionText;
        
        // TODO: Can be optimized by changing the tint instead of replacing the texture.
        [SerializeField] private SerializableDictionary<LegacyLayer, Sprite> _domainSpriteMap;

        [Inject] private ReactiveTextureProperty _iconProp;

        private ILegacy _legacy;

        private readonly ReactiveProperty<int> _progressProp = new(0);
        private readonly ReactiveProperty<bool> _isSelectedProp = new(false);
        private readonly ReactiveProperty<Sprite> _backgroundProp = new(null);
        private readonly ReactiveProperty<string> _domainProp = new(null);
        private readonly ReactiveProperty<string> _nameProp = new(null);
        private readonly ReactiveProperty<string> _typeProp = new(null);
        private readonly ReactiveProperty<string> _activeProp = new(null);
        private readonly ReactiveProperty<string> _descProp = new(null);

        public void Setup(ILegacy legacy)
        {
            var legacyLayer = legacy.LegacyLayer;
            _legacy = legacy;
            _domainProp.Value = legacyLayer.ToStringValue();
            _nameProp.Value = legacy.LegacyName;
            _iconProp.Value = legacy.LegacyIconName;
            _typeProp.Value = legacy.LegacySlot.ToString();
            _activeProp.Value = legacy.LegacySlot == LegacySlot.Passive ? "Passive" : "Active";
            _descProp.Value = legacy.LegacyDescription;
            _progressProp.Value = (int)legacy.CurrentLevel;

            if (_domainSpriteMap.TryGetValue(legacyLayer, out var bgSprite))
            {
                _backgroundProp.Value = bgSprite;
            }
        }

        public void SetButtonSelected(bool isSelected)
        {
            _isSelectedProp.Value = isSelected;
        }

        protected override void OnButtonClicked()
        {
            SetButtonSelected(true);
            BroadcastButtonClickedEvent();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            _progressProp.Subscribe(x => _progressFillImage.fillAmount = x * 0.2f)
                         .AddTo(_compositeDisposable);

            _isSelectedProp.Subscribe(x => _outlineImage.gameObject.SetActive(x))
                           .AddTo(_compositeDisposable);

            _iconProp.Subscribe(x =>
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

            _backgroundProp.Subscribe(x =>
            {
                _backgroundImage.sprite = x;
            }).AddTo(_compositeDisposable);

            _domainProp.Subscribe(x =>
            {
                _domainText.text = x;
                _domainText.gameObject.SetActive(!string.IsNullOrEmpty(x));
            }).AddTo(_compositeDisposable);

            _nameProp.Subscribe(x =>
            {
                _nameText.text = x;
                _nameText.gameObject.SetActive(!string.IsNullOrEmpty(x));
            }).AddTo(_compositeDisposable);

            _typeProp.Subscribe(x =>
            {
                _typeText.text = x;
                _typeText.gameObject.SetActive(!string.IsNullOrEmpty(x));
            }).AddTo(_compositeDisposable);
            
            _activeProp.Subscribe(x =>
            {
                _activeText.text = x;
                _activeText.gameObject.SetActive(!string.IsNullOrEmpty(x));
            }).AddTo(_compositeDisposable);

            _descProp.Subscribe(x =>
            {
                _descriptionText.text = x;
                _descriptionText.gameObject.SetActive(!string.IsNullOrEmpty(x));
            }).AddTo(_compositeDisposable);
        }

        private void BroadcastButtonClickedEvent(float delay = 0f)
        {
            Dispatcher.SendMessage(this, UIEvent.OnActiveLegacySelected, _legacy, delay);
        }
    }
}
