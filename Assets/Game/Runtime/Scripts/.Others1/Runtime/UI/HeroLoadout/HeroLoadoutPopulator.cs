using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    using UIEvent = GameEvents.UserInterface;

    public class HeroLoadoutPopulator : MonoBehaviour
    {
        [SerializeField] private Transform _heroButtonParent;
        [SerializeField] private HeroLoadoutButton _heroButtonPrefab;

        [Inject] private DiContainer _diContainer;
        [Inject] private IHeroLoadoutManager _heroLoadoutManager;

        private Hero _selectedHero;
        private HeroLoadoutButton _selectedHeroButton;
        private List<HeroLoadoutButton> _heroLoadoutButtons;

        private void PopulateHeroButtons(IEnumerable<Hero> heroes)
        {
            _heroLoadoutButtons = new List<HeroLoadoutButton>(heroes.Count());
            _selectedHero = _heroLoadoutManager.ActiveHero ?? heroes.First();

            foreach (var hero in heroes)
            {
                var heroButton = _diContainer.InstantiatePrefabForComponent<HeroLoadoutButton>(_heroButtonPrefab, _heroButtonParent);
                heroButton.Setup(hero);
                _heroLoadoutButtons.Add(heroButton);

                if (hero.Id.Equals(_selectedHero.Id))
                {
                    _selectedHeroButton = heroButton;
                }
            }

            _heroLoadoutManager.SetActiveHero(_selectedHero);
            Dispatcher.SendMessageData(UIEvent.OnActiveHeroSelected, _selectedHero);
        }

        private void HandleActiveHeroSelected(IMessage message)
        {
            if (message.Data is not Hero hero 
             || message.Sender is not HeroLoadoutButton heroButton)
            {
                return;
            }

            _selectedHero = hero;
            _selectedHeroButton.SetButtonSelected(false);
            _selectedHeroButton = heroButton;
        }

        private void OnEnable()
        {
            Dispatcher.AddListener(UIEvent.OnActiveHeroSelected, HandleActiveHeroSelected);
        }

        private void OnDisable()
        {
            Dispatcher.RemoveListener(UIEvent.OnActiveHeroSelected, HandleActiveHeroSelected, true);
        }

        private void Start()
        {
            PopulateHeroButtons(_heroLoadoutManager.Heroes);
        }
    }
}
