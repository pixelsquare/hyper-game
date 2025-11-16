using Kumu.Extensions;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Hangout;
using Kumu.Kulitan.Multiplayer;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

namespace Kumu.Kulitan.UI
{
    [RequireComponent(typeof(Button))]
    public class ReportHangoutButton : MonoBehaviour
    {
        [SerializeField] private Button button;

        public void ShowReportHangoutPopup()
        {
            if (!TryGetHostIds(out var accountId, out var playerId))
            {
                "Failed to show report popup.".LogError();
                return;
            }

            var popup = (ReportPopup)PopupManager.Instance.CreatePopup(PopupManager.PopupType.ReportPopup, 0);
            popup.Initialize(ReportType.Hangout, accountId, playerId);
            popup.OpenPopup();
        }

        private bool TryGetHostIds(out string accountId, out uint playerId)
        {
            var currentRoom = ConnectionManager.Client.CurrentRoom;
            var customProps = currentRoom.CustomProperties;

            if (!customProps.TryGetValue(Constants.HOST_DETAILS_PROP_KEY, out var hostDetailsObj)
             && string.IsNullOrEmpty(hostDetailsObj.ToString()))
            {
                accountId = "";
                playerId = 0;
                "Missing host details prop key.".LogError();
                return false;
            }

            var hostDetails = JsonConvert.DeserializeObject<PlayerDetails>(hostDetailsObj.ToString());
            accountId = hostDetails.accountId;
            playerId = hostDetails.playerId;
            return true;
        }

        private void HandleButtonClicked()
        {
            ShowReportHangoutPopup();
        }

        private void Awake()
        {
            if (button == null)
            {
                button = GetComponent<Button>();
            }
        }

        private void OnEnable()
        {
            button.onClick.AddListener(HandleButtonClicked);
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(HandleButtonClicked);
        }

        private void Start()
        {
            button.interactable = !ConnectionManager.Client.LocalPlayer.IsMasterClient;
        }
    }
}
