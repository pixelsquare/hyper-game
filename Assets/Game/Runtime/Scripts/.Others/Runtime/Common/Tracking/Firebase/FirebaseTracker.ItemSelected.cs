using Firebase.Analytics;

namespace Kumu.Kulitan.Tracking
{
    public partial class FirebaseTracker : IItemSelectedHandle
    {
        public void OnItemSelectedEvent(ItemSelectedEvent eventData)
        {
            var parameters = new Parameter[]
            {
                new("player_id", eventData.PlayerId),
                new("player_level", eventData.PlayerLevel),
                new("avatar_item_id", eventData.AvatarItemId),
                new("avatar_item_diamond_price", eventData.AvatarItemDiamondPrice),
                new("avatar_item_kumu_coins_price", eventData.AvatarItemKumuCoinPrice),
                new("avatar_item_coins_price", eventData.AvatarItemCoinsPrice),
                new("is_owned", eventData.IsOwned.ToString())
            };

            FirebaseAnalytics.LogEvent("avatar_item_selected", parameters);
        }
    }
}
