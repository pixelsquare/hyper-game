using System;
using Kumu.Kulitan.Backend;

namespace Kumu.Kulitan.Common
{
    [Serializable]
    public abstract class CurrencyShopItem
    {
        private CurrencyShopItemDefinition itemDefinition;
        
        public abstract bool IsRealMoney { get; }
        
        public abstract string PriceString { get; }
        
        public CurrencyShopItemDefinition ItemDefinition => itemDefinition;

        protected CurrencyShopItem(CurrencyShopItemDefinition itemDefinition)
        {
            this.itemDefinition = itemDefinition;
        }

        public override string ToString()
        {
            return $"ProductId:{itemDefinition.ProductId} IsRealMoney:{IsRealMoney.ToString()} PriceString:{PriceString}";
        }
    }

    [Serializable]
    public class RealMoneyCurrencyShopItem : CurrencyShopItem
    {
        private string realMoneyPrice;
        
        public override bool IsRealMoney => true;

        public override string PriceString => realMoneyPrice;

        public RealMoneyCurrencyShopItem(CurrencyShopItemDefinition itemDefinition, string realMoneyPrice) : base(itemDefinition)
        {
            this.realMoneyPrice = realMoneyPrice;
        }
    }
    
    [Serializable]
    public class NonRealMoneyCurrencyShopItem : CurrencyShopItem
    {
        private Currency nonRealMoneyPrice;
        
        public override bool IsRealMoney => false;

        public override string PriceString => nonRealMoneyPrice.ToString().Replace(" ", "");

        public NonRealMoneyCurrencyShopItem(CurrencyShopItemDefinition itemDefinition, Currency nonRealMoneyPrice) : base(itemDefinition)
        {
            this.nonRealMoneyPrice = nonRealMoneyPrice;
        }
    }
}
