using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kumu.Kulitan.Avatar;
using Unity.VisualScripting;

namespace Kumu.Kulitan.VisualScripting
{
    public class UnitUncartItems : Unit
    {
        [DoNotSerialize] private ControlInput inputTrigger;
        [DoNotSerialize] private ControlOutput outputTrigger;

        [DoNotSerialize] private ValueInput itemSelectionController;
        [DoNotSerialize] private ValueInput avatarShopCart;
        [DoNotSerialize] private ValueInput avatarDefaultItems;
        
        protected override void Definition()
        {
            itemSelectionController = ValueInput<ItemSelectionController>(nameof(itemSelectionController));
            avatarShopCart = ValueInput<AvatarShopCart>(nameof(avatarShopCart));
            avatarDefaultItems = ValueInput<DefaultItemData>(nameof(avatarDefaultItems));

            inputTrigger = ControlInputCoroutine(nameof(inputTrigger), UncartItems);
            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }

        private IEnumerator UncartItems(Flow flow)
        {
            var selectionController = flow.GetValue<ItemSelectionController>(itemSelectionController);
            var shopCart = flow.GetValue<AvatarShopCart>(avatarShopCart);
            var defaultItems = flow.GetValue<DefaultItemData>(avatarDefaultItems);

            var tasks = new List<Task>();

            foreach (var item in shopCart.DeselectedItems)
            {
                if (defaultItems.TryGetItem(item.State.ItemType, out var defaultItem))
                {
                    var task = selectionController.SelectItem(defaultItem.Data.itemId, ItemSelectionMode.All);
                    tasks.Add(task);
                }
                else
                {
                    var task = selectionController.DeselectItem(item.Data.itemId, ItemSelectionMode.All);
                    tasks.Add(task);
                }
            }

            var mainTask = Task.WhenAll(tasks);

            while (!mainTask.IsCompleted)
            {
                yield return null;
            }
            
            yield return outputTrigger;
        }
    }
}
