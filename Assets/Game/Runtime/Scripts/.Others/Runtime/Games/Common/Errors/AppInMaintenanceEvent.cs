using Kumu.Kulitan.Events;

namespace Kumu.Kulitan.Common
{
    public class AppInMaintenanceEvent : Event<string>
    {
        public const string EVENT_NAME = "AppInMaintenanceEvent";

        public AppInMaintenanceEvent() : base(EVENT_NAME)
        {
        }
    }
}
