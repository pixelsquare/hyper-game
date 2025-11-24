using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    using AppStateEvent = GameEvents.AppState;

    public class ShopState : State
    {
        [Inject] private IHudManager _hudManager;

        public override void OnEnter()
        {
            _hudManager.ShowHud(HudType.Shop);
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
            _hudManager.HideHud(HudType.Shop);
        }
    }
}
