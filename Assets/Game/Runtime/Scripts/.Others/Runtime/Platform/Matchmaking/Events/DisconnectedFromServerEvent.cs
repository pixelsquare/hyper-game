using Kumu.Kulitan.Events;

namespace Kumu.Kulitan.Multiplayer
{
    /// <summary>
    /// Event when server has been disconnected.
    /// </summary>
    public class DisconnectedFromServerEvent : Event<string>
    { 
        public const string EVENT_NAME = "DisconnectedFromServerEvent";

        public DisconnectedFromServerEvent() : base(EVENT_NAME)
        {
        }
    }
}
