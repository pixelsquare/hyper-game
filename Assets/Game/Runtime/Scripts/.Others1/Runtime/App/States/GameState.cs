using Zenject;

namespace Santelmo.Rinsurv
{
    using AppStateEvent = GameEvents.AppState;

    public class GameState : State
    {
        [Inject] private IHudManager _hudManager;
        [Inject] private IAudioManager _audioManager;
        [Inject] private ISceneManager _sceneManager;

        public override void OnEnter()
        {
            _sceneManager.SetSceneActive(GameConstants.SceneNames.WorldScene);
            _hudManager.ShowHud(HudType.Game);
            _audioManager.PlaySound(Bgm.Gameplay);

            Dispatcher.AddListener(AppStateEvent.OnGameEndedEvent, OnGameEndedEvent);
            Dispatcher.SendMessage(AppStateEvent.OnGameStartEvent);
            Dispatcher.AddListener(AppStateEvent.ToMainMenuScreenEvent, HandleMainMenuScreenEvent);
        }

        public override void OnExit()
        {
            _sceneManager.SetSceneActive(GameConstants.SceneNames.UiScene);
            _hudManager.HideHud(HudType.Game);
            _audioManager.StopSound(Bgm.Gameplay);

            Dispatcher.RemoveListener(AppStateEvent.OnGameEndedEvent, OnGameEndedEvent, true);
            Dispatcher.RemoveListener(AppStateEvent.ToMainMenuScreenEvent, HandleMainMenuScreenEvent, true);
        }

        private void OnGameEndedEvent(IMessage message)
        {
            _hudManager.HideHud(HudType.MissionResult);
            EndState();
        }

        private void HandleMainMenuScreenEvent(IMessage message)
        {
            EndState(AppStateEvent.ToMainMenuScreenEvent, true);
        }
    }
}
