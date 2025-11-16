using Kumu.Kulitan.Events;

namespace Kumu.Kulitan.UI
{
    public class OnScenePopupClosedEvent : Event<string>
    {
        public const string EVENT_NAME = "OnScenePopupClosedEvent";
        
        public OnScenePopupClosedEvent(string scenePopupName) : base(EVENT_NAME)
        {
            ScenePopupName = scenePopupName;
        }
        
        public string ScenePopupName { get; }
    }
}
