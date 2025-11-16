using Kumu.Kulitan.Events;

namespace Kumu.Kulitan.Tracking
{
    public class SendMessageEvent : Event<string>
    {
        public const string EVENT_ID = "SendMessageEvent";

        public SendMessageEvent(string playerId, int playerLevel, string hangoutId,
                                string sessionId, int maxVisitorsCount, int uniqueVisitorsCount) : base(EVENT_ID)
        {
            PlayerId = playerId;
            PlayerLevel = playerLevel;
            HangoutId = hangoutId;
            SessionId = sessionId;
            MaxVisitorsCount = maxVisitorsCount;
            UniqueVisitorsCount = uniqueVisitorsCount;
        }

        public string PlayerId { get; }

        public int PlayerLevel { get; }

        public string HangoutId { get; }

        public string SessionId { get; }

        public int MaxVisitorsCount { get; }

        public int UniqueVisitorsCount { get; }
    }

    public interface ISendMessageHandle
    {
        void OnSendMessageEvent(SendMessageEvent eventData);
    }
}
