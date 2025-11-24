using UniRx;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    using UIEvent = GameEvents.UserInterface;

    [RequireComponent(typeof(RinawaButton))]
    public class HeroLoadoutButton : BaseButton
    {
        [SerializeField] private RinawaImage _avatarImage;
        [SerializeField] private GameObject _buttonOutline;

        [Inject] private ReactiveTextureProperty _avatarProp;
        [Inject] private IHeroLoadoutManager _heroLoadoutManager;

        private Hero _hero;

        private readonly ReactiveProperty<bool> _outlineActiveProp = new(false);

        public void Setup(Hero hero)
        {
            _hero = hero;
            _avatarProp.Value = hero.IconSpriteName;
        }

        public void SetButtonSelected(bool isSelected)
        {
            _outlineActiveProp.Value = isSelected;
        }

        protected override void OnButtonClicked()
        {
            SetButtonSelected(true);
            BroadcastButtonClickedEvent();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            _avatarProp.Subscribe(x =>
            {
                _avatarImage.sprite = x;
                _avatarImage.gameObject.SetActive(x != null);
            }, () =>
            {
                ButtonLoaderActiveProp.Value = true;
                ButtonInteractableProp.Value = false;
            }, () =>
            {
                ButtonLoaderActiveProp.Value = false;
                ButtonInteractableProp.Value = true;
            }).AddTo(_compositeDisposable);

            _outlineActiveProp.Subscribe(x => _buttonOutline.SetActive(x))
                              .AddTo(_compositeDisposable);
        }

        private void BroadcastButtonClickedEvent(float delay = 0f)
        {
            Dispatcher.SendMessage(this, UIEvent.OnActiveHeroSelected, _hero, delay);
        }

        protected override void Start()
        {
            base.Start();

            var activeHero = _heroLoadoutManager.ActiveHero;

            if (!activeHero.Id.Equals(_hero.Id))
            {
                return;
            }

            SetButtonSelected(true);
            BroadcastButtonClickedEvent();
        }
    }
}
