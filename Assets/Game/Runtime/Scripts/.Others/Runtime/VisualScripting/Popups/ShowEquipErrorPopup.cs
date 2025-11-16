using System.Collections;
using Kumu.Kulitan.UI;
using Unity.VisualScripting;
using UnityEngine;

namespace Kumu.Kulitan.VisualScripting
{
    [UnitCategory("Kumu/Popups")]
    public class ShowEquipErrorPopup : WaitUnit
    {
        [DoNotSerialize] public ValueInput title;
        [DoNotSerialize] public ValueInput message;
        [DoNotSerialize] public ValueInput confirm;
        [DoNotSerialize] public ValueInput cancel;
        [DoNotSerialize] public ValueInput delay;
        
        [DoNotSerialize] public ValueOutput hasConfirmed;

        private PopupManager PopupManager => PopupManager.Instance;
        
        protected override void Definition()
        {
            base.Definition();

            title = ValueInput(nameof(title), "Title");
            message = ValueInput(nameof(message), "Message");
            confirm = ValueInput(nameof(confirm), "Confirm");
            cancel = ValueInput(nameof(cancel), "Cancel");
            delay = ValueInput(nameof(delay), 0f);
            
            hasConfirmed = ValueOutput<bool>(nameof(hasConfirmed));
        }

        protected override IEnumerator Await(Flow flow)
        {
            var time = flow.GetValue<float>(delay);
            yield return new WaitForSeconds(time);

            if (!PopupManager.IsPopupPriority(0))
            {
                yield return exit;
                yield break;
            }

            var popup = PopupManager.CreatePopup(PopupManager.PopupType.ConfirmationPopup, 0) as ConfirmationPopup;
            popup.SetDetails(
                flow.GetValue<string>(title),
                flow.GetValue<string>(message),
                flow.GetValue<string>(confirm),
                flow.GetValue<string>(cancel));
            
            var wait = true;

            popup.OnCancel += () => flow.SetValue(hasConfirmed, false);
            popup.OnConfirm += () => flow.SetValue(hasConfirmed, true);
            popup.OnClosed += () => wait = false;

            while (wait)
            {
                yield return null;
            }

            yield return exit;
        }
    }
}
