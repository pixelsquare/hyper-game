using Kumu.Kulitan.Common;
using Unity.VisualScripting;

namespace Kumu.Kulitan.VisualScripting
{
    [UnitCategory("Kumu/EventDispatch")]
    public class DispatchEvent : Unit
    {
        [DoNotSerialize]
        public ControlInput inputTrigger;

        [DoNotSerialize]
        public ControlOutput outputTrigger;

        [DoNotSerialize] 
        public ValueInput eventName;

        protected override void Definition()
        {
            inputTrigger = ControlInput(nameof(inputTrigger), f =>
            {
                var key = f.GetValue<string>(eventName);
                GlobalNotifier.Instance.Trigger(key);
                return outputTrigger;
            });
            outputTrigger = ControlOutput(nameof(outputTrigger));

            eventName = ValueInput(nameof(eventName), "event_name");
        }
    }
}
