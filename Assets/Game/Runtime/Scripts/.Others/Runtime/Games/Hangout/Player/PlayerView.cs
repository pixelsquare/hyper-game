using Kumu.Kulitan.Common;
using Kumu.Kulitan.Multiplayer;
using Quantum;
using UnityEngine;

namespace Kumu.Kulitan.Hangout
{
    public class PlayerView : MonoBehaviour
    {
        [SerializeField] private EntityView entityView;
        [SerializeField] private Transform modelTransform;

        public PlayerRef PlayerRef { get; private set; }
        public Transform ModelTransform => modelTransform;
        public uint PlayerId { get; private set; }

        public void Initialize()
        {
            var f = QuantumRunner.Default.Game.Frames.Verified;
            PlayerRef = f.Get<HangoutPlayer>(entityView.EntityRef).player;
            var isLocal = QuantumRunner.Default.Game.PlayerIsLocal(PlayerRef);
            var playerData = f.GetPlayerData(PlayerRef);
            PlayerId = playerData.playerId;
            GlobalNotifier.Instance.Trigger(new QuantumPlayerJoinedEvent(isLocal, transform, modelTransform,
                    entityView.EntityRef, PlayerRef, playerData.nickname, playerData.username, playerData.playerId, playerData.accountId));
        }

        public void DeInitialize()
        {
            var f = QuantumRunner.Default.Game.Frames.Verified;
            var playerData = f.GetPlayerData(PlayerRef);
            GlobalNotifier.Instance.Trigger(new QuantumPlayerRemovedEvent(transform, entityView.EntityRef, PlayerRef, 
                                            playerData.username, playerData.accountId));
        }
    }
}
