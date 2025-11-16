using Kumu.Kulitan.Events;

namespace Kumu.Kulitan.Tracking
{
    public class ItemPurchasedEvent : Event<string>
    {
        public const string EVENT_ID = "ItemPurchasedEvent";

        public ItemPurchasedEvent(string playerId, int playerLevel, string avatarItemId, int avatarItemDiamondPrice,
                                  int avatarItemKumuCoinPrice, int avatarItemCoinsPrice) : base(EVENT_ID)
        {
            PlayerId = playerId;
            PlayerLevel = playerLevel;
            AvatarItemId = avatarItemId;
            AvatarItemDiamondPrice = avatarItemDiamondPrice;
            AvatarItemKumuCoinPrice = avatarItemKumuCoinPrice;
            AvatarItemCoinsPrice = avatarItemCoinsPrice;
        }

        public string PlayerId { get; }

        public int PlayerLevel { get; }

        public string AvatarItemId { get; }

        public int AvatarItemDiamondPrice { get; }

        public int AvatarItemKumuCoinPrice { get; }

        public int AvatarItemCoinsPrice { get; }
    }

    public interface IItemPurchasedHandle
    {
        public void OnItemPurchasedEvent(ItemPurchasedEvent eventData);
    }
}
