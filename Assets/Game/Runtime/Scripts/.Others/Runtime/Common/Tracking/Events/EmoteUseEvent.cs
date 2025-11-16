using Kumu.Kulitan.Events;

namespace Kumu.Kulitan.Tracking
{
    public class EmoteUseEvent : Event<string>
    {
        public const string EVENT_ID = "EmoteUseEvent";
        
        public EmoteUseEvent(string playerId, int playerLevel, string emoteId) : base(EVENT_ID)
        {
            PlayerId = playerId;
            PlayerLevel = playerLevel;
            EmoteId = emoteId;
        }
        
        public string PlayerId { get; }
        public int PlayerLevel { get; }
        public string EmoteId { get; }
    }

    public interface IEmoteUseHandle
    {
        public void OnEmoteUseEvent(EmoteUseEvent eventData);
    }
}
