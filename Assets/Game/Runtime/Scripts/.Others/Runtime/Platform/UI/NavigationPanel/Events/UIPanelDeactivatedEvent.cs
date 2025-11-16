using Kumu.Kulitan.Events;

namespace Kumu.Kulitan.UI
{
    public class UIPanelDeactivatedEvent : Event<string>
    {
        public const string EVENT_NAME = "UIPanelDeactivatedEvent";

        public UIPanelDeactivatedEvent(string panelName) : base(EVENT_NAME)
        {
            PanelName = panelName;
        }
        
        public string PanelName { get; }
    }
}
