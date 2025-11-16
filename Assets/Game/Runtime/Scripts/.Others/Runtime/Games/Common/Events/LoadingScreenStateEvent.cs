using Kumu.Kulitan.Events;

namespace Kumu.Kulitan.Common
{
    public class LoadingScreenStateEvent : Event<string>
    {
        public const string EVENT_ID = "LoadingScreenState";
        
        public enum LoadingScreenState { SHOWN, HIDDEN }

        public LoadingScreenStateEvent(LoadingScreenState state) : base(EVENT_ID)
        {
            State = state;
        }
        
        public LoadingScreenState State { get; }
    }
}
