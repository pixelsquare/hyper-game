using Kumu.Kulitan.Events;

namespace Kumu.Kulitan.Common
{
    public class GameSceneLoadedEvent : Event<string>
    {
        public const string EVENT_NAME = "HangoutGameSceneLoadedEvent";
        
        public GameSceneLoadedEvent() : base(EVENT_NAME)
        {
        }
    }
}
