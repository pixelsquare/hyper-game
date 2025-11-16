namespace Kumu.Kulitan.Events
{
    public class VGGiftingEvent : Event<string>
    {
        public const string EVENT_ID = "VGGiftingEvent";

        public VGGiftingEvent(string senderPid, int senderPLvl, string receiverPid, int receiverPLvl, string hangoutId,
                              string sessionId, string vgId, int vgCost, string vgType) : base(EVENT_ID)
        {
            SenderPlayerId = senderPid;
            SenderPlayerLevel = senderPLvl;
            ReceiverPlayerId = receiverPid;
            ReceiverPlayerLevel = receiverPLvl;
            HangoutId = hangoutId;
            SessionId = sessionId;
            VgId = vgId;
            VgCost = vgCost;
            VgType = vgType;
        }

        public string SenderPlayerId { get; }
        public int SenderPlayerLevel { get; }
        public string ReceiverPlayerId { get; }
        public int ReceiverPlayerLevel { get; }
        public string HangoutId { get; }
        public string SessionId { get; }
        public string VgId { get; }
        public int VgCost { get; }
        public string VgType { get; }
    }

    public class VGTrayOpenEvent : Event<string>
    {
        public const string EVENT_ID = "VGTrayOpenEvent";

        public VGTrayOpenEvent(string playerId, int playerLevel, int currentCoinsBalance, string hangoutId, 
                                string sessionId, int maxVisitorsCount, int uniqueVisitorsCount, int duration) : base(EVENT_ID)
        {
            PlayerId = playerId;
            PlayerLevel = playerLevel;
            CurrentCoinBalance = currentCoinsBalance;
            HangoutId = hangoutId;
            SessionId = sessionId;
            MaxVisitorsCount = maxVisitorsCount;
            UniqueVisitorsCount = uniqueVisitorsCount;
            Duration = duration;
        }

        public string PlayerId { get; }
        public int PlayerLevel { get; }
        public int CurrentCoinBalance { get; }
        public string HangoutId { get; }
        public string SessionId { get; }
        public int MaxVisitorsCount { get; }
        public int UniqueVisitorsCount { get; }
        public int Duration { get; }
    }

    public interface IVGGiftingHandle
    {
        public void OnVGGiftingEvent(VGGiftingEvent eventData);
    }

    public interface IVGTrayOpenHandle
    {
        public void OnVGTrayOpenEvent(VGTrayOpenEvent eventData);
    }
}
