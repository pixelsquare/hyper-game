using System.Collections.Generic;
using Kumu.Kulitan.Avatar;
using Unity.VisualScripting;

namespace Kumu.Kulitan.VisualScripting
{
    public class UnitItemSelectionAllEquipped : Unit
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
            
            if (selectionController.IsAllEquipped()
                && CompareEquipSelection(selectionController))
            {
                return trueTrigger;
            }

            return falseTrigger;
        }

        /// <summary>
        /// Compares the array of equipped items against the array of selected items.
        /// </summary>
        /// <returns>True if there is no difference between current equipment and current selection. Otherwise returns false.</returns>
        private bool CompareEquipSelection(ItemSelectionController selectionController)
        {
            var equipItems = new HashSet<string>();
            var selectItems = new HashSet<string>();

            foreach (var equip in UserInventoryData.EquippedItems)
            {
                equipItems.Add(equip.GetHashString());
            }

            foreach (var item in selectionController.SelectedItems)
            {
                selectItems.Add(item.State.GetHashString());
            }
            
            // not a setter method, this checks if the two hash -Sets- contain the same values
            return equipItems.SetEquals(selectItems);
        }
    }
}
