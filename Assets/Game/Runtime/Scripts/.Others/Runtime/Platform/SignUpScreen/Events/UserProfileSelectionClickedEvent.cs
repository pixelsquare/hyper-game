using Kumu.Kulitan.Events;

namespace Kumu.Kulitan.UI
{
    public class UserProfileSelectionClickedEvent : Event<string>
    { 
        public const string EVENT_NAME = "UserProfileSelectionClickedEvent";

        private UserProfileSelection selection;
        
        public UserProfileSelectionClickedEvent(UserProfileSelection selection) : base(EVENT_NAME)
        {
            this.selection = selection;
        }

        public UserProfileSelection Selection => selection;
    }
}
