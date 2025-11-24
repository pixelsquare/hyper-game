using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    using AppStateEvent = GameEvents.AppState;
    using GameEvent = GameEvents.Gameplay;

    public class GameplayHud : BaseHud
    {
        [SerializeField] private CanvasGroup _legacyIconCanvasGrp;

        [SerializeField] private ExperienceProgress _experienceProgress;
        [SerializeField] private BossHealthProgress _bossHealthProgress;

        [Inject] private IHudManager _hudManager;
        [Inject] private IMissionManager _missionManager;
        [Inject] private IOverlayManager _overlayManager;
        [Inject] private IInventoryManager _inventoryManager;

        private CompositeDisposable _compositeDisposable = new();
        private CancellationTokenSource _cancellationTokenSource;

        private readonly ReactiveProperty<float> _legacyIconAlphaProp = new(1.0f);
        private readonly ReactiveProperty<bool> _legacyIconProp = new(false);
        private readonly ReactiveProperty<bool> _bossHealthActiveProp = new(false);

        private async void HandleLevelUpEvent(IMessage message)
        {
            if (message.Data is not LegacyUpgradeSelection[] legacySelection)
            {
                return;
            }

            _legacyIconProp.Value = false;
            var legacies = legacySelection.Select(x => x._legacy);
            var selectedLegacy = await _hudManager.ShowHudAsync<LegacyPickerHud>(HudType.LegacyPicker)
                                                  .Setup(legacies, 1)
                                                  .Task;

            var legacy = Array.Find(legacySelection, x => x._legacy.LegacyId.Equals(selectedLegacy.LegacyId));
            Dispatcher.SendMessageData(GameEvent.OnSelectLegacy, legacy);

            await UniTask.NextFrame(); // Wait for spawned objects to be added on the dictionary.
            ShowLegacyIcons();
        }

        private async void ShowLegacyIcons()
        {
            _legacyIconProp.Value = true;
            _cancellationTokenSource?.Cancel();

            var isCancelled = false;
            using (_cancellationTokenSource = new CancellationTokenSource())
            {
                isCancelled = await UniTask.Delay(TimeSpan.FromSeconds(3f), DelayType.UnscaledDeltaTime, cancellationToken: _cancellationTokenSource.Token)
                                           .SuppressCancellationThrow();
            }

            _cancellationTokenSource = null;

            if (!isCancelled)
            {
                _legacyIconProp.Value = false;
            }
        }

        private void HandleBossSpawnedEvent(IMessage message)
        {
            if (message.Data is not UnitSpawn bossSpawn)
            {
                return;
            }

            _bossHealthActiveProp.Value = true;
            _bossHealthProgress.Setup(bossSpawn);
        }

        private void HandleGameEndedEvent(IMessage message)
        {
            _bossHealthActiveProp.Value = false;
        }

        private async void HandleGameWinEvent(IMessage message)
        {
            Time.timeScale = 0f;
            var rewards = new List<IItem>();
            rewards.AddRange(_missionManager.ActiveMissionLevel.FirstClearRewards);
            rewards.AddRange(_missionManager.ActiveMissionLevel.OtherRewards);

            await _hudManager.ShowHudAsync<MissionResultHud>(HudType.MissionResult)
                             .Setup(MissionResultHud.ResultType.Win, rewards)
                             .Task;

            _overlayManager.ShowOverlay("Saving results. Please wait ...");
            _inventoryManager.AddItemsToInventory(rewards);
            await _inventoryManager.SyncUpAsync();
            _overlayManager.HideOverlay();

            Dispatcher.SendMessage(AppStateEvent.OnGameEndedEvent);
            Time.timeScale = 1f;
        }

        private async void HandleGameLoseEvent(IMessage message)
        {
            Time.timeScale = 0f;
            await _hudManager.ShowHudAsync<MissionResultHud>(HudType.MissionResult)
                             .Setup(MissionResultHud.ResultType.Lose)
                             .Task;

            Dispatcher.SendMessage(AppStateEvent.OnGameEndedEvent);
            Time.timeScale = 1f;
        }

        private async void HandleGameQuitEvent(IMessage message)
        {
            Time.timeScale = 0f;
            await _hudManager.ShowHudAsync<MissionResultHud>(HudType.MissionResult)
                             .Setup(MissionResultHud.ResultType.Quit)
                             .Task;

            Dispatcher.SendMessage(AppStateEvent.OnGameEndedEvent);
            Time.timeScale = 1f;
        }

        private void OnEnable()
        {
            Dispatcher.AddListener(AppStateEvent.OnGameEndedEvent, HandleGameEndedEvent);
            Dispatcher.AddListener(GameEvent.OnBossSpawn, HandleBossSpawnedEvent);
            Dispatcher.AddListener(GameEvent.OnRollLegacy, HandleLevelUpEvent);

            Dispatcher.AddListener(GameEvent.OnGameWin, HandleGameWinEvent);
            Dispatcher.AddListener(GameEvent.OnGameLose, HandleGameLoseEvent);
            Dispatcher.AddListener(GameEvent.OnGameQuit, HandleGameQuitEvent);

            _legacyIconAlphaProp.Subscribe(x => _legacyIconCanvasGrp.alpha = x)
                                .AddTo(_compositeDisposable);

            _legacyIconProp.Subscribe(x => _legacyIconCanvasGrp.gameObject.SetActive(x))
                           .AddTo(_compositeDisposable);

            _bossHealthActiveProp.Subscribe(x =>
            {
                _bossHealthProgress.gameObject.SetActive(x);
                _experienceProgress.gameObject.SetActive(!x);
            }).AddTo(_compositeDisposable);
        }

        private void OnDisable()
        {
            Dispatcher.RemoveListener(AppStateEvent.OnGameEndedEvent, HandleGameEndedEvent, true);
            Dispatcher.RemoveListener(GameEvent.OnBossSpawn, HandleBossSpawnedEvent, true);
            Dispatcher.RemoveListener(GameEvent.OnRollLegacy, HandleLevelUpEvent, true);

            Dispatcher.RemoveListener(GameEvent.OnGameWin, HandleGameWinEvent, true);
            Dispatcher.RemoveListener(GameEvent.OnGameLose, HandleGameLoseEvent, true);
            Dispatcher.RemoveListener(GameEvent.OnGameQuit, HandleGameQuitEvent, true);

            _compositeDisposable.Clear();
        }

        private void OnDestroy()
        {
            _compositeDisposable.Dispose();
            _compositeDisposable = null;
        }
    }
}
