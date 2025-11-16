using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;
using Kumu.Kulitan.Multiplayer;

namespace Kumu.Kulitan.UI
{
    public class MatchmakingPopup : BasePopup
    {
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI countdownText;
        [SerializeField] private TextMeshProUGUI buttonText;
        [SerializeField] private TextMeshProUGUI warningText;

        [SerializeField] private Button closeButton;

        [SerializeField] private LobbyPlayerNameUI playerNameTemplate;
        [SerializeField] private RectTransform playerNamesContainer;

        private List<LobbyPlayerNameUI> playerNames = new List<LobbyPlayerNameUI>();

        public event Action OnCancel;

        /// <summary>
        /// Call this method when popup is first shown.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="countdown"></param>
        /// <param name="button"></param>
        public void Initialize(string title, int countdown, string button)
        {
            SetDetails(title, countdown, button);
            ShowHideCloseButton(false);
            ShowHideWarningText(false);
        }

        public void SetDetails(string title, int countdown, string button)
        {
            titleText.text = title;
            countdownText.text = countdown.ToString();
            if (button != null)
            {
                buttonText.text = button;
            }
            else
            {
                closeButton.gameObject.SetActive(false);
            }
        }

        public void ShowHideCloseButton(bool toShow)
        {
            closeButton.interactable = toShow;
        }

        public void ShowHideCountdownText(bool toShow)
        {
            countdownText.gameObject.SetActive(toShow);
        }

        public void ShowHideWarningText(bool toShow, string msg = "")
        {
            if (!string.IsNullOrEmpty(msg))
            {
                warningText.text = msg;
            }
            warningText.gameObject.SetActive(toShow);
        }

        public void UpdateTimer(int time)
        {
            countdownText.text = time.ToString();
        }

        public void AddCallback(Action callback)
        {
            OnCancel += () =>
            {
                callback?.Invoke();
            };
        }

        public void OnPopupClose()
        {
            OnCancel?.Invoke();
            Close();
        }

        public void GeneratePlayerNames(IEnumerable<Player> players)
        {
            foreach (LobbyPlayerNameUI playerName in playerNames)
            {
                Destroy(playerName.gameObject);
            }
            playerNames.Clear();

            foreach (var player in players)
            {
                LobbyPlayerNameUI playerName = Instantiate(playerNameTemplate, playerNamesContainer);
                playerName.SetPlayerName(player.NickName, player.IsLocal);
                playerNames.Add(playerName);
            }
        }

        private void Start()
        {
            ShowHideCloseButton(false);
        }

        private void OnEnable()
        {
            closeButton.onClick.AddListener(OnPopupClose);   
        }

        private void OnDisable()
        {
            closeButton.onClick.RemoveListener(OnPopupClose);
        }
    }
}
