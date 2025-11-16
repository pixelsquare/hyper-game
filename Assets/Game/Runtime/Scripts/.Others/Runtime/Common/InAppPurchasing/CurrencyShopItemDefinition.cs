using Kumu.Kulitan.Backend;
using UnityEngine;

namespace Kumu.Kulitan.Common
{
    [CreateAssetMenu(menuName = "Kumu/CurrencyShop/CurrencyShopItem", fileName = "CurrencyShopItem", order = 0)]
    public class CurrencyShopItemDefinition : ScriptableObject
    {
        [SerializeField] private string devProductId = "kumu.ph.kulitan_stg.coins1k";
        [SerializeField] private string productId = "kumu.ph.kulitan.coins1k";
        [SerializeField] private PurchasingMethod purchaseMethod;
        [SerializeField] private Currency currencyGranted;
        [SerializeField] private Sprite icon;
        
        [Header("For non-real money products")]
        [SerializeField] private string productName;
        [SerializeField] private Currency currencyCost;

        public string ProductId
        {
            get
            {
#if UBE_RELEASE
                return productId;
#else
                return devProductId;
#endif
            }
        }
        public PurchasingMethod PurchaseMethod => purchaseMethod;
        public Currency CurrencyGranted => currencyGranted;
        public Sprite Icon => icon;
        
        /// <summary>
        /// Only for non-real money product.
        /// </summary>
        public string ProductName => productName;

        /// <summary>
        /// Only for non-real money product.
        /// </summary>
        public Currency CurrencyCost => currencyCost;
    }
}
