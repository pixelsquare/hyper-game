using Zenject;

namespace Santelmo.Rinsurv
{
    using AppStateEvent = GameEvents.AppState;

    public class InventoryState : State
    {
        [Inject] private IHudManager _hudManager;

        public override void OnEnter()
        {
            _hudManager.ShowHud(HudType.Inventory);
            Dispatcher.AddListener(AppStateEvent.ToMainMenuScreenEvent, HandleMainMenuScreenEvent);
        }

        public override void OnExit()
        {
            _hudManager.HideHud(HudType.Inventory);
            Dispatcher.RemoveListener(AppStateEvent.ToMainMenuScreenEvent, HandleMainMenuScreenEvent, true);
        }

        private void HandleMainMenuScreenEvent(IMessage message)
        {
            EndState(AppStateEvent.ToMainMenuScreenEvent);
        }
    }
}
