using Kumu.Kulitan.Avatar;
using Kumu.Kulitan.Common;
using Unity.VisualScripting;

namespace Kumu.Kulitan.VisualScripting
{
    public class UnitSendExitShopEvent : Unit
    {
        [DoNotSerialize] private ControlInput inputTrigger;
        [DoNotSerialize] private ControlOutput outputTrigger;

        [UnitHeaderInspectable][DoNotSerialize] private ValueInput exitMode;
        
        protected override void Definition()
        {
            exitMode = ValueInput(nameof(exitMode), ExitShopEvent.Mode.Immediate);

            inputTrigger = ControlInput(nameof(inputTrigger), CheckState);
            outputTrigger = ControlOutput(nameof(outputTrigger));
            
            Succession(inputTrigger, outputTrigger);
        }

        private ControlOutput CheckState(Flow flow)
        {
            var mode = flow.GetValue<ExitShopEvent.Mode>(exitMode);
            GlobalNotifier.Instance.Trigger(new ExitShopEvent(mode));

            return outputTrigger;
        }
    }
}
