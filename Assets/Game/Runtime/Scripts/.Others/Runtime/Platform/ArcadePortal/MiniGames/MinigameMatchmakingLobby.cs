using UnityEngine;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections;
using Kumu.Extensions;
using Kumu.Kulitan.UI;
using Kumu.Kulitan.Common;

namespace Kumu.Kulitan.Multiplayer
{
    public class MinigameMatchmakingLobby : MonoBehaviour, IInRoomCallbacks
    {
        private const int START_DELAY = 3;

        [SerializeField] private int matchmakingCounter = 10;
        [Header("Popup Messages")]
        [SerializeField] private string findingMatchMsg = "FINDING A MATCH...";
        [SerializeField] private string gameStartingMsg = "GAME STARTING...";
        [SerializeField] private string warningMsg = "We cannot find other players right now, you may cancel the search or continue waiting.";

        private MatchmakingPopup popup;
        private Coroutine countdownCoroutine;

        public void HidePopup()
        {
            if (popup != null)
            {
                popup.Close();
                popup = null;
            }
        }

        private void UpdatePlayerList()
        {
            var players = ConnectionManager.Client.CurrentRoom.Players;
            var playerCountToStart = ConnectionManager.Instance.CurrentLevelConfig.Config.maxPlayers;

            if(popup != null)
            {
                popup.GeneratePlayerNames(players.Values);
            }

            if (players.Count >= playerCountToStart)
            {
                SetPopupToGameStart();
                EndCountdown();
                StartCoroutine(StartDelayTimer());
            }
        }

        private void BeginCountdown()
        {
            popup = (MatchmakingPopup)PopupManager.Instance.ShowMatchmakingPopup(() =>
            {
                EndCountdown();
                GlobalNotifier.Instance.Trigger(new LoadHangoutEvent(true));
                popup = null;
                gameObject.SetActive(false);
            });
            popup.Initialize(findingMatchMsg, matchmakingCounter, "Cancel");
            countdownCoroutine = StartCoroutine(CountdownTimer());
            return;          
        }

        private void EndCountdown()
        {
            if (countdownCoroutine != null)
            {
                StopCoroutine(countdownCoroutine);
                countdownCoroutine = null;
            }
        }

        private void OnCountdownEnd()
        {
            popup.ShowHideCloseButton(true);
            popup.ShowHideWarningText(true, warningMsg);
        }

        private void SetPopupToGameStart()
        {
            popup.SetDetails(gameStartingMsg, 0, "Cancel");
            popup.ShowHideCloseButton(false);
            popup.ShowHideCountdownText(false);
            popup.ShowHideWarningText(false);
        }

        private IEnumerator CountdownTimer()
        {
            int counter = matchmakingCounter;
            while (counter > 0)
            {
                yield return new WaitForSeconds(1);
                counter--;
                popup.UpdateTimer(counter);
            }

            popup.ShowHideCountdownText(false);
            OnCountdownEnd();
        }

        private IEnumerator StartDelayTimer()
        {
            yield return new WaitForSeconds(START_DELAY);
            SendStartGameEvent();
        }

        private void SendStartGameEvent()
        {
            $"IsOpen = {ConnectionManager.Client.CurrentRoom.IsOpen}".Log();
            if (ConnectionManager.Client != null && ConnectionManager.Client.InRoom &&
                ConnectionManager.Client.LocalPlayer.IsMasterClient && ConnectionManager.Client.CurrentRoom.IsOpen)
            {
                if (!ConnectionManager.Client.OpRaiseEvent((byte)ConnectionManager.PhotonEventCode.StartGame, null,
                    new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable))
                {
                    "Failed to send start game event".LogError();
                }
                else
                {
                    "Start game event sent".Log();
                }
            }
        }

        #region IInRoomCallbacks
        public void OnMasterClientSwitched(Player newMasterClient)
        {
            UpdatePlayerList();
        }

        public void OnPlayerEnteredRoom(Player newPlayer)
        {
            $"{newPlayer.NickName} entered the minigame room".Log();
            UpdatePlayerList();
        }

        public void OnPlayerLeftRoom(Player otherPlayer)
        {
            $"{otherPlayer.NickName} left the minigame room".Log();
            UpdatePlayerList();
        }

        public void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
        {
        }

        public void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
        {
        }
        #endregion

        private void OnDisconnectedFromServerByClient()
        {
            EndCountdown();
            HidePopup();
        }

        private void OnEnable()
        {
            ConnectionManager.Client.AddCallbackTarget(this);
            ConnectionManager.OnDisconnectedFromServerByClient += OnDisconnectedFromServerByClient;

            BeginCountdown();
            UpdatePlayerList();
        }

        private void OnDisable()
        {
            ConnectionManager.Client.RemoveCallbackTarget(this);
            ConnectionManager.OnDisconnectedFromServerByClient -= OnDisconnectedFromServerByClient;
        }
    }
}
