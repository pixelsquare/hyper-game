using UnityEngine;

namespace Santelmo.Rinsurv
{
    using UIEvent = GameEvents.UserInterface;

    [RequireComponent(typeof(RinawaButton))]
    public class UIEventButton : BaseButton
    {
        [SerializeField] private float _delay;

        [ConstantDropdown(typeof(UIEvent))]
        [SerializeField] private string _eventName;

        protected override void OnButtonClicked()
        {
            Dispatcher.SendMessage(_eventName, _delay);
        }
    }
}
