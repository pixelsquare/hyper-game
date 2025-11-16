using Kumu.Kulitan.Avatar;
using Unity.VisualScripting;

namespace Kumu.Kulitan.VisualScripting
{
    public class UnitValidateItems : Unit
    {
        [DoNotSerialize] private ControlInput inputTrigger;
        [DoNotSerialize] private ControlOutput outputTrigger;

        [DoNotSerialize] private ValueInput avatarDefaultItems;
        [DoNotSerialize] private ValueInput itemCategorySelector;
        [DoNotSerialize] private ValueOutput isValid;

        protected override void Definition()
        {
            avatarDefaultItems = ValueInput<DefaultItemData>(nameof(avatarDefaultItems), null);
            itemCategorySelector = ValueInput<ItemCategorySelector>(nameof(itemCategorySelector), null);

            inputTrigger = ControlInput(nameof(inputTrigger), ValidateItems);
            outputTrigger = ControlOutput(nameof(outputTrigger));
            isValid = ValueOutput<bool>(nameof(isValid));

            Succession(inputTrigger, outputTrigger);
        }

        private ControlOutput ValidateItems(Flow flow)
        {
            var defaultItems = flow.GetValue<DefaultItemData>(avatarDefaultItems);
            var itemCategories = flow.GetValue<ItemCategorySelector>(itemCategorySelector);                       

            var equipFlags = AvatarItemType.None;
            var defaultFlags = AvatarItemType.None;
            
            foreach (var itemState in UserInventoryData.EquippedItems)
            {   
                if (ItemDatabase.Current.TryGetItem(itemState.itemId, out var itemConfig))
                {
                    equipFlags &= AvatarItemUtil.ToAvatarItemType(itemConfig.GetTypeCode());

                    if (itemCategories.TryGetValue(itemConfig.Data, out var itemCategory))
                    {
                        equipFlags &= itemCategory.ItemType;
                        equipFlags &= itemCategory.DeselectedTypes;
                    }
                }
            }

            foreach (var item in defaultItems.AllItems())
            {
                if (ItemDatabase.Current.TryGetItem(item.Data.itemId, out var itemConfig))
                {
                    defaultFlags &= AvatarItemUtil.ToAvatarItemType(itemConfig.GetTypeCode());

                    if (itemCategories.TryGetValue(itemConfig.Data, out var itemCategory))
                    {
                        defaultFlags &= itemCategory.ItemType;
                        defaultFlags &= itemCategory.DeselectedTypes;
                    }
                }
            }

            var hasEmptySlot = (defaultFlags & equipFlags) != defaultFlags;
            
            flow.SetValue(isValid, !hasEmptySlot);    

            return outputTrigger;
        }
    }
}
