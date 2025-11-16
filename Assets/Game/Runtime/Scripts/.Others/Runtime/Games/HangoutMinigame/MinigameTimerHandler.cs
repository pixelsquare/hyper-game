using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;
using Kumu.Kulitan.Multiplayer;
using Quantum;
using TMPro;
using UnityEngine;

namespace Kumu.Kulitan.Minigame
{
    public unsafe class MinigameTimerHandler : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI txtGameTimer;
        
        private bool isMinigameRunning;
        private float gameTimer;
        private Slot<string> eventSlot;

        public void ShowHideTimerDisplay(bool isDisplayed)
        {
            txtGameTimer.gameObject.SetActive(isDisplayed);
            isMinigameRunning = isDisplayed;
        }

        private void PollTimer()
        {
            var f = QuantumRunner.Default.Game.Frames.Verified;
            var gameController = f.Global->minigameController;
            gameTimer = gameController.countdown.AsFloat;
            
            UpdateTimerDisplay();
        }
        
        private void OnMinigameStart(EventOnMinigameStart callback)
        {
            var f = QuantumRunner.Default.Game.Frames.Verified;
            var minigameState = f.Global->minigameController.minigameState;
            if (minigameState != MinigameState.Play)
            {
                return;
            }
            ShowHideTimerDisplay(true);
        }

        private void OnMinigameEnd(EventOnMinigameEnd callback)
        {
            ShowHideTimerDisplay(false);
        }

        private void OnPlayerJoined(IEvent<string> callback)
        {
            if (callback is QuantumPlayerJoinedEvent eventData
                && eventData.IsLocal)
            {
                var f = QuantumRunner.Default.Game.Frames.Verified;
                var minigameState = f.Global->minigameController.minigameState;
                isMinigameRunning = minigameState == MinigameState.Play;

                ShowHideTimerDisplay(isMinigameRunning);
            }
        }

        private void UpdateTimerDisplay()
        {
            if (txtGameTimer == null)
            {
                return;
            }

            txtGameTimer.text = gameTimer.ToString("00");
        }

        private void OnEnable()
        { 
            QuantumEvent.Subscribe<EventOnMinigameStart>(this, OnMinigameStart);
            QuantumEvent.Subscribe<EventOnMinigameEnd>(this, OnMinigameEnd);
            
            eventSlot = new Slot<string>(GlobalNotifier.Instance);
            eventSlot.SubscribeOn(QuantumPlayerJoinedEvent.EVENT_NAME, OnPlayerJoined);
        }

        private void OnDisable()
        {
            QuantumEvent.UnsubscribeListener(this);
            eventSlot.Dispose();
        }

        private void Update()
        {
            if (QuantumRunner.Default == null || QuantumRunner.Default.Game.Frames.Verified == null || 
                !isMinigameRunning)
            {
                return;
            }
            
            PollTimer();
        }
    }
}
