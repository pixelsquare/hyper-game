using Kumu.Kulitan.Events;

namespace Kumu.Kulitan.UI
{
    public class OnActivateMenuPanel : Event<string>
    { 
        public const string EVENT_NAME = "OnActivateMenuPanel";

        public OnActivateMenuPanel() : base(EVENT_NAME)
        {
        }
    }
}
