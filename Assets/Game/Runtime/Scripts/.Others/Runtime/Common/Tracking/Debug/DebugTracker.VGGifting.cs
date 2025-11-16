using Kumu.Extensions;
using Kumu.Kulitan.Events;

namespace Kumu.Kulitan.Tracking
{
    public partial class DebugTracker : IVGGiftingHandle
    {
        public void OnVGGiftingEvent(VGGiftingEvent eventData)
        {
            var logMessage = $"Tracking: Player {eventData.SenderPlayerId} sent gift to " +
                    $"{eventData.ReceiverPlayerId} on hangout [{eventData.HangoutId}]. " +
                    $"Gift [{eventData.VgId} | {eventData.VgType} | {eventData.VgCost}]";

            logMessage.Log();
        }
    }

    public partial class DebugTracker : IVGTrayOpenHandle
    {
        public void OnVGTrayOpenEvent(VGTrayOpenEvent eventData)
        {
            var logMessage = $"Tracking: Player {eventData.PlayerId} opened virtual gift tray and was closed after {eventData.Duration} secs. " +
                             $"Coin balance: {eventData.CurrentCoinBalance} | unique visitors: {eventData.UniqueVisitorsCount} " +
                             $"| max concurrent visitors: {eventData.MaxVisitorsCount}";
            logMessage.Log();
        }
    }
}
