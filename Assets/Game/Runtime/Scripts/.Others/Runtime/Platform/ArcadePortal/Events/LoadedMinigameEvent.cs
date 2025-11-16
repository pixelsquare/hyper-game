using Kumu.Kulitan.Events;

namespace Kumu.Kulitan.Multiplayer
{
    public class LoadedMinigameEvent : Event<string>
    { 
        public const string EVENT_NAME = "LoadedMinigameEvent";

        public LoadedMinigameEvent(LevelConfigScriptableObject levelConfig) : base(EVENT_NAME)
        {
            LevelConfig = levelConfig;
        }

        public LevelConfigScriptableObject LevelConfig { get; private set; }
    }
}
