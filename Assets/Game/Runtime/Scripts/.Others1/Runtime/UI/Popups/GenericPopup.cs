using System;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class GenericPopup : BasePopup
    {
        public enum ModalType
        {
            Default,
            Info,
            Warning,
            Error,
            Success
        }

        [Flags]
        public enum ConfirmationOption
        {
            None = 0,
            Close = 1 << 1,
            Cancel = 1 << 2,
            Confirm = 1 << 3
        }

        [SerializeField] private RinawaImage _iconImage;
        [SerializeField] private RinawaText _messageText;

        [Header("Generic Button")]
        [SerializeField] private Transform _actionPanelTransform;
        [SerializeField] private GenericButton _genericButtonPrefab;

        [Header("Icons")]
        [SerializeField] private Sprite _infoSprite;
        [SerializeField] private Sprite _warningSprite;
        [SerializeField] private Sprite _successSprite;
        [SerializeField] private Sprite _errorSprite;

        public new UniTask<ConfirmationOption> Task { get; private set; }

        [Inject] private DiContainer _diContainer;
        [Inject] private IPopupManager _popupManager;

        private ConfirmationOption _confirmationOption;
        private CompositeDisposable _compositeDisposable = new();

        private readonly ReactiveProperty<Sprite> _iconProp = new(null);
        private readonly ReactiveProperty<string> _messageProp = new(null);

        public GenericPopup Setup(ModalType modalType, string message, ConfirmationOption buttons)
        {
            var iconSprite = GetPopupIconSprite(modalType);
            _iconProp.Value = iconSprite;
            _messageProp.Value = message;
            PopulateActionButtons(buttons);

            Task = UniTask.Create(async () =>
            {
                await UniTask.WaitUntil(() => _confirmationOption != ConfirmationOption.None);
                _popupManager.ClosePopup(PopupType.Generic);
                return _confirmationOption;
            });

            return this;
        }

        private void PopulateActionButtons(ConfirmationOption buttons)
        {
            var confirmOptions = (ConfirmationOption[])Enum.GetValues(typeof(ConfirmationOption));

            foreach (var confirmOption in confirmOptions)
            {
                if ((buttons & confirmOption) != confirmOption || confirmOption == ConfirmationOption.None)
                {
                    continue;
                }

                var iconSprite = GetButtonIconSprite(confirmOption);
                var genericButton = _diContainer.InstantiatePrefabForComponent<GenericButton>(_genericButtonPrefab, _actionPanelTransform);
                genericButton.Setup(iconSprite, confirmOption.ToString(), () => { _confirmationOption = confirmOption; });
            }
        }

        private Sprite GetPopupIconSprite(ModalType modalType)
        {
            return modalType switch
            {
                ModalType.Info    => _infoSprite,
                ModalType.Warning => _warningSprite,
                ModalType.Error   => _errorSprite,
                ModalType.Success => _successSprite,
                _                 => null
            };
        }

        private Sprite GetButtonIconSprite(ConfirmationOption option)
        {
            return option switch
            {
                ConfirmationOption.Confirm => _successSprite,
                ConfirmationOption.Close   => _errorSprite,
                ConfirmationOption.Cancel  => _errorSprite,
                _                          => null
            };
        }

        private void OnEnable()
        {
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
