using Kumu.Kulitan.Events;
using Quantum;
using UnityEngine;

namespace Kumu.Kulitan.Multiplayer
{
    public class QuantumPlayerRemovedEvent : Event<string>
    {
        public const string EVENT_NAME = "QuantumPlayerRemovedEvent";
        
        public QuantumPlayerRemovedEvent(Transform playerTransform, EntityRef entityRef, PlayerRef playerRef, string username,
                                        string accountId) : base(EVENT_NAME)
        {
            PlayerTransform = playerTransform;
            EntityRef = entityRef;
            PlayerRef = playerRef;
            Username = username;
            AccountId = accountId;
        }
        
        public Transform PlayerTransform { get; }
        public EntityRef EntityRef { get; }
        public PlayerRef PlayerRef { get; }
        public string Username { get; }
        public string AccountId { get; }
    }
}
