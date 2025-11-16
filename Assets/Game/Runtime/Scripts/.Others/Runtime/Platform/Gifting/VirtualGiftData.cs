using System;
using Kumu.Kulitan.Backend;

namespace Kumu.Kulitan.Gifting
{
    [Serializable]
    public struct VirtualGiftData
    {
        public string giftId;
        public string giftName;
        public VGType giftType;
        public Currency cost; // base price of the item
        public int priority;

        public enum VGType
        {
            Individual,
            Shared,
            Collectible,
            Interactable,
            Area,
        }
    }

    public struct CostingInfo
    {
        private int baseAmount;
        private int finalAmount;
        private string currencyCode;

        public CostingInfo(int baseAmount, int finalAmount, string currencyCode)
        {
            this.baseAmount = baseAmount;
            this.finalAmount = finalAmount;
            this.currencyCode = currencyCode;
        }

        /// <summary>
        /// Base price of the item, with no mark up/down adjustments.
        /// </summary>
        public Currency Base => new Currency { amount = baseAmount, code = currencyCode };

        /// <summary>
        /// Price of the item after mark up/down has been applied.
        /// </summary>
        public Currency Final => new Currency { amount = finalAmount, code = currencyCode };

        /// <summary>
        /// <para>Final price / base price.</para>
        /// <para>Less than 1.0f if marked down. Greater than 1.0f if marked up.</para>
        /// <para>-1.0f if base price is 0.</para>
        /// </summary>
        public float MarkUpDownRatio => baseAmount == 0 ? -1.0f : finalAmount / (float)baseAmount;

        /// <summary>
        /// <para>Mark up/down expressed as a percentage.</para>
        /// <para>(e.g. -0.3f is a 30% discount on the base price).</para>
        /// <para>0f if base price is 0.</para>
        /// </summary>
        public float MarkUpDownPercentage => baseAmount == 0 ? 0f : MarkUpDownRatio - 1f;
    }
}
