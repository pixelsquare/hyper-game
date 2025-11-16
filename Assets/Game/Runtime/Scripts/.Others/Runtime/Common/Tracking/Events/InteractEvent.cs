using Kumu.Kulitan.Events;

namespace Kumu.Kulitan.Tracking
{
    public class InteractEvent : Event<string>
    {
        public const string EVENT_ID = "InteractEvent";
        
        public InteractEvent(string playerId, int playerLevel, string hangoutId, string interactiveObjectId) : base(EVENT_ID)
        {
            PlayerId = playerId;
            PlayerLevel = playerLevel;
            HangoutId = hangoutId;
            InteractiveObjectId = interactiveObjectId;
        }
        
        public string PlayerId { get; }
        public int PlayerLevel { get; }
        public string HangoutId { get; }
        public string InteractiveObjectId { get; }
    }

    public interface IInteractHandle
    {
        public void OnInteractEvent(InteractEvent eventData);
    }
}
