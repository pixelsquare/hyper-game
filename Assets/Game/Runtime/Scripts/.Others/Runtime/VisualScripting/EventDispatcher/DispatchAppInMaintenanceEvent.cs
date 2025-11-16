using Kumu.Kulitan.Common;
using Unity.VisualScripting;

namespace Kumu.Kulitan.VisualScripting
{
    [UnitCategory("Kumu/EventDispatch")]
    public class DispatchAppInMaintenanceEvent : Unit
    {
        [DoNotSerialize]
        public ControlInput inputTrigger;

        [DoNotSerialize]
        public ControlOutput outputTrigger;

        protected override void Definition()
        {
            inputTrigger = ControlInput(nameof(inputTrigger), _ =>
            {
                GlobalNotifier.Instance.Trigger(new AppInMaintenanceEvent());
                return outputTrigger;
            });
            outputTrigger = ControlOutput(nameof(outputTrigger));
        }
    }
}
