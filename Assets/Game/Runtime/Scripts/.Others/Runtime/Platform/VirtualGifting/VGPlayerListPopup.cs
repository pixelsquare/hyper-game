using System;
using System.Collections.Generic;
using Kumu.Kulitan.Gifting;
using UnityEngine;
using UnityEngine.UI;

namespace Kumu.Kulitan.UI
{
    public class VGPlayerListPopup : BasePopup
    {
        [SerializeField] private Button selectBtn;
        [SerializeField] private Button closeBtn;
        [SerializeField] private VGPlayerButton playerButtonPrefab;
        [SerializeField] private RectTransform buttonContainer;
        [SerializeField] private GameObject noUsersText;
        private List<VGPlayerButton> playerListButtons = new();
        private VirtualGiftGifteeData selectedGifteeData;
        
        public Action OnSubmit { get; set; }

        public void Initialize(VirtualGiftGifteeData[] gifteesData, string selectedGifteeId)
        {
            Clear();
            selectBtn.interactable = false;

            if (gifteesData.Length <= 0)
            {
                noUsersText.SetActive(true);
                return;
            }
            noUsersText.SetActive(false);
            foreach (var giftee in gifteesData)
            {
                var btn = Instantiate(playerButtonPrefab, buttonContainer);
                btn.SetDetails(giftee, this);
                if (giftee.id == selectedGifteeId)
                {
                    btn.SetState(true);
                }

                playerListButtons.Add(btn);
            }
        }
        
        public void AddCallback(Action<VirtualGiftGifteeData> callback)
        {
            OnSubmit += () =>
            {
                callback?.Invoke(selectedGifteeData);
            };
        }

        public void UpdateButtonsState(VGPlayerButton vgPlayerButton)
        {
            foreach (var btn in playerListButtons)
            {
                if (vgPlayerButton == btn)
                {
                    btn.SetState(true);
                    selectBtn.interactable = true;
                    selectedGifteeData = btn.GifteeData;
                    continue;
                }
                btn.SetState(false);
            }
        }

        public void Clear()
        {
            foreach (var btn in playerListButtons)
            {
                Destroy(btn.gameObject);
            }

            selectedGifteeData = default;
            playerListButtons.Clear();
        }

        private void OnSelectClicked()
        {
            OnSubmit?.Invoke();
            Close();
        }
        
        private void OnCloseButtonClicked()
        {
            Close();
        }

        private void OnEnable()
        {
            selectBtn.onClick.AddListener(OnSelectClicked);
            closeBtn.onClick.AddListener(OnCloseButtonClicked);
        }

        private void OnDisable()
        {
            selectBtn.onClick.RemoveListener(OnSelectClicked);
            closeBtn.onClick.RemoveListener(OnCloseButtonClicked);
        }
    }
}
