using System.Collections;
using Kumu.Kulitan.Avatar;
using Unity.VisualScripting;

namespace Kumu.Kulitan.VisualScripting
{
    public class UnitResetItems : Unit
    {
        [DoNotSerialize] private ControlInput inputTrigger;
        [DoNotSerialize] private ControlOutput outputTrigger;

        [DoNotSerialize] private ValueInput itemSelectionController;
        
        protected override void Definition()
        {
            itemSelectionController = ValueInput<ItemSelectionController>(nameof(itemSelectionController));

            inputTrigger = ControlInputCoroutine(nameof(inputTrigger), ResetItems);
            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }

        private IEnumerator ResetItems(Flow flow)
        {
            var selectionController = flow.GetValue<ItemSelectionController>(itemSelectionController);

            var task = selectionController.DeselectAllUnequippedItems();

            while (!task.IsCompleted)
            {
                yield return null;
            }
            
            yield return outputTrigger;
        }
    }
}
