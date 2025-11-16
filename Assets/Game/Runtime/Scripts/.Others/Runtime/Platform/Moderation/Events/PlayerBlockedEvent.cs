namespace Kumu.Kulitan.Events
{
    public class PlayerBlockedEvent : Event<string>
    {
        public const string EVENT_NAME = "PlayerBlockedEvent";

        public PlayerBlockedEvent(string accountId, uint playerId, bool isBlocked) : base(EVENT_NAME)
        {
            AccountId = accountId;
            PlayerId = playerId;
            IsBlocked = isBlocked;
        }

        public string AccountId { get; }
        
        public uint PlayerId { get; }

        public bool IsBlocked { get; }
    }
}
