using Kumu.Kulitan.Avatar;

namespace Kumu.Kulitan.Backend
{
    public class GetWalletBalanceResult : ResultBase
    {
        public Currency[] walletBalance;
    }

    public class GetInventoryResult : ResultBase
    {
        public string[] ownedItemIds;
        public Currency[] walletBalance;
        public AvatarItemState[] equippedItems;
    }

    public class EquipItemsResult : ResultBase
    {
        public AvatarItemState[] equippedItems;
    }
}
