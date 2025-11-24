using Zenject;

namespace Santelmo.Rinsurv
{
    using AppStateEvent = GameEvents.AppState;

    public class MissionSelectState : State
    {
        [Inject] private IHudManager _hudManager;

        public override void OnEnter()
        {
            _hudManager.ShowHud(HudType.MissionsSelect);
            Dispatcher.AddListener(AppStateEvent.OnLoadGameStartEvent, HandleLoadGameStartEvent);
            Dispatcher.AddListener(AppStateEvent.ToMainMenuScreenEvent, HandleMainMenuScreenEvent);
        }

        public override void OnExit()
        {
            _hudManager.HideHud(HudType.MissionsSelect);
            Dispatcher.RemoveListener(AppStateEvent.OnLoadGameStartEvent, HandleLoadGameStartEvent, true);
            Dispatcher.RemoveListener(AppStateEvent.ToMainMenuScreenEvent, HandleMainMenuScreenEvent, true);
        }

        private void HandleLoadGameStartEvent(IMessage message)
        {
            EndState();
        }

        private void HandleMainMenuScreenEvent(IMessage message)
        {
            EndState(AppStateEvent.ToMainMenuScreenEvent);
        }
    }
}
