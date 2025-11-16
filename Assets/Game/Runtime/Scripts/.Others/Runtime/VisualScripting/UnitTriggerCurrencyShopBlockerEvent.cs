using Kumu.Kulitan.Common;
using Kumu.Kulitan.UI;
using Unity.VisualScripting;

namespace Kumu.Kulitan.VisualScripting
{
    public class UnitTriggerCurrencyShopBlockerEvent : Unit
    {
        [DoNotSerialize] private ControlInput enter;
        [DoNotSerialize] private ControlOutput exit;
        [DoNotSerialize] private ValueInput isBlocked;

        protected override void Definition()
        {
            enter = ControlInput(nameof(enter), TriggerEvent);
            exit = ControlOutput(nameof(exit));
            isBlocked = ValueInput<bool>(nameof(isBlocked));
            Succession(enter, exit);
        }

        private ControlOutput TriggerEvent(Flow flow)
        {
            var blockedValue = flow.GetValue<bool>(isBlocked);
            GlobalNotifier.Instance.Trigger(new CurrencyShopBlockerEvent(blockedValue));
            return exit;
        }
    }
}
