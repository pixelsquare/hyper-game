using System.Collections;
using Kumu.Kulitan.UI;
using Unity.VisualScripting;

namespace Kumu.Kulitan.VisualScripting
{
    public class UnitPromptTextPopup : WaitUnit
    {
        [DoNotSerialize] public ValueInput title;
        [DoNotSerialize] public ValueInput message;
        [DoNotSerialize] public ValueInput confirm;
        
        protected override void Definition()
        {
            title = ValueInput(nameof(title), string.Empty);
            message = ValueInput(nameof(message), string.Empty);
            confirm = ValueInput(nameof(confirm), string.Empty);
            base.Definition();
        }

        protected override IEnumerator Await(Flow flow)
        {
            var titleValue = flow.GetValue<string>(title);
            var messageValue = flow.GetValue<string>(message);
            var confirmValue = flow.GetValue<string>(confirm);

            var isFinished = false;
            
            var popup = PopupManager.Instance.OpenTextPopup(titleValue, messageValue, confirmValue);
            popup.OnClosed += () => isFinished = true;

            while (!isFinished)
            {
                yield return null;
            }

            yield return exit;
        }
    }
}
