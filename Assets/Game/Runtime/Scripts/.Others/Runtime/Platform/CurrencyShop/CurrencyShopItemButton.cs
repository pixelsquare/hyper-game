using Kumu.Extensions;
using Kumu.Kulitan.Avatar;
using Kumu.Kulitan.Backend;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Multiplayer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kumu.Kulitan.UI
{
    public class CurrencyShopItemButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI productName;
        [SerializeField] private TextMeshProUGUI productPrice;
        [SerializeField] private Image imgIcon;

        private CurrencyShopItem item;

        public void Initialize(CurrencyShopItem shopItem)
        {
            item = shopItem;
            if (shopItem.ItemDefinition.Icon != null)
            {
                imgIcon.sprite = shopItem.ItemDefinition.Icon;
            }
            productPrice.text = ParsePriceString(shopItem.PriceString);
            productName.text = ParseProductName(shopItem);
        }

        private string ParseProductName(CurrencyShopItem item)
        {
            var parsedName = $"{item.ItemDefinition.CurrencyGranted.amount:n0}";
            return parsedName;
        }

        private string ParsePriceString(string priceString)
        {
            var parsedPrice = priceString.Replace("UDI", "<sprite=0>");
            return parsedPrice;
        }

        private void OnButtonClick()
        {
            if (item.IsRealMoney)
            {
                PurchaseItem();
                return;
            }

            if (IsBalanceSufficient(item.ItemDefinition.CurrencyCost.amount))
            {
                var confirmPopup = (ConfirmationPopup)PopupManager.Instance.OpenConfirmationPopup("Confirm Purchase", $"Are you sure you want to buy " +
                    $"{productName.text} coins for {productPrice.text}?", "Buy", "Cancel");
                confirmPopup.OnConfirm += PurchaseItem;
                return;
            }

            PopupManager.Instance.OpenErrorPopup("Error", "Not enough diamonds " +
                "to complete this transaction.", "OK");
        }

        private void PurchaseItem()
        {
            $"Processing purchase. Product:{item.ItemDefinition.ProductId} UseRealMoney:{item.IsRealMoney}".Log();
            LoadingScreenManager.Instance.ShowHideLoadingOverlay(true);
            ConnectionManager.Instance.IgnoreDisconnectCallback = true;
            CurrencyShopController.Instance.PurchaseCurrencyShopItem(item);
        }

        private bool IsBalanceSufficient(int price)
        {
            var walletBalance = UserInventoryData.UserWallet.GetCurrency(Currency.UBE_DIA);
            return walletBalance.amount >= price;
        }

        private void OnEnable()
        {
            button.onClick.AddListener(OnButtonClick);
        }
        
        private void OnDisable()
        {
            button.onClick.RemoveListener(OnButtonClick);
        }
    }
}
