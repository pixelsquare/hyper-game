using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    using UIEvent = GameEvents.UserInterface;

    public class InventoryCategorySelector : MonoBehaviour
    {
        [Inject] private InventoryHud _inventoryHud;

        private InventoryCategoryButton _lastButtonSelected;

        private void GenerateFilteredItems(InventoryCategoryType filter)
        {
            _inventoryHud.GenerateCategoryItems(filter);
        }

        private void HandleFilterSelected(IMessage message)
        {
            if (message.Data is not InventoryCategoryType categoryType 
             || message.Sender is not InventoryCategoryButton button 
             || button == _lastButtonSelected)
            {
                return;
            }

            _lastButtonSelected?.SetButtonSelected(false);

            GenerateFilteredItems(categoryType);
            _lastButtonSelected = button;
        }

        private void OnEnable()
        {
            Dispatcher.AddListener(UIEvent.OnInventoryFilterSelected, HandleFilterSelected, true);
        }

        private void OnDisable()
        {
            Dispatcher.RemoveListener(UIEvent.OnInventoryFilterSelected, HandleFilterSelected, true);
        }
    }
}
