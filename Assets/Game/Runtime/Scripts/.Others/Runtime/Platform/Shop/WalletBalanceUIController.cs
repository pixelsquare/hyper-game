using Kumu.Kulitan.Avatar;
using UnityEngine;
using Kumu.Kulitan.Backend;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;

namespace Kumu.Kulitan.UI
{
    public class WalletBalanceUIController : MonoBehaviour
    {
        [SerializeField] private CombinedCurrencyIndicator currencyIndicator;
        private Slot<string> eventSlot;

        private void UpdateWalletBalanceDisplay(Currency[] walletBalance)
        {
            var coinValue = 0;
            var diamondValue = 0;
            
            foreach (var currency in walletBalance)
            {
                switch (currency.code)
                {
                    case Currency.UBE_COI:
                        coinValue = currency.amount;
                        continue;
                    case Currency.UBE_DIA:
                        diamondValue = currency.amount;
                        break;
                }
            }
            
            currencyIndicator.UpdateValue(coinValue, diamondValue);
        }

        private void OnWalletUpdated(IEvent<string> callback)
        {
            var eventCallback = (UserWalletUpdatedEvent)callback;
            UpdateWalletBalanceDisplay(eventCallback.Currencies);
        }

        private void Start()
        {
            var wallet = UserInventoryData.UserWallet;
            UpdateWalletBalanceDisplay(wallet.GetCurrenciesAsArray());
        }
        
        private void OnEnable()
        {
            eventSlot = new Slot<string>(GlobalNotifier.Instance);
            eventSlot.SubscribeOn(UserWalletUpdatedEvent.EVENT_NAME, OnWalletUpdated);
        }

        private void OnDisable()
        {
            eventSlot.Dispose();
        }
    }
}
