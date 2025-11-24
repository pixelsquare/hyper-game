using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv.Tests
{
    using AppStateEvent = GameEvents.AppState;

    public class GameplayTester : MonoBehaviour
    {
        [SerializeField] private GameObject _worldContext;
        [SerializeField] private GameObject _gameplaySystems;
        [SerializeField] private GameObject[] _postActiveObjects;

        [TitleGroup("Overrides")]
        [SerializeField] private MissionConfig _missionConfigOverride;
        [SerializeField] private MissionLevelConfig _missionLevelConfigOverride;
        [SerializeField] private HeroConfig _heroConfigOverride;

        [Inject] private DiContainer _diContainer;
        [Inject] private IAssetLoaderManager _assetLoaderManager;
        [Inject] private LazyInject<IHudManager> _hudManager;
        [Inject] private IMissionManager _missionManager;
        [Inject] private IHeroLoadoutManager _heroLoadoutManager;
        [Inject] private LazyInject<IOverlayManager> _overlayManager;

        private void SetConfigOverrides()
        {
            var mission = _missionConfigOverride?.Config ?? _missionManager.ActiveMission;
            _missionManager.SetActiveMission(mission);

            var missionLevel = _missionLevelConfigOverride?.Config ?? _missionManager.ActiveMissionLevel;
            _missionManager.SetActiveMissionLevel(missionLevel);

            var hero = _heroConfigOverride?.Config ?? _heroLoadoutManager.ActiveHero;
            _heroLoadoutManager.SetActiveHero(hero);
        }

        private void HandleGameEndedEvent(IMessage message)
        {
            GameUtil.ExitApplication();
        }

        private async UniTask InitializeAssetLoaderAsync()
        {
            await UniTask.NextFrame();
            _overlayManager.Value.ShowOverlay("Loading assets ...");
            _assetLoaderManager.InitializeAsync();
            await UniTask.WaitUntil(() => _assetLoaderManager.IsInitialized);
            _overlayManager.Value.HideOverlay();
        }

        private async UniTask InstantiateSystemsAsync()
        {
            await UniTask.NextFrame();
            _ = _diContainer.InstantiatePrefab(_worldContext) ?? Instantiate(_worldContext);
            await UniTask.NextFrame();
        }

        private async UniTask ShowGameHudAsync()
        {
            await UniTask.WaitUntil(() => _hudManager.Value.IsInitialized);
            _hudManager.Value.ShowHud(HudType.Game);
        }

        private void Awake()
        {
            foreach (var obj in _postActiveObjects)
            {
                obj.SetActive(false);
            }
        }

        private void OnEnable()
        {
            Dispatcher.AddListener(AppStateEvent.OnGameEndedEvent, HandleGameEndedEvent);
        }

        private void OnDisable()
        {
            Dispatcher.RemoveListener(AppStateEvent.OnGameEndedEvent, HandleGameEndedEvent, true);
        }

        private async void Start()
        {
            SetConfigOverrides();

            await InitializeAssetLoaderAsync();
            await InstantiateSystemsAsync();

            _overlayManager.Value.ShowOverlay("Initializing ...");

            await UniTask.NextFrame();

            foreach (var obj in _postActiveObjects)
            {
                obj.SetActive(true);
            }

            await GameplayUtility.InitializeGameplaySystemsAsync(_gameplaySystems);
            await ShowGameHudAsync();

            await UniTask.NextFrame();
            Dispatcher.SendMessage(AppStateEvent.OnGameStartEvent);
            _overlayManager.Value.HideOverlay();
        }
    }
}
