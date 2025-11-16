using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Kumu.Extensions;
using Kumu.Kulitan.Avatar;
using UnityEngine;
using UnityEngine.Scripting;

namespace Kumu.Kulitan.SROptions
{
    public class SelectedItemsSROptions : UbeSROptions
    {
        private const string ITEM_ID = "UB_tshirt_muscletees_01_G";

        [Preserve]
        [Category("SelectedItems")]
        public void InjectUpperBodyItemToSelectedItems()
        {
            if (!ItemDatabase.Current.TryGetItem(ITEM_ID, out var itemConfig))
            {
                $"Could not find ItemConfig in ItemDatabase! ItemId: {ITEM_ID}".LogError();
                return;
            }

            var selectionContoller = Object.FindObjectOfType<ItemSelectionController>();
            var selectedItems = selectionContoller.GetType()
                                                  .GetField("selectedItems", BindingFlags.Instance | BindingFlags.NonPublic)
                                                  .GetValue(selectionContoller) as Dictionary<string, AvatarItemConfig>;
            
            selectedItems.Add(ITEM_ID, ItemDatabase.Current.GetItem(ITEM_ID));
        }
    }
}
