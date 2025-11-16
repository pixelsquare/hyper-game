using Kumu.Kulitan.Events;

namespace Kumu.Kulitan.Tracking
{
    public class ScreenViewEvent : Event<string>
    {
        public const string EVENT_ID = "ScreenViewEvent";
        
        public ScreenViewEvent(string screenId) : base(EVENT_ID)
        {
            ScreenId = screenId;
        }
        
        public string ScreenId { get; }
    }

    public interface IScreenViewHandle
    {
        public void OnScreenView(ScreenViewEvent eventData);
    }
}
