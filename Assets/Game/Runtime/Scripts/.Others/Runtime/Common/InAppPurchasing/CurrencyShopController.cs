using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kumu.Extensions;
using Kumu.Kulitan.Backend;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using Product = UnityEngine.Purchasing.Product;

namespace Kumu.Kulitan.Common
{
    public struct CurrencyShopPurchaseActionArgs
    {
        public string id;
        public bool isRealMoney;
        public bool isSuccessful;
        public Currency[] walletBalance;
        public string error;
        public string errorMessage;
    }

    public class CurrencyShopController : SingletonMonoBehaviour<CurrencyShopController>, IDetailedStoreListener
    {
        private List<CurrencyShopItem> items = new();

        private Dictionary<string, CurrencyShopItemDefinition> idToDefinitionMap = new();

        private IStoreController storeController;
        public List<CurrencyShopItem> Items => items;
        public bool IsInitialized { get; private set; }
        
        public bool PurchaseInProgress { get; private set; }
        
        [SerializeField] private List<CurrencyShopItemDefinition> itemDefinitions;

        private StandardPurchasingModule PurchasingModule => StandardPurchasingModule.Instance();
        
        public static event Action<bool> OnInitializeFinished;

        public static event Action<CurrencyShopPurchaseActionArgs> OnPurchaseFinished;

        public void InitializeAndSetNonRealMoneyCosts(CurrencyShopProductCostData[] costData) // add itemDefinitions as parameter
        {
            if (IsInitialized)
            {
                $"[CurrencyShop] {nameof(CurrencyShopController)} already initialized! Skipping Initialize()!".Log();
                return;
            }

            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
                "[CurrencyShop] UnityServices not yet initialized!".LogError();
                return;
            }
            
            InitializeMap();
            InitializeFakeStore();
            PrepareNonRealMoneyItems(costData);
            PrepareRealMoneyItems(GetRealMoneyDefinitions(itemDefinitions));
        }

        public void PurchaseCurrencyShopItem(CurrencyShopItem item)
        {
            if (!IsInitialized)
            {
                "[CurrencyShop] Uninitialized! Skipping purchase.".LogError();
                return;
            }
            
            if (PurchaseInProgress)
            {
                "[CurrencyShop] Cannot initiate a purchase as another one is still in progress.".LogError();
                return;
            }

            PurchaseInProgress = true;
            
            if (item.IsRealMoney)
            {
                PurchaseUsingRealMoney(item);
            }
            else
            {
                PurchaseUsingNonRealMoney(item);
            }
        }

        public CurrencyShopItem FindItemWithId(CurrencyShopItemDefinition itemDefinition, bool isRealMoney)
        {
            return FindItemWithId(itemDefinition.ProductId, isRealMoney);
        }

        public CurrencyShopItem FindItemWithId(string productId, bool isRealMoney)
        {
            if (!IsInitialized)
            {
                "[CurrencyShop] Uninitialized! Returning null.".LogError();
                return default;
            }

            var item = items.Find(i => i.ItemDefinition.ProductId == productId && i.IsRealMoney == isRealMoney);

            return item;
        }

        private void PurchaseUsingRealMoney(CurrencyShopItem item)
        {
            storeController.InitiatePurchase(item.ItemDefinition.ProductId);
        }

        private void PurchaseUsingNonRealMoney(CurrencyShopItem item)
        {
            StartPurchaseUsingNonRealMoney(item.ItemDefinition.ProductId);
        }

        private void InitializeMap()
        {
            foreach (var def in itemDefinitions)
            {
                idToDefinitionMap.Add(def.ProductId, def);
            }
        }

        /// <summary>
        /// Sets the store to be used by IAP depending on environment. DEV = fake store, STG,PROD = real store
        /// </summary>
        private void InitializeFakeStore()
        {
            var isUsingFakeStore = false;
#if UBE_DEV
            isUsingFakeStore = true; // Only use fake store in Dev environment
#endif
            PurchasingModule.useFakeStoreAlways = isUsingFakeStore;
            PurchasingModule.useFakeStoreUIMode = FakeStoreUIMode.StandardUser;
        }

        private void PrepareRealMoneyItems(IEnumerable<CurrencyShopItemDefinition> itemDefinitions)
        {
            var builder = ConfigurationBuilder.Instance(PurchasingModule);

            foreach (var def in itemDefinitions)
            {
                builder.AddProduct(def.ProductId, ProductType.Consumable);
            }
            
            UnityPurchasing.Initialize(this, builder);
        }

        private void PrepareNonRealMoneyItems(CurrencyShopProductCostData[] costData)
        {
            foreach (var d in costData)
            {
                var hasItemDefinition = idToDefinitionMap.TryGetValue(d.id, out var def);
                if (!hasItemDefinition)
                {
                    $"Could not find item definition for item id:{d.id}".LogError();
                    continue;
                }
                
                items.Add(new NonRealMoneyCurrencyShopItem(def, d.cost));
            }
        }

