using Kumu.Extensions;

namespace Kumu.Kulitan.Tracking
{
    public partial class DebugTracker : IItemPurchasedHandle
    {
        public void OnItemPurchasedEvent(ItemPurchasedEvent eventData)
        {
            var message = $"<color=#{HEX}>Item Purchased: {eventData.PlayerId} " +
                    $"| {eventData.PlayerLevel} " +
                    $"| {eventData.AvatarItemId} " +
                    $"| {eventData.AvatarItemDiamondPrice} " +
                    $"| {eventData.AvatarItemKumuCoinPrice} " +
                    $"| {eventData.AvatarItemCoinsPrice}</color>";

            message.Log();
        }
    }
}
