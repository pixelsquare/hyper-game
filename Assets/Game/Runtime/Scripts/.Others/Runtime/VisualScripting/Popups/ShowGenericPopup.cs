using System.Collections;
using Kumu.Kulitan.UI;
using Unity.VisualScripting;
using UnityEngine;

namespace Kumu.Kulitan.VisualScripting
{
    [UnitCategory("Kumu/Popups")]
    public class ShowGenericTextPopup : WaitUnit
    {
        [DoNotSerialize] 
        public ValueInput title;

        [DoNotSerialize] 
        public ValueInput message;

        [DoNotSerialize] 
        public ValueInput button;

        [DoNotSerialize] 
        public ValueInput delay;

        private PopupManager PopupManager => PopupManager.Instance;
        
        protected override void Definition()
        {
            base.Definition();

            title = ValueInput(nameof(title), "Title");
            message = ValueInput(nameof(message), "Message");
            button = ValueInput(nameof(button), "Button");
            delay = ValueInput(nameof(delay), 0f);
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

            var popup = PopupManager.CreatePopup(PopupManager.PopupType.GenericTextPopup, 0);
            (popup as GenericTextPopup).SetDetails(flow.GetValue<string>(title), flow.GetValue<string>(message), flow.GetValue<string>(button));
            
            var wait = true;
            popup.OnClosed += () => wait = false;

            while (wait)
            {
                yield return null;
            }

            yield return exit;
        }
    }
}
