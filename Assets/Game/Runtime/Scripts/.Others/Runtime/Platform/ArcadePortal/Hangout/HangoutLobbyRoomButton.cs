using Kumu.Kulitan.Common;
using Kumu.Kulitan.Hangout;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Kumu.Kulitan.Multiplayer
{
    public class HangoutLobbyRoomButton : MonoBehaviour
    {
        [SerializeField] private string roomName;

        [SerializeField] private Button button;

        [SerializeField] private TextMeshProUGUI txtName;
        [SerializeField] private TextMeshProUGUI txtOwner;
        [SerializeField] private TextMeshProUGUI txtPlayerCount;
        [SerializeField] private Image imgPreviewIcon;

        // TODO: load LEVEL CONFIG dynamically, currently this is always set to BIRTHDAY ROOM asset
        [SerializeField] private LevelConfigScriptableObject levelConfig;
        private AsyncOperationHandle<Sprite> spriteOperation;

        public string RoomName => roomName;
        public string SceneName { get; set; }
        public string LayoutName { get; set; }
        public string RoomId { get; set; }
        public static bool CanLoadHangout { get; set; }

        public LevelConfigScriptableObject LevelConfig
        {
            get => levelConfig;
            set => levelConfig = value;
        }

        public int PlayerCount { get; private set; }
        public int MaxPlayersCount { get; private set; }
        public string PreviewIconAddress { get; private set; }

        public void UpdatePlayerCount(int count, int maxPlayersCount)
        {
            PlayerCount = count;
            MaxPlayersCount = maxPlayersCount;
            txtPlayerCount.text = $"{count}/{MaxPlayersCount}";
        }

        public void LoadHangout()
        {
            if (!CanLoadHangout)
            {
                return;
            }
            
            RoomConnectionDetails.Instance.levelConfig = levelConfig;
            RoomConnectionDetails.Instance.roomName = RoomId;
            RoomConnectionDetails.Instance.sceneName = SceneName;

            var roomDetails = new RoomDetails
            {
                roomId = RoomId,
                roomName = roomName,
                sceneName = SceneName,
                layoutName = LayoutName
            };

            GlobalNotifier.Instance.Trigger(new HangoutJoinRoomBtnClickedEvent(roomDetails, PreviewIconAddress, levelConfig));
        }

        public void SetButtonInteractable(bool isInteractable)
        {
            button.interactable = isInteractable;
        }

        public void SetRoomName(string name)
        {
            roomName = name;
            txtName.text = name;
        }

        public void SetOwnerName(string name)
        {
            txtOwner.text = name;
        }

        public void LoadIcon(string address)
        {
            PreviewIconAddress = address;
            spriteOperation = Addressables.LoadAssetAsync<Sprite>(address);
            spriteOperation.Completed += OnIconLoaded;
        }

        private void OnIconLoaded(AsyncOperationHandle<Sprite> operationHandle)
        {
            imgPreviewIcon.sprite = operationHandle.Result;
        }

        private void UnloadIcon()
        {
            if (!spriteOperation.IsValid())
            {
                return;
            }

            Addressables.Release(spriteOperation);
        }

        private void OnDestroy()
        {
            UnloadIcon();
        }
    }
}
