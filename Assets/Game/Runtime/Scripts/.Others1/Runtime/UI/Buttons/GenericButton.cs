using UniRx;
using UnityEngine;
using UnityEngine.Events;

namespace Santelmo.Rinsurv
{
    [RequireComponent(typeof(RinawaButton))]
    public class GenericButton : BaseButton
    {
        [SerializeField] private RinawaImage _iconImage;
        [SerializeField] private RinawaText _messageText;

        private UnityAction _onButtonClicked;

        private readonly ReactiveProperty<Sprite> _iconProp = new(null);
        private readonly ReactiveProperty<string> _messageProp = new(null);

        public RinawaButton Setup(Sprite icon, string message, UnityAction onButtonClicked)
        {
            _iconProp.Value = icon;
            _messageProp.Value = message;
            _onButtonClicked = onButtonClicked;
            return _button;
        }

        protected override void OnButtonClicked()
        {
            _onButtonClicked?.Invoke();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            _iconProp.Subscribe(x =>
            {
                _iconImage.sprite = x;
                _iconImage.gameObject.SetActive(x != null);
            }).AddTo(_compositeDisposable);

            _messageProp.Subscribe(x =>
            {
                _messageText.text = x;
                _messageText.gameObject.SetActive(!string.IsNullOrEmpty(x));
            }).AddTo(_compositeDisposable);
        }
    }
}
