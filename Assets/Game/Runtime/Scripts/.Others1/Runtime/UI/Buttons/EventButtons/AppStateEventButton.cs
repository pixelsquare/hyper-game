using UnityEngine;

namespace Santelmo.Rinsurv
{
    using AppStateEvent = GameEvents.AppState;

    [RequireComponent(typeof(RinawaButton))]
    public class AppStateEventButton : BaseButton
    {
        [SerializeField] private float _delay;

        [ConstantDropdown(typeof(AppStateEvent))]
        [SerializeField] private string _eventName;

        protected override void OnButtonClicked()
        {
            Dispatcher.SendMessage(_eventName, _delay);
        }
    }
}
