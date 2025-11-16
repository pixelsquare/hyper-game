using Kumu.Kulitan.Hangout;
using Kumu.Kulitan.Multiplayer;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Kumu.Kulitan.UI
{
    public class ReportDetails : MonoBehaviour
    {
        [SerializeField] private TMP_Text headerText;
        [SerializeField] private Button submitButton;

        private Variables variables;
        private StateMachine stateMachine;
        private ReportPopup reportPopup;

        /// <summary>
        ///     Used by FSM.
        /// </summary>
        public void Initialize(string headerMessage)
        {
            headerText.text = headerMessage;
        }

        private void HandleSubmitButtonClicked()
        {
            if (reportPopup.ReportType == ReportType.Hangout)
            {
                variables.declarations.Set("PhotonRoomId", ConnectionManager.Client.CurrentRoom.Name);
                variables.declarations.Set("PhotonRoomName", RoomConnectionDetails.Instance.roomName);
            }
            
            stateMachine.TriggerUnityEvent("OnSubmit");
        }

        private void Awake()
        {
            variables = GetComponentInParent<Variables>();
            stateMachine = GetComponentInParent<StateMachine>();
            reportPopup = GetComponentInParent<ReportPopup>();
        }

        private void OnEnable()
        {
            submitButton.onClick.AddListener(HandleSubmitButtonClicked);
        }

        private void OnDisable()
        {
            submitButton.onClick.RemoveListener(HandleSubmitButtonClicked);
        }
    }
}
