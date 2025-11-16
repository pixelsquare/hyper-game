using Kumu.Kulitan.Common;
using Quantum;
using Kumu.Kulitan.Multiplayer;
using Kumu.Kulitan.Tracking;
using Kumu.Kulitan.UI;
using Kumu.Kulitan.Backend;

namespace Kumu.Kulitan.Hangout
{
    public class HangoutQuantumCallbackHandler : QuantumCallbacks
    {
        public override void OnGameStart(QuantumGame game)
        {
            if (game.Session.IsPaused)
            {
                return;
            }

            SendLocalPlayerData(game);
            
            GlobalNotifier.Instance.Trigger(new UserJourneyEvent(UserJourneyEvent.Checkpoint.JoinHangout));
            GlobalNotifier.Instance.Trigger(new ScreenViewEvent("hangout_room"));
        }

        public override void OnGameResync(QuantumGame game)
        {
            SendLocalPlayerData(game);
        }

        private void SendLocalPlayerData(QuantumGame game)
        {
            var config = ConnectionManager.Instance.CurrentLevelConfig;
            foreach (var lp in game.GetLocalPlayers())
            {
                game.SendPlayerData(lp, new RuntimePlayer
                {
                    playerRef = config.PlayerPrototype,
                    nickname = ConnectionManager.Client.NickName,
                    username = UserProfileLocalDataManager.Instance.GetLocalUserProfile().userName.ToString(),
                    isCustomSpawnPos = config.CustomPlayerInitialData.HasCustomInitTransform,
                    customSpawnPos = config.CustomPlayerInitialData.CustomInitPosition.ToFPVector3(),
                    playerId = UserProfileLocalDataManager.Instance.GetLocalUserProfile().playerId,
                    accountId = UserProfileLocalDataManager.Instance.GetLocalUserProfile().accountId,
                    isKumuAccountLinked = UserProfileLocalDataManager.Instance.GetLocalUserProfile().hasLinkedKumuAccount
                });
            }
            
            PopupManager.Instance.CloseAllPopups();
        }      
    }
}
