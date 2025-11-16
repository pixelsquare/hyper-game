using System.Collections;
using System.Linq;
using Kumu.Kulitan.Avatar;
using Kumu.Kulitan.Backend;
using Unity.VisualScripting;

namespace Kumu.Kulitan.VisualScripting
{
    public class UnitEquipItems : Unit
    {
        [DoNotSerialize] 
        public ValueInput itemSelectionController;
     
        [DoNotSerialize]
        public ValueOutput error;
        
        [DoNotSerialize]
        public ControlInput inputTrigger;

        [DoNotSerialize]
        public ControlOutput outputTrigger;
        
        [DoNotSerialize]
        public ControlOutput errorTrigger;

        protected override void Definition()
        {
            itemSelectionController = ValueInput<ItemSelectionController>(nameof(itemSelectionController));
            error = ValueOutput<ServiceError>(nameof(error));
            
            inputTrigger = ControlInputCoroutine(nameof(inputTrigger), RunCoroutine);
            outputTrigger = ControlOutput(nameof(outputTrigger));
            errorTrigger = ControlOutput(nameof(errorTrigger));
        
            Succession(inputTrigger, outputTrigger);
        }

        private IEnumerator RunCoroutine(Flow flow)
        {
            var selectionController = flow.GetValue<ItemSelectionController>(itemSelectionController);
            
            var selectedItems = selectionController.SelectedItems;
            var request = new EquipItemsRequest
            {
                equippedItems = selectedItems.Select(a => a.State).ToArray()
            };

            var task = Services.InventoryService.EquipItemsAsync(request);

            while (!task.IsCompleted)
            {
                yield return null;
            }

            if (task.Result.HasError)
            {
                flow.SetValue(error, task.Result.Error);
                yield return errorTrigger;
                yield break;
            }

            UserInventoryData.EquippedItems = task.Result.Result.equippedItems;
            
            yield return outputTrigger;
        }
    }
}
