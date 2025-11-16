using System;
using Kumu.Kulitan.Backend;

namespace Kumu.Kulitan.Avatar
{
    [Serializable]
    public struct AvatarItemData
    {
        public string itemId;
        public string itemName;
        public string itemBatch;
        public string itemCategory;
        public Currency cost;
        public int markUpDownCost;
    }
}
