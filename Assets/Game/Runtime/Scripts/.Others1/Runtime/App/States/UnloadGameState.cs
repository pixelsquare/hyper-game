using Zenject;

namespace Santelmo.Rinsurv
{
    using AppStateEvent = GameEvents.AppState;
    
    public class UnloadGameState : State
    {
        [Inject] private IOverlayManager _overlayManager;
        [Inject] private IHudManager _hudManager;
        
        public override async void OnEnter()
        {            
            Dispatcher.SendMessage(AppStateEvent.OnUnloadGameStartEvent);

            _overlayManager.ShowOverlay("Returning to main menu ...");
            await _hudManager.ShowHudAsync<UnloadGameHud>(HudType.UnloadGame)
                             .Setup()
                             .Task;

            EndState();
        }

        public override void OnExit()
        {
            _overlayManager.HideOverlay();
            _hudManager.HideHud(HudType.UnloadGame);
            Dispatcher.SendMessage(AppStateEvent.OnUnloadGameEndedEvent);
        }
    }
}
