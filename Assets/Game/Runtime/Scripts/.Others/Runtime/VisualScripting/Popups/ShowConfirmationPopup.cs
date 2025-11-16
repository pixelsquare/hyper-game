using System.Collections;
using Kumu.Kulitan.UI;
using Unity.VisualScripting;
using UnityEngine;

namespace Kumu.Kulitan.VisualScripting
{
    [UnitCategory("Kumu/Popups")]
    public class ShowConfirmationPopup : WaitUnit
    {
        [DoNotSerialize]
        public ValueInput title;

        [DoNotSerialize]
        public ValueInput message;

        [DoNotSerialize]
        public ValueInput confirmButton;

        [DoNotSerialize]
        public ValueInput cancelButton;

        [DoNotSerialize]
        public ValueInput delay;

        [DoNotSerialize]
        public ValueOutput didConfirmed;

        private PopupManager PopupManager => PopupManager.Instance;

        protected override void Definition()
        {
            base.Definition();

            title = ValueInput(nameof(title), "Title");
            message = ValueInput(nameof(message), "Message");
            confirmButton = ValueInput(nameof(confirmButton), "Confirm Button");
            cancelButton = ValueInput(nameof(cancelButton), "Cancel Button");
            delay = ValueInput(nameof(delay), 0f);
            didConfirmed = ValueOutput<bool>(nameof(didConfirmed));
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
                    flow.GetValue<string>(confirmButton),
                    flow.GetValue<string>(cancelButton)
            );

            var wait = true;

            popup.OnCancel += () => flow.SetValue(didConfirmed, false);
            popup.OnConfirm += () => flow.SetValue(didConfirmed, true);
            popup.OnClosed += () => wait = false;

            while (wait)
            {
                yield return null;
            }

            yield return exit;
        }
    }
}
