using Kumu.Kulitan.Backend;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Hangout;
using Kumu.Kulitan.Multiplayer;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kumu.Kulitan.Lobby
{
    public class LobbySelectionView : ListViewElement<LobbyConfig>
    {
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private Button button;
        [SerializeField] private LobbyInfoView lobbyInfoView;
        [SerializeField] private GameObject playerHereIcon;

        private LobbyConfig data;
        private string lobbyName;

        public override void Refresh(LobbyConfig data)
        {
            this.data = data;

            label.text = data.name;

            lobbyName = LobbyUtil.WrapVersionLobbyId(data.id);

            button.onClick.AddListener(JoinLobby);

            var currentLobbyName = ConnectionManager.Client.CurrentLobby.Name;
            playerHereIcon.SetActive(currentLobbyName.Equals(lobbyName));
        }

        private void JoinLobby()
        {
            LoadingScreenManager.Instance.ShowHideLoadingOverlay(true);
            
            var lobby = new TypedLobby(lobbyName, LobbyType.Default);
            ConnectionManager.Client.OpJoinLobby(lobby);
            RoomConnectionDetails.Instance.myLobby = lobby;
            RoomConnectionDetails.Instance.lobbyLabel = data.name;

            lobbyInfoView.TryRefreshLabel();
        }
    }
}
