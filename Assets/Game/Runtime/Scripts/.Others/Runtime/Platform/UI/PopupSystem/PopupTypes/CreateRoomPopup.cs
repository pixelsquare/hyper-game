using System;
using System.Globalization;
using Kumu.Kulitan.Hangout;
using Kumu.Kulitan.Multiplayer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kumu.Kulitan.UI
{
    public class CreateRoomPopup : BasePopup
    {
        [SerializeField] private int roomNameCharLimit = 16;
        [SerializeField] private TMP_InputField inputFieldRoomName;
        [SerializeField] private TextMeshProUGUI errorText;
        [SerializeField] private Button submitButton;
        [SerializeField] private Toggle isFriendsOnlyToggle;

        [SerializeField] private LevelConfigScriptableObject levelConfig;

        private string roomName;

        public Action OnSubmit { get; set; }

        public RoomLayoutConfig RoomLayoutConfig { get; set; }

        public LevelConfigScriptableObject LevelConfig
        {
            get => levelConfig;
            set => levelConfig = value;
        }

        public void AddCallback(Action<RoomDetails, LevelConfigScriptableObject> callback)
        {
            OnSubmit += () =>
            {
                var roomDetails = new RoomDetails
                {
                    roomName = roomName,
                    layoutName = RoomLayoutConfig.LayoutName,
                    sceneName = levelConfig.SceneToLoad,
                    isFriendsOnly = isFriendsOnlyToggle.isOn
                };

                callback?.Invoke(roomDetails, levelConfig);
            };
        }

        public void Submit()
        {
            if (string.IsNullOrWhiteSpace(inputFieldRoomName.text.Trim()))
            {
                PopupManager.Instance.OpenErrorPopup("Error", "Room name not set.", "Okay");
                return;
            }

            roomName = inputFieldRoomName.text;

            RoomConnectionDetails.Instance.roomName = roomName;
            RoomConnectionDetails.Instance.levelConfig = levelConfig;
            RoomConnectionDetails.Instance.sceneName = levelConfig.SceneToLoad;

            OnSubmit?.Invoke();
            Close();
        }

        private void ValidateRoomname(string text)
        {
            if (string.IsNullOrWhiteSpace(text.Trim()))
            {
                ShowErrorText("* Room Name cannot be blank.");
                submitButton.interactable = false;
            }
            else if (HasEmoji(text.Trim()))
            {
                ShowErrorText("* Room Name invalid.");
                submitButton.interactable = false;
            }
            else
            {
                submitButton.interactable = true;
                HideErrorText();
            }
        }

        private void ShowErrorText(string error)
        {
            if (!errorText)
            {
                return;
            }

            errorText.text = error;
            errorText.gameObject.SetActive(true);
        }

        private void HideErrorText()
        {
            if (!errorText)
            {
                return;
            }

            errorText.gameObject.SetActive(false);
        }

        private void Initialize()
        {
            HideErrorText();
            inputFieldRoomName.characterLimit = roomNameCharLimit;
            submitButton.interactable = false;
        }

        private void OnEnable()
        {
            submitButton.onClick.AddListener(Submit);
            inputFieldRoomName.onValueChanged.AddListener(ValidateRoomname);
            Initialize();
        }

        private void OnDisable()
        {
            submitButton.onClick.RemoveListener(Submit);
            inputFieldRoomName.onValueChanged.RemoveListener(ValidateRoomname);
        }

        // Reference: https://forum.unity.com/threads/solved-disable-emoji-in-mobile-keyboard.552202/#post-5728351
        private bool HasEmoji(string text)
        {
            foreach (var c in text)
            {
                var unicodeCategory = char.GetUnicodeCategory(c);

                switch (unicodeCategory)
                {
                    case UnicodeCategory.OtherSymbol:
                    case UnicodeCategory.Surrogate:
                    case UnicodeCategory.ModifierSymbol:
                    case UnicodeCategory.NonSpacingMark:
                        return true;
                }
            }

            return false;
        }
    }
}
