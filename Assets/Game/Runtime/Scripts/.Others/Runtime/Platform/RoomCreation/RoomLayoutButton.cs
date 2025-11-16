using Kumu.Kulitan.Common;
using Kumu.Kulitan.Multiplayer;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Kumu.Kulitan.UI
{
    public class RoomLayoutButton : MonoBehaviour
    {
        [SerializeField] private Image imgIcon;
        [SerializeField] private GameObject iconHighlight;
        [SerializeField] private TextMeshProUGUI txtTemp;
        [SerializeField] private bool isSelected;
        [SerializeField] private Button button;

        private RoomLayoutSelectedEvent roomLayoutSelectedEvent;
        private Sprite previewIcon;
        public RoomLayoutConfig LayoutConfig { get; set; }

        private AsyncOperationHandle<Sprite> spriteOperation;

        public void Initialize(RoomLayoutConfig layoutConfig)
        {
            LayoutConfig = layoutConfig;
            SetTempName(layoutConfig.LayoutName);
            LoadIcon();
        }

        public void ToggleSelect(bool isButtonSelected)
        {
            isSelected = isButtonSelected;
            iconHighlight.SetActive(isSelected);
        }

        private void LoadIcon()
        {
            var address = LayoutConfig.LevelConfig.PreviewIconSpriteAddressableAddress;
            spriteOperation = Addressables.LoadAssetAsync<Sprite>(address);
            spriteOperation.Completed += OnIconLoaded;
        }

        private void OnIconLoaded(AsyncOperationHandle<Sprite> operationHandle)
        {
            imgIcon.sprite = operationHandle.Result;
        }

        private void UnloadIcon()
        {
            if (spriteOperation.IsValid())
            {
                Addressables.Release(spriteOperation);
            }
        }

        private void SetTempName(string name)
        {
            txtTemp.text = name;
        }

        private void OnButtonClicked()
        {
            GlobalNotifier.Instance.Trigger(roomLayoutSelectedEvent);
        }

        private void OnEnable()
        {
            button.onClick.AddListener(OnButtonClicked);
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(OnButtonClicked);
        }

        private void Start()
        {
            roomLayoutSelectedEvent = new RoomLayoutSelectedEvent(this);
        }

        private void OnDestroy()
        {
            UnloadIcon();
        }
    }
}
