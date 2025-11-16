using Kumu.Kulitan.Events;

namespace Kumu.Kulitan.Multiplayer
{
    /// <summary>
    /// Event when server connection has been established.
    /// </summary>
    public class ConnectedToServerEvent : Event<string>
    { 
        public const string EVENT_NAME = "ConnectedToServerEvent";

        public ConnectedToServerEvent() : base(EVENT_NAME)
        {
        }
    }
}
