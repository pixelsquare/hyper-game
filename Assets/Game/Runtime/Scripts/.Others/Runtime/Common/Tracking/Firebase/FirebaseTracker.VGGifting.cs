using Firebase.Analytics;
using Kumu.Kulitan.Events;

namespace Kumu.Kulitan.Tracking
{
    public partial class FirebaseTracker : IVGGiftingHandle
    {
        public void OnVGGiftingEvent(VGGiftingEvent eventData)
        {
            var parameters = new Parameter[]
            {
                new("sender_player_id", eventData.SenderPlayerId),
                new("sender_player_level", eventData.SenderPlayerLevel),
                new("receiver_player_id", eventData.ReceiverPlayerId),
                new("receiver_player_level", eventData.ReceiverPlayerLevel),
                new("hangout_id", eventData.HangoutId),
                new("session_id", eventData.SessionId),
                new("vg_id", eventData.VgId),
                new("vg_cost", eventData.VgCost),
                new("vg_type", eventData.VgType)
            };

            FirebaseAnalytics.LogEvent("virtual_gift_sent", parameters);
        }
    }

    public partial class FirebaseTracker : IVGTrayOpenHandle
    {
        public void OnVGTrayOpenEvent(VGTrayOpenEvent eventData)
        {
            var parameters = new Parameter[]
            {
                new("player_id", eventData.PlayerId),
                new("player_level", eventData.PlayerLevel),
                new("current_coins_balance", eventData.CurrentCoinBalance),
                new("hangout_id", eventData.HangoutId),
                new("session_id", eventData.SessionId),
                new("max_visitors_count", eventData.MaxVisitorsCount),
                new("unique_visitors_count", eventData.UniqueVisitorsCount),
                new("duration", eventData.Duration),
            };
            
            FirebaseAnalytics.LogEvent("virtual_gift_tray_opened", parameters);
        }
    }
}
