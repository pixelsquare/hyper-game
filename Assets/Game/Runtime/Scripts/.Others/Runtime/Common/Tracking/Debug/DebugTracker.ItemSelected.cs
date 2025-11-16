using Kumu.Extensions;

namespace Kumu.Kulitan.Tracking
{
    public partial class DebugTracker : IItemSelectedHandle
    {
        public void OnItemSelectedEvent(ItemSelectedEvent eventData)
        {
            var message = $"<color=#{HEX}>Item Selected: {eventData.PlayerId} " +
                    $"| {eventData.PlayerLevel} " +
                    $"| {eventData.AvatarItemId} " +
                    $"| {eventData.AvatarItemDiamondPrice} " +
                    $"| {eventData.AvatarItemKumuCoinPrice} " +
                    $"| {eventData.AvatarItemCoinsPrice} " +
                    $"| {eventData.IsOwned}</color>";

            message.Log();
        }
    }
}
