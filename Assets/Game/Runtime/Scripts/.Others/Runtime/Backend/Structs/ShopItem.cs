using System;

namespace Kumu.Kulitan.Backend
{
    [Serializable]
    public struct ShopItem
    {
        public string itemId;
        public Currency cost;
        public int markUpDownCost;
    }
}
