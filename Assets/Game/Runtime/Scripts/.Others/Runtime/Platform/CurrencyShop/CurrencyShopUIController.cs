using System.Collections.Generic;
using Kumu.Extensions;
using Kumu.Kulitan.Avatar;
using Kumu.Kulitan.Backend;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Multiplayer;
using UnityEngine;

namespace Kumu.Kulitan.UI
{
    public class CurrencyShopUIController : MonoBehaviour
    {
        [SerializeField] private CurrencyShopItemButton buttonPrefab;
        [SerializeField] private Transform buttonContainer;
        [SerializeField] private CurrencyShopItemButton highlightedShopItemButton;
        [SerializeField] private List<CurrencyShopItemButton> shopItemButtons;
        private bool hasInitialized;
        private CurrencyShopController ShopController => CurrencyShopController.Instance;

        // Start is called before the first frame update
        public void InitializeShopButtons()
        {
            if (hasInitialized)
            {
                return;
            }

            hasInitialized = true;
            
            var itemList = new List<CurrencyShopItem>();
            itemList.AddRange(ShopController.Items);
            var hItem = itemList.Find(x => x.ItemDefinition.ProductName.Contains("5k") &&
                                           x.IsRealMoney);
            itemList.Remove(hItem);
            itemList.Insert(0, hItem);

            highlightedShopItemButton.Initialize(itemList[0]);
            for (var i = 1; i < itemList.Count; i++)
            {
                var button = Instantiate(buttonPrefab, buttonContainer);
                button.Initialize(itemList[i]);
                shopItemButtons.Add(button);
            }
        }

        private void SetIgnoreDisconnectCallbackToFalse()
        {
            ConnectionManager.Instance.IgnoreDisconnectCallback = false;
        }
        
        private void CurrencyShopControllerOnOnPurchaseFinished(CurrencyShopPurchaseActionArgs obj)
        {
            LoadingScreenManager.Instance.ShowHideLoadingOverlay(false);
            // ConnectionManager.Instance.IgnoreDisconnectCallback = false;
            Invoke(nameof(SetIgnoreDisconnectCallbackToFalse), 0.1f);
            if (!obj.isSuccessful)
            {
                $"Purchase failed using {obj.isRealMoney}, {obj.id}".Log();
                
                var errorString = obj.error.ToLower();
                if (errorString.Contains("error code"))
                {
                    PopupManager.Instance.OpenErrorPopup("Error", "Transaction failed. Something went wrong while trying to verify the receipt.", "OK");
                }
                else if (errorString.Contains("cancelled"))
                {
                    PopupManager.Instance.OpenErrorPopup("Error", "Transaction cancelled.", "OK");
                }
                else
                {
                    PopupManager.Instance.OpenErrorPopup("Error", "Transaction failed.", "OK");
                }

                return;
            }
            
            $"Purchase successful using {obj.isRealMoney}, {obj.id} {obj.walletBalance}".Log();
            var popup = PopupManager.Instance.OpenTextPopup("Success", "Purchase successful.", "OK");
            popup.OnClosed += () => UserInventoryData.UserWallet = new UserWallet(obj.walletBalance);
        }
        
        private void OnEnable()
        {
            CurrencyShopController.OnPurchaseFinished += CurrencyShopControllerOnOnPurchaseFinished;
        }
     
        private void OnDisable()
        {
            CurrencyShopController.OnPurchaseFinished -= CurrencyShopControllerOnOnPurchaseFinished;
        }
    }
}
