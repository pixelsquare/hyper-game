using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    using AppEvent = GameEvents.AppState;
    using UIEvent = GameEvents.UserInterface;

    public class LegacyPickerHud : BaseHud
    {
        [SerializeField] private Transform _parentTransform;
        [SerializeField] private LegacyCardButton _legacyCardButtonPrefab;

        public new UniTask<ILegacy> Task { get; private set; }
        private LegacyCardPool CardPool => _legacyCardPool ??= new LegacyCardPool(_diContainer, _legacyCardButtonPrefab, _parentTransform);

        [Inject] private DiContainer _diContainer;
        [Inject] private IHudManager _hudManager;

        private ILegacy _selectedLegacy;
        private LegacyCardButton _selectedLegacyButton;

        private LegacyCardPool _legacyCardPool;
        private readonly List<LegacyCardButton> _legacyCardButtons = new();

        public LegacyPickerHud Setup(IEnumerable<ILegacy> legacies, int selectedIdx = 0)
        {
            _selectedLegacy = legacies.ElementAt(selectedIdx);
            PopulateLegacyCards(legacies);
            Task = UniTask.RunOnThreadPool(async () =>
            {
                await UniTask.WaitUntil(() => _isClosed);
                return _selectedLegacy;
            });
            return this;
        }

        private void PopulateLegacyCards(IEnumerable<ILegacy> legacies)
        {
            foreach (var legacy in legacies)
            {
                var legacyCard = CardPool.Rent();
                legacyCard.Setup(legacy);
                _legacyCardButtons.Add(legacyCard);

                if (legacy.LegacyId.Equals(_selectedLegacy.LegacyId))
                {
                    _selectedLegacyButton = legacyCard;
                }
            }

            _selectedLegacyButton.SetButtonSelected(true);
        }

        private void HandleActiveLegacySelected(IMessage message)
        {
            if (message.Data is not ILegacy legacy || message.Sender is not LegacyCardButton legacyCardButton)
            {
                return;
            }

            _selectedLegacy = legacy;
            _selectedLegacyButton.SetButtonSelected(false);
            legacyCardButton.SetButtonSelected(true);
            _selectedLegacyButton = legacyCardButton;
        }

        private void HandleMainMenuScreenEvent(IMessage message)
        {
            _hudManager.HideHud(HudType.LegacyPicker);
        }

        private void OnEnable()
        {
            Time.timeScale = 0f;
            Dispatcher.AddListener(UIEvent.OnActiveLegacySelected, HandleActiveLegacySelected);
            Dispatcher.AddListener(AppEvent.ToMainMenuScreenEvent, HandleMainMenuScreenEvent);
        }

        private void OnDisable()
        {
            Time.timeScale = 1f;
            Dispatcher.RemoveListener(UIEvent.OnActiveLegacySelected, HandleActiveLegacySelected, true);
            Dispatcher.RemoveListener(AppEvent.ToMainMenuScreenEvent, HandleMainMenuScreenEvent, false);
        }

        private class LegacyCardPool : RinawaObjectPool<LegacyCardButton>
        {
            public LegacyCardPool(DiContainer diContainer, LegacyCardButton prefab, Transform parent)
                    : base(diContainer, prefab, parent)
            {
            }
        }
    }
}
