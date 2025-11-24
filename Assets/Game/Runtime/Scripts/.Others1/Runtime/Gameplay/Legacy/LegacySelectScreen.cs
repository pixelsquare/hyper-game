using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class LegacySelectScreen : MonoBehaviour
    {
        [SerializeField] private LegacySelectCard _cardPrefab;
        [SerializeField] private Transform _container;

        private Stack<LegacySelectCard> _cards;
        private Transform _playerTransform;

        private DiContainer _diContainer;

        [Inject]
        private void Construct(DiContainer diContainer)
        {
            _diContainer = diContainer;
        }

        private void OnRollLegacy(IMessage message)
        {
            PromptLegacySelect((LegacyUpgradeSelection[])message.Data);
        }

        private void PromptLegacySelect(LegacyUpgradeSelection[] selection)
        {
            Time.timeScale = 0f;
            _container.gameObject.SetActive(true);

            foreach (var select in selection)
            {
                var card = CreateLegacyCard(select);
                _cards.Push(card);
            }
        }

        private LegacySelectCard CreateLegacyCard(LegacyUpgradeSelection selection)
        {
            var card = _diContainer.InstantiatePrefabForComponent<LegacySelectCard>(_cardPrefab, _container) ?? Instantiate(_cardPrefab, _container);

            var legacy = selection._legacy;
            var cardLabel = selection._isNew
                    ? $"{legacy.LegacyName}"
                    : $"{legacy.LegacyName}\nLevel {legacy.CurrentLevel} to {legacy.CurrentLevel + 1}";

            card.Init(cardLabel, () =>
            {
                OnSelectLegacy();
                Dispatcher.SendMessageData(GameEvents.Gameplay.OnSelectLegacy, selection);
            });

            return card;
        }

        private void OnSelectLegacy()
        {
            _container.gameObject.SetActive(false);

            Time.timeScale = 1f;

            while (_cards.Count > 0)
            {
                Destroy(_cards.Pop().gameObject);
            }
        }

        private void OnEnable()
        {
            Dispatcher.AddListener(GameEvents.Gameplay.OnRollLegacy, OnRollLegacy);
        }

        private void OnDisable()
        {
            Dispatcher.RemoveListener(GameEvents.Gameplay.OnRollLegacy, OnRollLegacy, true);
        }

        private void Start()
        {
            _cards = new Stack<LegacySelectCard>();
        }
    }
}
