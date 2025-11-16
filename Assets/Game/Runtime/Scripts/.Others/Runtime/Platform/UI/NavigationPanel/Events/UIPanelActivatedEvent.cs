using Kumu.Kulitan.Events;

namespace Kumu.Kulitan.UI
{
    public class UIPanelActivatedEvent : Event<string>
    {
        public const string EVENT_NAME = "UIPanelActivatedEvent";

        public UIPanelActivatedEvent(string panelName) : base(EVENT_NAME)
        {
            PanelName = panelName;
        }

        public string PanelName { get; }
    }
}
