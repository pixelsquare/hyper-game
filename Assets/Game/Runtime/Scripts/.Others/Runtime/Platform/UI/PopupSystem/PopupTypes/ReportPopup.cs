using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;
using Unity.VisualScripting;
using UnityEngine;

namespace Kumu.Kulitan.UI
{
    [RequireComponent(typeof(Variables))]
    public class ReportPopup : BasePopup
    {
        [SerializeField] private UICollapsibleDrawer collapsibleDrawer;

        public string ReportedAccountId { get; private set; }
        public ReportType ReportType { get; private set; }

        private Variables variables;

        /// <summary>
        ///     Used by FSM.
        /// </summary>
        public void Initialize(ReportType reportType, string accountId, uint playerId)
        {
            ReportType = reportType;
            ReportedAccountId = accountId;
            variables.declarations.Set("UserAccountId", accountId);
            variables.declarations.Set("UserPlayerId", playerId);
        }

        /// <summary>
        ///     Used by FSM.
        /// </summary>
        public void OnUserBlocked(string accountId, uint playerId)
        {
            GlobalNotifier.Instance.Trigger(new PlayerBlockedEvent(accountId, playerId, true));
            GlobalNotifier.Instance.Trigger(new FSMSendUnityEvent("ReinitializeUserProfile"));
        }

        public void OpenPopup()
        {
            collapsibleDrawer.ExpandPanel();
        }

        public void ClosePopup()
        {
            collapsibleDrawer.CollapsePanel();
        }

        private void Awake()
        {
            variables = GetComponent<Variables>();
        }
    }
}
