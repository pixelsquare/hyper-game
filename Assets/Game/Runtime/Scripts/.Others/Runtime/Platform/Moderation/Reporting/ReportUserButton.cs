using UnityEngine;
using UnityEngine.UI;

namespace Kumu.Kulitan.UI
{
    [RequireComponent(typeof(Button))]
    public class ReportUserButton : MonoBehaviour
    {
        [SerializeField] private Button button;

        private string accountId;
        private uint playerId;

        public void Initialize(string accountId, uint playerId)
        {
            this.accountId = accountId;
            this.playerId = playerId;
        }

        public void ShowReportUserPopup(string accountId, uint playerId)
        {
            var popup = (ReportPopup)PopupManager.Instance.CreatePopup(PopupManager.PopupType.ReportPopup, 0);
            popup.Initialize(ReportType.User, accountId, playerId);
            popup.OpenPopup();
        }

        private void HandleButtonClicked()
        {
            ShowReportUserPopup(accountId, playerId);
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
    }
}
