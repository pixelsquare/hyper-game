using UnityEngine;
using Kumu.Kulitan.Backend;
using UnityEngine.UI;

namespace Kumu.Kulitan.Avatar
{
    public class AvatarShopCartBalanceChecker : MonoBehaviour
    {
        [SerializeField] private Button checkOutButton;
        private int coinCache;
        private int diamondCache;
        
        public void UpdateBalance()
        {
            var walletBalance = UserInventoryData.UserWallet.GetCurrenciesAsArray();            
            UpdateWalletBalance(walletBalance);
        }
        
        public void CheckSufficientBalance(int totalCoinCost, int totalDiamondCost)
        {
            var isCoinSufficient = coinCache >= totalCoinCost;
            var isDiamondSufficient = diamondCache >= totalDiamondCost;

            checkOutButton.interactable = isCoinSufficient && isDiamondSufficient;
        }

        private void UpdateWalletBalance(Currency[] walletBalance)
        {
            foreach (var currency in walletBalance)
            {
                if (currency.code == Currency.UBE_COI)
                {
                    coinCache = currency.amount;
                }
                
                if (currency.code == Currency.UBE_DIA)
                {
                    diamondCache = currency.amount;
                }
            }
        }
    }
}
