using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    using UIEvent = GameEvents.UserInterface;

    [RequireComponent(typeof(RinawaButton))]
    public class SetActiveHeroButton : BaseButton
    {
        [Inject] private IHeroLoadoutManager _heroLoadoutManager;

        private Hero _selectedHero;

        protected override void OnButtonClicked()
        {
            _heroLoadoutManager.SetActiveHero(_selectedHero);
            Dispatcher.SendMessageData(UIEvent.OnActiveHeroSelected, _selectedHero);
        }

        private void HandleActiveHeroSelected(IMessage message)
        {
            if (message.Data is not Hero hero)
            {
                return;
            }

            _selectedHero = hero;
        }

        protected override void Awake()
        {
            base.Awake();
            Dispatcher.AddListener(UIEvent.OnActiveHeroSelected, HandleActiveHeroSelected, true);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Dispatcher.RemoveListener(UIEvent.OnActiveHeroSelected, HandleActiveHeroSelected, true);
        }
    }
}
