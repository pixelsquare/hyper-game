using Kumu.Kulitan.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kumu.Kulitan.Gifting
{
    public class VGPlayerButton : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI txtPlayerName;
        [SerializeField] private GameObject selectedImg;
        private VirtualGiftGifteeData gifteeData;
        private Button button;
        private VGPlayerListPopup popup;
        private bool isSelected;
        
        public VirtualGiftGifteeData GifteeData => gifteeData;
        
        public void SetDetails(VirtualGiftGifteeData gifteeData, VGPlayerListPopup popup)
        {
            txtPlayerName.text = gifteeData.username;
            this.gifteeData.id = gifteeData.id;
            this.gifteeData.nickname = gifteeData.nickname;
            this.gifteeData.username = gifteeData.username;
            this.popup = popup;
        }

        public void SetState(bool selected)
        {
            isSelected = selected;
            selectedImg.SetActive(selected);
        }

        private void OnClick()
        {
            if (isSelected)
            {
                return;
            }
            popup.UpdateButtonsState(this);
        }
        
        private void Awake()
        {
            button = GetComponent<Button>();
        }
        
        private void OnEnable()
        {
            button.onClick.AddListener(OnClick);
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(OnClick);
        }
    }
}
