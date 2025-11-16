using Kumu.Kulitan.Events;

namespace Kumu.Kulitan.UI
{
    public class MenuPanelChangedEvent : Event<string>
    { 
        public const string EVENT_NAME = "MenuPanelChangedEvent";

        private string openPanelName;

        public MenuPanelChangedEvent(string newOpenPanel) : base(EVENT_NAME)
        {
            this.openPanelName = newOpenPanel;
        }

        public string OpenPanelName => openPanelName;
    }
}
