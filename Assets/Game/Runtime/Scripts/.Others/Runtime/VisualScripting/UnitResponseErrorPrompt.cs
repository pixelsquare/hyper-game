using Kumu.Kulitan.Backend;
using Kumu.Kulitan.UI;
using Unity.VisualScripting;

namespace Kumu.Kulitan.VisualScripting
{
    public class UnitResponseErrorPrompt : Unit
    {
        [DoNotSerialize] private ControlInput enter;
        [DoNotSerialize] private ControlOutput exit;
        [DoNotSerialize] private ValueInput serviceError;
        
        protected override void Definition()
        {
            enter = ControlInput(nameof(enter), DisplayPrompt);
            exit = ControlOutput(nameof(exit));
            serviceError = ValueInput<ServiceError>(nameof(serviceError));
            
            Succession(enter, exit);
        }

        private ControlOutput DisplayPrompt(Flow flow)
        {
            var errorValue = flow.GetValue<ServiceError>(serviceError);

            PopupManager.Instance.OpenErrorPopup($"Error {errorValue.Code}", errorValue.Message, "Confirm");

            return exit;
        }
    }
}
