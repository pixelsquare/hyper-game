using Zenject;

namespace Santelmo.Rinsurv
{
    using AppStateEvent = GameEvents.AppState;

    public class HeroLoadoutState : State
    {
        [Inject] private IHudManager _hudManager;

        public override void OnEnter()
        {
            _hudManager.ShowHud(HudType.HeroLoadout);
            Dispatcher.AddListener(AppStateEvent.ToMainMenuScreenEvent, HandleMainMenuScreenEvent);
            Dispatcher.AddListener(AppStateEvent.ToInventoryScreenEvent, HandleInventoryScreenEvent);
        }

        public override void OnExit()
        {
            _hudManager.HideHud(HudType.HeroLoadout);
            Dispatcher.RemoveListener(AppStateEvent.ToMainMenuScreenEvent, HandleMainMenuScreenEvent, true);
            Dispatcher.RemoveListener(AppStateEvent.ToInventoryScreenEvent, HandleInventoryScreenEvent, true);
        }

        private void HandleMainMenuScreenEvent(IMessage message)
        {
            EndState(AppStateEvent.ToMainMenuScreenEvent);
        }

        private void HandleInventoryScreenEvent(IMessage message)
        {
            EndState(AppStateEvent.ToInventoryScreenEvent);
        }
    }
}
