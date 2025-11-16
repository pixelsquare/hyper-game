using Kumu.Kulitan.Events;

namespace Kumu.Kulitan.Hangout
{
    public class OnAudioSettingsChanged : Event<string>
    {
        public const string EVENT_NAME = "OnAudioSettingsChanged";

        public OnAudioSettingsChanged(AudioSourceHandler.AudioSourceType aType) : base(EVENT_NAME)
        {
            SourceType = aType;
        }

        public AudioSourceHandler.AudioSourceType SourceType { get; }
    }
}
