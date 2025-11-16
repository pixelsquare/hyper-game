using System;
using Kumu.Kulitan.Avatar;

namespace Kumu.Kulitan.Backend
{
    [Serializable]
    public class BuyShopItemsRequest : RequestCommon
    {
        public AvatarItemState[] equippedAndItemsToBuy;
        public Currency[] expectedCost;
    }

    [Serializable]
    public class GetShopItemCostsRequest : RequestCommon { }
}