        private IEnumerable<CurrencyShopItemDefinition> GetRealMoneyDefinitions(IEnumerable<CurrencyShopItemDefinition> definitions)
        {
            return definitions.Where(i => i.PurchaseMethod.HasFlag(PurchasingMethod.RealMoney));
        }

        private IEnumerable<ProductAdapter> GenerateProductAdapters(IStoreController controller)
        {
            return controller.products.all.Select(p => new ProductAdapter
            {
                id = p.definition.id,
                priceString = p.metadata.localizedPriceString
            });
        }
        
        #region Non real money purchases

        private void StartPurchaseUsingNonRealMoney(string itemId)
        {
            PurchaseUsingNonRealMoneyAsync(itemId);
        }

        private async Task PurchaseUsingNonRealMoneyAsync(string itemId)
        {
            var result = await Services.CurrencyShopService.BuyCurrencyAsync(new BuyCurrencyRequest
            {
                id = itemId
            });

            var args = new CurrencyShopPurchaseActionArgs
            {
                id = itemId,
                isRealMoney = false
            };

            if (result.HasError)
            {
                args.isSuccessful = false;
                args.error = $"Purchase failed. Error code:{result.Error.Code.ToString()}";
                args.errorMessage = result.Error.Message;
            }
            else
            {
                args.isSuccessful = true;
                args.walletBalance = result.Result.walletBalance;
            }

            PurchaseInProgress = false;
            OnPurchaseFinished?.Invoke(args);
        }
        
        #endregion
        
        #region Validation

        private void StartValidatingReceipt(Product product)
        {
            ValidateReceiptAsync(product);
        }

        private async Task ValidateReceiptAsync(Product product)
        {
            // Validate here
            var currencyShopService = Services.CurrencyShopService;
            var result = await currencyShopService.ValidateReceiptAsync(new ValidateReceiptRequest
            {
                id = product.definition.id,
                receipt = product.receipt
            });

            var args = new CurrencyShopPurchaseActionArgs
            {
                id = product.definition.id,
                isRealMoney = true
            };

            if (result.HasError)
            {
                args.isSuccessful = false;
                args.error = $"Receipt validation failed. Error code:{result.Error.Code.ToString()}";
                args.errorMessage = result.Error.Message;
            }
            else
            {
                args.isSuccessful = true;
                args.walletBalance = result.Result.walletBalance;
            }
            
            storeController.ConfirmPendingPurchase(product);
            PurchaseInProgress = false;

            OnPurchaseFinished?.Invoke(args);
        }

        #endregion
        
        #region IDetailedStoreListener

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            storeController = controller;

            var products = GenerateProductAdapters(controller);

            foreach (var p in products)
            {
                var id = p.id;
                var hasDefinition = idToDefinitionMap.TryGetValue(id, out var def);

                if (!hasDefinition)
                {
                    $"[CurrencyShop] Could not find definition for {id} in {nameof(idToDefinitionMap)}!".LogError();
                    continue;
                }
                
                if (idToDefinitionMap.ContainsKey(p.id))
                {
                    items.Add(new RealMoneyCurrencyShopItem(def, p.priceString));
                }
            }
            
            "[CurrencyShop] UnityPurchasing has initialized!".Log();
            
            IsInitialized = true;
            
            OnInitializeFinished?.Invoke(true);
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            OnInitializeFailed(error, default);
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            $"[CurrencyShop] UnityPurchasing has failed to initialize! Reason: {error.ToString()}, Message: {message}".LogError();
            
            OnInitializeFinished?.Invoke(false);
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            $"[CurrencyShop] Processing purchase for {purchaseEvent.purchasedProduct.definition.id}...".Log();

            // Need to validate receipt...
            StartValidatingReceipt(purchaseEvent.purchasedProduct);
            
            return PurchaseProcessingResult.Pending; // TODO: Replace this with pending and only complete once receipt is validated by the backend
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            OnPurchaseFailed(product, (PurchaseFailureDescription)default);
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        {
            $"[CurrencyShop] UnityPurchasing has failed to complete the purchase for {product.definition.id}! Reason:{failureDescription.reason}, Message:{failureDescription.message}".Log();
            
            var args = new CurrencyShopPurchaseActionArgs
            { 
                id = product.definition.id,
                isRealMoney = true,
                isSuccessful = false,
                error = $"{failureDescription.reason.ToString()}",
                errorMessage = failureDescription.message
            };

            PurchaseInProgress = false;
            
            OnPurchaseFinished?.Invoke(args);
        }

        #endregion
    }

    public struct ProductAdapter
    {
        public string id;
        public string priceString;
    }

    [Flags]
    public enum PurchasingMethod
    {
        None = 0, 
        NonRealMoney = 0xb1 << 0, 
        RealMoney = 0xb1 << 1, 
        Both = NonRealMoney | RealMoney
    }
}
