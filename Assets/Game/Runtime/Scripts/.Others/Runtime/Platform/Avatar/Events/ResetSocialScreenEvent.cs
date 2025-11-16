using Kumu.Kulitan.Events;

namespace Kumu.Kulitan.Multiplayer
{
    public class ResetSocialScreenEvent : Event<string>
    {
        public const string EVENT_NAME = "ResetSocialScreenEvent";
        
        public ResetSocialScreenEvent() : base(EVENT_NAME)
        {
        }
    }
}
