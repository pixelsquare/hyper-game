using System;
using System.Collections.Generic;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;
using Kumu.Kulitan.Multiplayer;
using UnityEngine;
using UnityEngine.UI;

namespace Kumu.Kulitan.UI
{
    public class RoomLayoutPopup : BasePopup
    {
        [SerializeField] private Button submitButton;
        [SerializeField] private Button bgCloseButton;
        [SerializeField] private Button closeButton;

        [SerializeField] private RectTransform layoutButtonsContainer;
        [SerializeField] private GameObject layoutButtonPrefab;
        [SerializeField] private List<RoomLayoutButton> currLayoutButtons = new();

        [SerializeField] private RoomLayoutConfigs layoutConfigs;
        private RoomLayoutConfig activeConfig;
        private Slot<string> eventSlot;

        public Action OnSubmit { get; set; }

        public void AddCallback(Action<RoomLayoutConfig> callback)
        {
            OnSubmit += () =>
            {
                callback?.Invoke(activeConfig);
            };
        }

        public void Initialize()
        {
            AddButtons();

            if (currLayoutButtons.Count <= 0 && layoutConfigs.LayoutConfigs.Length <= 0)
            {
                return;
            }

            currLayoutButtons[0].ToggleSelect(true);
            activeConfig = layoutConfigs.LayoutConfigs[0];
        }

        private void AddButtons()
        {
            foreach (var layout in layoutConfigs.LayoutConfigs)
            {
                var btn = Instantiate(layoutButtonPrefab, layoutButtonsContainer).GetComponent<RoomLayoutButton>();
                currLayoutButtons.Add(btn);
                btn.Initialize(layout);
            }
        }

        private void RemoveAllButtons()
        {
            foreach (var btn in currLayoutButtons)
            {
                Destroy(btn);
            }

            currLayoutButtons.Clear();
        }

        private void ActivateButton(RoomLayoutButton btnToActivate)
        {
            foreach (var btn in currLayoutButtons)
            {
                btn.ToggleSelect(btn == btnToActivate);
            }

            activeConfig = btnToActivate.LayoutConfig;
        }

        private void OnSubmitButtonClicked()
        {
            OnSubmit?.Invoke();
            Close();
        }

        private void OnCloseButtonClicked()
        {
            Close();
        }

        private void OnRoomLayoutSelected(IEvent<string> callback)
        {
            var eventCallback = (RoomLayoutSelectedEvent)callback;
            ActivateButton(eventCallback.LayoutButton);
        }

        private void OnEnable()
        {
            eventSlot.SubscribeOn(RoomLayoutSelectedEvent.EVENT_NAME, OnRoomLayoutSelected);
            submitButton.onClick.AddListener(OnSubmitButtonClicked);
            bgCloseButton.onClick.AddListener(OnCloseButtonClicked);
            closeButton.onClick.AddListener(OnCloseButtonClicked);

            Initialize();
        }

        private void OnDisable()
        {
            RemoveAllButtons();
        }

        private void Awake()
        {
            eventSlot = new Slot<string>(GlobalNotifier.Instance);
        }
    }
}
