using System.Threading.Tasks;
using Kumu.Kulitan.Backend;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Services.Core;
using UnityEngine.Assertions;

namespace Kumu.Kulitan.Common
{
    public class CurrencyShopTest : MonoBehaviour
    {
        [SerializeField] private string productId;

        [SerializeField] private Button initializeButton;
        
        [SerializeField] private Button realMoneyPurchaseButton;

        [SerializeField] private Button fetchCostDataButton;

        [SerializeField] private Button initializeUgsButton;
        
        [SerializeField] private Button nonRealMoneyPurchaseButton;
        
        [SerializeField] private TMP_InputField textBox;

        private CurrencyShopController CSController => CurrencyShopController.Instance;

        private CurrencyShopProductCostData[] costData;

        public void BuyCoins1k(bool useRealMoney)
        {
            LogLine($"Processing purchase. Product:{productId} UseRealMoney:{useRealMoney.ToString()}");
            CSController.PurchaseCurrencyShopItem(CSController.FindItemWithId(productId, useRealMoney));
        }

        private void InitUgs()
        {
            InitUgsAsync();

            async Task InitUgsAsync()
            {
                LogLine("Initializing Unity gaming services...");
                await UnityServices.InitializeAsync(new InitializationOptions()); // uses default "production" UGS environment
                LogLine("Finished initializing Unity Gaming Services!");
            }
        }

        private void FetchDiamondPrices()
        {
            FetchDiamondPricesAsync();
            
            async Task FetchDiamondPricesAsync()
            {
                LogLine("Fetching diamond prices for currency shop items from the backend...");
                var result = await Services.CurrencyShopService.GetCurrencyShopProductsAsync(new GetCurrencyShopProductsRequest());
                costData = result.Result.items;
                LogLine("Finished fetching diamond prices for currency shop items from the backend!");
            }
        }

        private void InitCSController()
        {
            Assert.IsNotNull(costData);
            CSController.InitializeAndSetNonRealMoneyCosts(costData);
        }

        private void LogLine(string message)
        {
            textBox.text += $"{message}\n \n";
        }

        private void HandleOnPurchaseFinishedEvent(CurrencyShopPurchaseActionArgs args)
        {
            if (args.isSuccessful)
            {
                LogLine($"<color=#005500>Purchase succeeded! Product:{args.id} UseRealMoney:{args.isRealMoney.ToString()}</color>");
            }
            else
            {
                LogLine($"<color=Red>Purchase failed! Product:{args.id} UseRealMoney:{args.isRealMoney.ToString()}</color>");
            }
        }

        private void HandleOnInitializeEvent(bool isInitialized)
        {
            if (isInitialized)
            {
                LogLine("Init successful!");
            }
            else
            {
                LogLine("Init failed!");
            }
        }

        private void Update()
        {
            var isUnityServicesInitialized = UnityServices.State == ServicesInitializationState.Initialized;
            var isCostDataReady = costData != null;
            initializeUgsButton.interactable = !isUnityServicesInitialized;
            fetchCostDataButton.interactable = isUnityServicesInitialized && !isCostDataReady;
            initializeButton.interactable = isUnityServicesInitialized && isCostDataReady && !CSController.IsInitialized;
            realMoneyPurchaseButton.interactable = !CSController.PurchaseInProgress && CSController.IsInitialized;
            nonRealMoneyPurchaseButton.interactable = !CSController.PurchaseInProgress && CSController.IsInitialized;;
        }

        private void Start()
        {
            // Clear text box
            textBox.text = ""; // clear;

#if !UBE_RELEASE
            // Enable SRDebugger
            SRDebug.Init();
#endif
            
            // Assign services
            Services.CurrencyShopService = MockedServiceMono.CreateNewInstance<MockedCurrencyShopServiceMono>();;
            
            initializeButton.onClick.AddListener(InitCSController);
            initializeUgsButton.onClick.AddListener(InitUgs);
            fetchCostDataButton.onClick.AddListener(FetchDiamondPrices);
            realMoneyPurchaseButton.onClick.AddListener(() => BuyCoins1k(true));
            nonRealMoneyPurchaseButton.onClick.AddListener(() => BuyCoins1k(false));
        }

        private void OnEnable()
        {
            CurrencyShopController.OnInitializeFinished += HandleOnInitializeEvent;
            CurrencyShopController.OnPurchaseFinished += HandleOnPurchaseFinishedEvent;
        }

        private void OnDisable()
        {
            CurrencyShopController.OnInitializeFinished -= HandleOnInitializeEvent;
            CurrencyShopController.OnPurchaseFinished -= HandleOnPurchaseFinishedEvent;
        }
    }
}
