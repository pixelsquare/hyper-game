using System;
using System.Collections.Generic;
using System.Linq;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.UI;
using Kumu.Kulitan.Backend;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Kumu.Kulitan.Avatar
{
    public class AvatarShopCart : MonoBehaviour
    {
        [SerializeField] private OnItemsCheckoutEvent onCheckout;
        [SerializeField] private Button exitButton;
        [SerializeField] private Button checkoutButton;
        [SerializeField] private TextMeshProUGUI itemAmountLabel;
        [SerializeField] private CombinedCurrencyIndicator currencyIndicator;
        [SerializeField] private AvatarItem prefab;
        [SerializeField] private Transform contentContainer;
        [SerializeField] private AvatarShopCartBalanceChecker balanceChecker;
        
        private readonly List<AvatarItem> itemDisplays = new();
        private readonly Dictionary<string, CartItem> checkoutCart = new();

        private Currency totalCoinCost;
        private Currency totalDiamondCost;

        private int itemAmount;
        private int unownedAmount;

        private FSMSendUnityEvent sendCheckoutUnityEvent;
        private FSMSendUnityEvent sendItemSelectUnityEvent;

        public List<AvatarItemConfig> CheckoutItems => GetCheckoutItems().ToList();
        public List<AvatarItemConfig> DeselectedItems => GetDeselectedItems().ToList();

        /// <summary>
        /// Generates AvatarItem displays based on the given data.
        /// </summary>
        public void LoadItems(List<AvatarItemConfig> itemConfigs)
        {
            ClearItems();

            foreach (var config in itemConfigs)
            {
                var cartItem = new CartItem(true, config);
                checkoutCart.Add(config.Data.itemId, cartItem);

                var avatarCartItem = Instantiate(prefab, contentContainer);
                itemDisplays.Add(avatarCartItem);
                avatarCartItem.Initialize(config);
                avatarCartItem.AddListenerOnToggle(isIncluded => OnItemIncluded(config, isIncluded));
                avatarCartItem.Toggle(true);
            }
        }

        /// <summary>
        /// Invokes the onCheckout event.
        /// This should be used for integration of backend events with regards to wallet functionality. 
        /// </summary>
        public void AttemptPurchase()
        {
            onCheckout.Invoke(GetCheckoutItems());
        }

        /// <summary>
        /// Invoked when an item in the cart is included or excluded.
        /// Updates total price and item amount accordingly.
        /// </summary>
        /// <param name="isIncluded">True if item is included in cart. Otherwise false.</param>
        private void OnItemIncluded(AvatarItemConfig item, bool isIncluded)
        {
            if (isIncluded)
            {
                itemAmount++;

                if (!UserInventoryData.IsItemOwned(item.Data.itemId))
                {
                    CalculateCurrencyCost(item.Data.cost);
                    unownedAmount++;
                }
            }
            else
            {
                itemAmount--;

                if (!UserInventoryData.IsItemOwned(item.Data.itemId))
                {
                    CalculateCurrencyCost(item.Data.cost, false);
                    unownedAmount--;
                }
            }
     
            checkoutCart[item.Data.itemId].isIncluded = isIncluded;

            itemAmountLabel.text = $"BUY {unownedAmount} ITEMS";
            currencyIndicator.UpdateValue(totalCoinCost.amount, totalDiamondCost.amount);
            balanceChecker.CheckSufficientBalance(totalCoinCost.amount, totalDiamondCost.amount);
            
            if (itemAmount <= 0)
            { 
                checkoutButton.interactable = false;
            }
        }

        private void CalculateCurrencyCost(Currency cost, bool addTo = true)
        {
            if (!string.IsNullOrEmpty(cost.code) && cost.code.Equals(Currency.UBE_DIA))
            {
                totalDiamondCost.amount += addTo ? cost.amount : -cost.amount;
                return;
            }

            totalCoinCost.amount += addTo ? cost.amount : -cost.amount;
        }

        private IEnumerable<AvatarItemConfig> GetCheckoutItems()
        {
            return from item in checkoutCart
                   where item.Value.isIncluded
                   select item.Value.itemData;
        }

        private IEnumerable<AvatarItemConfig> GetDeselectedItems()
        {
            return from item in checkoutCart
                   where !item.Value.isIncluded
                   select item.Value.itemData;
        }

        /// <summary>
        /// Clears the cart items and its displays.
        /// </summary>
        private void ClearItems()
        {
            checkoutCart.Clear();
            totalCoinCost.amount = 0;
            totalDiamondCost.amount = 0;
            itemAmount = 0;

            foreach (var item in itemDisplays)
            {
                Destroy(item.gameObject);
            }

            itemDisplays.Clear();
        }

        private void OnCheckoutClicked()
        {   
            GlobalNotifier.Instance.Trigger(sendCheckoutUnityEvent);
            PopupManager.Instance.CloseScenePopup("AvatarShopCartPopup");
        }

        private void OnExitClicked()
        {
            GlobalNotifier.Instance.Trigger(sendItemSelectUnityEvent);
            PopupManager.Instance.CloseScenePopup("AvatarShopCartPopup");
        }

        private void Awake()
        {
            sendCheckoutUnityEvent = new FSMSendUnityEvent("Checkout");
            sendItemSelectUnityEvent = new FSMSendUnityEvent("ItemSelect");
        }

        private void OnEnable()
        {
            exitButton.onClick.AddListener(OnExitClicked);
            checkoutButton.onClick.AddListener(OnCheckoutClicked);
            balanceChecker.UpdateBalance();
        }

        private void OnDisable()
        {
            exitButton.onClick.RemoveListener(OnExitClicked);
            checkoutButton.onClick.RemoveListener(OnCheckoutClicked);
        }

        private class CartItem
        {
            public bool isIncluded;
            public readonly AvatarItemConfig itemData;

            public CartItem(bool isIncluded, AvatarItemConfig itemData)
            {
                this.isIncluded = isIncluded;
                this.itemData = itemData;
            }
        }

        [Serializable]
        private class OnItemsCheckoutEvent : UnityEvent<IEnumerable<AvatarItemConfig>> { }
    }
}
