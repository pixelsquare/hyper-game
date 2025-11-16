using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;
using Kumu.Kulitan.Multiplayer;
using Quantum;
using TMPro;
using UnityEngine;

namespace Kumu.Kulitan.Minigame
{
    public unsafe class MinigameCountdownTimerHandler : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI txtPrompt;
        [SerializeField] private TextMeshProUGUI txtGameTimer;
        private bool isCountdownRunning;
        private float startCountdownTimer;

        private Slot<string> eventSlot;
        
        public void ShowHideTimerDisplay(bool isDisplayed)
        {
            txtPrompt.gameObject.SetActive(isDisplayed);
            txtGameTimer.gameObject.SetActive(isDisplayed);
        }

        private void PollTimer()
        {
            var f = QuantumRunner.Default.Game.Frames.Verified;
            var gameController = f.Global->minigameController;
            startCountdownTimer = gameController.countdown.AsFloat;
            
            UpdateTimerDisplay();
        }
        
        private void OnMinigameCountdownStart(EventOnMinigameCountdownStart callback)
        {
            isCountdownRunning = true;
            ShowHideTimerDisplay(true);
        }
        
        private void OnMinigameCountdownEnd(EventOnMinigameCountdownEnd callback)
        {
            isCountdownRunning = false;
            ShowHideTimerDisplay(false);
        }

        private void UpdateTimerDisplay()
        {
            if (txtGameTimer == null)
            {
                return;
            }

            var f = QuantumRunner.Default.Game.Frames.Verified;
            var gameController = f.Global->minigameController;
    
            switch (gameController.minigameState)
            {
                case MinigameState.Join:
                    txtPrompt.text = "Minigame starts in...";
                    break;
                case MinigameState.Result:
                    txtPrompt.text = "Returning to hangout in...";
                    break;
            }

            txtGameTimer.text = startCountdownTimer.ToString("00");
        }
        
        private void OnPlayerJoined(IEvent<string> callback)
        {
            if (callback is QuantumPlayerJoinedEvent eventData
                && eventData.IsLocal)
            {
                var f = QuantumRunner.Default.Game.Frames.Verified;
                var minigameState = f.Global->minigameController.minigameState;
                var countdownStates = MinigameState.Join | MinigameState.Result;
                isCountdownRunning = minigameState == MinigameState.Join
                                     || minigameState == MinigameState.Result;
                
                ShowHideTimerDisplay(isCountdownRunning);
            }
        }

        private void OnEnable()
        { 
            QuantumEvent.Subscribe<EventOnMinigameCountdownStart>(this, OnMinigameCountdownStart);
            QuantumEvent.Subscribe<EventOnMinigameCountdownEnd>(this, OnMinigameCountdownEnd);
            
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
                !isCountdownRunning)
            {
                return;
            }
            
            PollTimer();
        }
    }
}
