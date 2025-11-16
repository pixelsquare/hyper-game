using System;
using Kumu.Kulitan.Avatar;

namespace Kumu.Kulitan.Backend
{
    [Serializable]
    public class GetWalletBalanceRequest : RequestCommon { }

    [Serializable]
    public class GetInventoryRequest : RequestCommon { }

    [Serializable]
    public class EquipItemsRequest : RequestCommon
    {
        public AvatarItemState[] equippedItems;
    }
}
