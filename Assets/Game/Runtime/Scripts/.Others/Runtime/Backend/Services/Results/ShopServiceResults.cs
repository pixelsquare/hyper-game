using Kumu.Kulitan.Avatar;

namespace Kumu.Kulitan.Backend
{
    public class BuyShopItemsResult : ResultBase
    {
        public bool fullySuccessful;
        public AvatarItemState[] boughtItems;
        public AvatarItemState[] equippedItems;
        public Currency[] updatedBalance;
    }

    public class GetShopItemsCostResult : ResultBase
    {
        public ShopItem[] itemsCost;
    }
}
