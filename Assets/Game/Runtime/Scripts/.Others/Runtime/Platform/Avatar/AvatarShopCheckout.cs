using System;
using System.Collections.Generic;
using System.Linq;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Kumu.Kulitan.Avatar
{
    public class AvatarShopCheckout : MonoBehaviour
    {
        private const string CHECKOUT_LABEL = "Buy";

        [SerializeField] private OnItemsConfirmedEvent onCartItems;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Image confirmButtonImage;
        [SerializeField] private TextMeshProUGUI confirmButtonLabel;
        [SerializeField] private ItemSelectionController selectionController;

        private FSMSendUnityEvent fsmSendUnityEvent;
        private AvatarShopCart shopCart;
        
        public AvatarShopCart ShopCart => shopCart;

        /// <summary>
        /// Opens the AvatarShopCart popup.
        /// Used in the FSM.
        /// </summary>
        public void ShowCartPopup()
        {
            shopCart = null;
            PopupManager.Instance.OpenScenePopup("AvatarShopCartPopup", () =>
            {
                shopCart = (AvatarShopCart)FindObjectOfType(typeof(AvatarShopCart));
                GlobalNotifier.Instance.Trigger(fsmSendUnityEvent);
            });
        }

        /// <summary>
        /// Returns filtered carted items that are not owned.
        /// </summary>
        public List<AvatarItemConfig> CartItems
        {
            get
            {
                var items = from item in selectionController.SelectedItems
                            where !UserInventoryData.IsItemOwned(item.Data.itemId)
                            select item;
                return items.ToList();
            }
        }

        /// <summary>
        /// Returns filtered carted items that are owned.
        /// </summary>
        public List<AvatarItemConfig> EquipItems
        {
            get
            {
                var items = from item in selectionController.SelectedItems
                            where UserInventoryData.IsItemOwned(item.Data.itemId)
                            select item;
                return items.ToList();
            }
        }

        /// <summary>
        /// Invoked when an item is selected.
        /// Adds and removes the item to and from the cart respectively.
        /// Button functionality updates depending on the cart content.
        /// </summary>
        /// <param name="isSelected">True if the item is selected. Otherwise false.</param>
        public void OnItemSelected(AvatarItemConfig itemConfig, bool isSelected)
        {
            confirmButton.onClick.RemoveAllListeners();

            if (ItemsAllOwned())
            {
                ShowButton(false);
                return;
            }
         
            ShowButton(true);
            confirmButton.onClick.AddListener(OnCartItems);
            confirmButtonLabel.text = $"{CHECKOUT_LABEL} {CartItems.Count}";
        }

        /// <summary>
        /// Clears the cart and its displays.
        /// </summary>
        public void ClearCheckout()
        {
            ShowButton(false);
            confirmButton.onClick.RemoveAllListeners();
        }
        
        /// <summary>
        /// Checks if all items in the cart are owned.
        /// </summary>
        /// <returns>True if all items are owned. Otherwise false.</returns>
        public bool ItemsAllOwned()
        {
            return selectionController.SelectedItems
                .All(item => UserInventoryData.IsItemOwned(item.Data.itemId));
        }

        /// <summary>
        /// Invoked when the cart has at least one unowned item.
        /// </summary>
        private void OnCartItems()
        {
            onCartItems.Invoke(CartItems);
        }

        private void ShowButton(bool isVisible)
        {
            confirmButtonImage.enabled = isVisible;
            confirmButton.interactable = isVisible;
            confirmButtonLabel.gameObject.SetActive(isVisible);
        }

        private void Awake()
        {
            fsmSendUnityEvent = new FSMSendUnityEvent("OnAvatarShopCartOpened");
        }

        [Serializable]
        private class OnItemsConfirmedEvent : UnityEvent<IEnumerable<AvatarItemConfig>> { }
    }
}
