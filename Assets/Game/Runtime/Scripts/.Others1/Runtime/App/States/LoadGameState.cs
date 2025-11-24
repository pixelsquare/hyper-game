using Zenject;

namespace Santelmo.Rinsurv
{
    using AppStateEvent = GameEvents.AppState;

    public class LoadGameState : State
    {
        [Inject] private IHudManager _hudManager;

        public override async void OnEnter()
        {
            Dispatcher.AddListener(AppStateEvent.OnLoadGameEndedEvent, OnLoadGameEnd, true);

            await _hudManager.ShowHudAsync<LoadGameHud>(HudType.LoadGame)
                             .Setup()
                             .Task;

            Dispatcher.SendMessage(AppStateEvent.OnLoadGameplayObjectsEvent);
        }

        public override void OnExit()
        {
            _hudManager.HideHud(HudType.LoadGame);
            Dispatcher.RemoveListener(AppStateEvent.OnLoadGameEndedEvent, OnLoadGameEnd, true);
        }

        private void OnLoadGameEnd(IMessage message)
        {
            EndState();
        }
    }
}
