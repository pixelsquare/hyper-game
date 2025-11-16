using Kumu.Kulitan.Avatar;
using Unity.VisualScripting;

namespace Kumu.Kulitan.VisualScripting
{
    public class UnitItemSelectionAllOwned : Unit
    {
        [DoNotSerialize] private ControlInput inputTrigger;
        [DoNotSerialize] private ControlOutput trueTrigger;
        [DoNotSerialize] private ControlOutput falseTrigger;

        [DoNotSerialize] private ValueInput itemSelectionController;
        
        protected override void Definition()
        {
            itemSelectionController = ValueInput<ItemSelectionController>(nameof(itemSelectionController));

            inputTrigger = ControlInput(nameof(inputTrigger), CheckState);
            trueTrigger = ControlOutput(nameof(trueTrigger));
            falseTrigger = ControlOutput(nameof(falseTrigger));
            
            Succession(inputTrigger, trueTrigger);
            Succession(inputTrigger, falseTrigger);
        }

        private ControlOutput CheckState(Flow flow)
        {
            var selectionController = flow.GetValue<ItemSelectionController>(itemSelectionController);

            if (selectionController.IsAllOwned())
            {
                return trueTrigger;
            }

            return falseTrigger;
        }
    }
}
