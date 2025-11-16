using Kumu.Kulitan.Events;
using Quantum;
using UnityEngine;

namespace Kumu.Kulitan.Multiplayer
{
    public class QuantumPlayerJoinedEvent : Event<string>
    {
        public const string EVENT_NAME = "QuantumPlayerJoinedEvent";
                
        public QuantumPlayerJoinedEvent(bool isLocal, Transform playerTransform, Transform modelTransform, EntityRef playerEntity, PlayerRef playerRef, 
            string nickname, string username, uint playerId, string accountId) : base(EVENT_NAME)
        {
            IsLocal = isLocal;
            PlayerTransform = playerTransform;
            ModelTransform = modelTransform;
            Nickname = nickname;
            Username = username;
            PlayerEntity = playerEntity;
            PlayerRef = playerRef;
            PlayerId = playerId;
            AccountId = accountId;
        }

        public bool IsLocal { get; }
        public Transform PlayerTransform { get; }
        public Transform ModelTransform { get; }
        public string Nickname { get; }
        public string Username { get; }
        public EntityRef PlayerEntity { get; }
        public PlayerRef PlayerRef { get; }
        public uint PlayerId { get; }
        public string AccountId { get; }
    }
}
