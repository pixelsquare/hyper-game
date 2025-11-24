using UniRx;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    using UIEvent = GameEvents.UserInterface;

    [RequireComponent(typeof(RinawaButton))]
    public class MissionLevelButton : BaseButton
    {
        [SerializeField] private RinawaImage _levelIconImage;
        [SerializeField] private RinawaText _levelAliasText;

        [SerializeField] private Sprite _activeIconSprite;
        [SerializeField] private Sprite _inactiveIconSprite;

        [SerializeField] private Sprite _activeSprite;
        [SerializeField] private Color _activeColor;

        [SerializeField] private Sprite _inactiveSprite;
        [SerializeField] private Color _inactiveColor;

        [SerializeField] private Color _disabledColor;

        public MissionLevel MissionLevel { get; private set; }

        private readonly ReactiveProperty<bool> _isSelectedProp = new(false);
        private readonly ReactiveProperty<string> _aliasProp = new(null);

        public void Setup(MissionLevel missionLevel, bool interactive)
        {
            MissionLevel = missionLevel;
            _aliasProp.Value = MissionLevel.Alias;
            ButtonInteractableProp.Value = interactive;
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

            _aliasProp.Subscribe(x => _levelAliasText.text = x)
                      .AddTo(_compositeDisposable);

            _isSelectedProp.Subscribe(x =>
            {
                _image.sprite = x ? _activeSprite : _inactiveSprite;
                _levelIconImage.color = x ? _activeColor : _inactiveColor;
                _levelAliasText.color = x ? _activeColor : _inactiveColor;
                _levelIconImage.sprite = x ? _activeIconSprite : _inactiveIconSprite;
            }).AddTo(_compositeDisposable);

            ButtonInteractableProp.Where(x => x == false).Subscribe(_ =>
            {
                _image.color = _disabledColor;
                _levelIconImage.color = _disabledColor;
                _levelAliasText.color = _disabledColor;
            }).AddTo(_compositeDisposable);
        }

        private void BroadcastButtonClickedEvent(float delay = 0f)
        {
            Dispatcher.SendMessage(this, UIEvent.OnActiveLevelSelected, MissionLevel, delay);
        }
    }
}
