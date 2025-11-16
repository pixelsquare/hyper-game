using Kumu.Kulitan.Events;

namespace Kumu.Kulitan.Tracking
{
    public class ItemSelectedEvent : Event<string>
    {
        public const string EVENT_ID = "ItemSelectedEvent";

        public ItemSelectedEvent(string playerId, int playerLevel, string avatarItemId, int avatarItemDiamondPrice,
                                 int avatarItemKumuCoinPrice, int avatarItemCoinsPrice, bool isOwned) : base(EVENT_ID)
        {
            PlayerId = playerId;
            PlayerLevel = playerLevel;
            AvatarItemId = avatarItemId;
            AvatarItemDiamondPrice = avatarItemDiamondPrice;
            AvatarItemKumuCoinPrice = avatarItemKumuCoinPrice;
            AvatarItemCoinsPrice = avatarItemCoinsPrice;
            IsOwned = isOwned;
        }

        public string PlayerId { get; }

        public int PlayerLevel { get; }

        public string AvatarItemId { get; }

        public int AvatarItemDiamondPrice { get; }

        public int AvatarItemKumuCoinPrice { get; }

        public int AvatarItemCoinsPrice { get; }

        public bool IsOwned { get; }
    }

    public interface IItemSelectedHandle
    {
        void OnItemSelectedEvent(ItemSelectedEvent eventData);
    }
}
