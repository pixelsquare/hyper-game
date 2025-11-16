using Kumu.Kulitan.Events;

namespace Kumu.Kulitan.Hangout
{
    public class SceneLoadingEvent : Event<string>
    {
        public const string EVENT_NAME = "SceneLoadingEvent";
        
        public SceneLoadingEvent() : base(EVENT_NAME)
        {
        }
    }
}
