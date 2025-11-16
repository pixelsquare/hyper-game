using Kumu.Extensions;

namespace Kumu.Kulitan.Tracking
{
    public partial class DebugTracker : ISendMessageHandle
    {
        public void OnSendMessageEvent(SendMessageEvent eventData)
        {
            var message = $"<color=#{HEX}>Send Message: {eventData.PlayerId} " +
                    $"| {eventData.PlayerLevel} " +
                    $"| {eventData.HangoutId} " +
                    $"| {eventData.SessionId} " +
                    $"| {eventData.MaxVisitorsCount} " +
                    $"| {eventData.UniqueVisitorsCount}</color>";

            message.Log();
        }
    }
}
