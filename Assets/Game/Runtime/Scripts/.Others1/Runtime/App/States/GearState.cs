using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    using AppStateEvent = GameEvents.AppState;

    public class GearState : State
    {
        [Inject] private IHudManager _hudManager;

        public override void OnEnter()
        {
            _hudManager.ShowHud(HudType.Gear);
        }

        public override void OnUpdate()
        {
            if (Input.GetMouseButtonDown(0))
            {
                EndState(AppStateEvent.ToMainMenuScreenEvent);
            }
        }

        public override void OnExit()
        {
            _hudManager.HideHud(HudType.Gear);
        }
    }
}
