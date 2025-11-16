using System.Threading.Tasks;
using Kumu.Extensions;
using Kumu.Kulitan.Backend;
using Unity.Services.Core;
using UnityEditor;
using UnityEngine;

namespace Kumu.Kulitan.Common
{
    [CustomEditor(typeof(CurrencyShopController))]
    public class CurrencyShopControllerEditor : Editor
    {
        private CurrencyShopItemDefinition itemDefinitionToPurchase;
        private CurrencyShopProductCostData[] costData;
        private bool isRealMoney;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var currencyShop = target as CurrencyShopController;

            using (new EditorGUI.DisabledScope(!Application.isPlaying))
            {
                // Purchasing input
                itemDefinitionToPurchase = (CurrencyShopItemDefinition)EditorGUILayout.ObjectField("CurrencyShopItem", itemDefinitionToPurchase, typeof(CurrencyShopItemDefinition), false);
                isRealMoney = EditorGUILayout.Toggle("IsRealMoneyPurchase", isRealMoney);
                
                // Initialize Unity Gaming Services button
                var isNotUninitialized = Application.isPlaying && UnityServices.State != ServicesInitializationState.Uninitialized; // Application.isPlaying is needed or otherwise error is thrown while not in play mode
                using (new EditorGUI.DisabledScope(isNotUninitialized))
                {
                    if (GUILayout.Button("Initialize Unity Gaming Services"))
                    {
                        UnityServices.InitializeAsync();
                    }
                }
                
                // Fetch non-real money prices
                var isFetched = costData != null;
                using (new EditorGUI.DisabledScope(isFetched))
                {
                    if (GUILayout.Button("Fetch non-real money cost data"))
                    {
                        FetchCostData();
                    }
                }

                // Initialize CurrencyShopController button
                var cannotInitialize = currencyShop == null || currencyShop.IsInitialized || (Application.isPlaying && UnityServices.State != ServicesInitializationState.Initialized); 
                using (new EditorGUI.DisabledScope(cannotInitialize))
                {
                    if (GUILayout.Button("Initialize CurrencyShopController"))
                    {
                        currencyShop.InitializeAndSetNonRealMoneyCosts(costData);
                    }
                }

                var cannotPurchase = currencyShop == null || !currencyShop.IsInitialized || itemDefinitionToPurchase == null;
                // Purchasing button
                using (new EditorGUI.DisabledScope(cannotPurchase))
                {
                    if (GUILayout.Button("Purchase coins1k"))
                    {
                        var item = currencyShop.FindItemWithId(itemDefinitionToPurchase.ProductId, isRealMoney);
                        if (item == null)
                        {
                            $"Could not find item with id:{itemDefinitionToPurchase.ProductId}, isRealMoney:{isRealMoney}".LogError();
                            return;
                        }
                    
                        currencyShop.PurchaseCurrencyShopItem(item);
                    }
                }
            }
        }

        private async Task FetchCostData()
        {
            var result = await Services.CurrencyShopService.GetCurrencyShopProductsAsync(new GetCurrencyShopProductsRequest());

            costData = result.Result.items;
        }
    }
}
