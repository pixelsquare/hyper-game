using Kumu.Kulitan.Events;

namespace Kumu.Kulitan.Multiplayer
{
    public class LoadMinigameEvent : Event<string>
    { 
        public const string EVENT_NAME = "LoadMinigameBtnClickedEvent";

        public LoadMinigameEvent(LevelConfigScriptableObject levelConfig) : base(EVENT_NAME)
        {
            LevelConfig = levelConfig;
        }

        public LevelConfigScriptableObject LevelConfig { get; private set; }
    }
}
