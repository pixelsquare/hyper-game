using Kumu.Kulitan.Hangout;
using Quantum;
using UnityEngine;

namespace Kumu.Kulitan.Minigame
{
    public class MinigameSfxHandler : MonoBehaviour
    {
        [SerializeField] private AudioClip readyToStartClip;
        [SerializeField] private AudioClip joinMinigameClip;
        [SerializeField] private AudioClip minigameEndClip;
        
        private AudioUISFXPlayer uiSfxPlayer;  // used for playing sounds global

        /// <summary>
        /// Called at the very start of the minigame,
        /// in time with the READY timer.
        /// </summary>
        /// <param name="callback"></param>
        private unsafe void OnMinigameCountdownEnd(EventOnMinigameCountdownEnd callback)
        {
            var f = QuantumRunner.Default.Game.Frames.Verified;
            var gameController = f.Global->minigameController;
            if (gameController.minigameState != MinigameState.Ready)
            {
                return;
            }
            RefreshPlayerReference();
            uiSfxPlayer.PlayUISFXClip(readyToStartClip);
        }
        
        private unsafe void OnMinigameCountdownStart(EventOnMinigameCountdownStart callback)
        {
            var f = QuantumRunner.Default.Game.Frames.Verified;
            var gameController = f.Global->minigameController;
            if (gameController.minigameState != MinigameState.Join)
            {
                return;
            }
            RefreshPlayerReference();
            uiSfxPlayer.PlayUISFXClip(joinMinigameClip);
        }

        private void OnMinigameEnd(EventOnMinigameEnd callback)
        {
            RefreshPlayerReference();
            uiSfxPlayer.PlayUISFXClip(minigameEndClip);
        }

        private void RefreshPlayerReference()
        {
            if (uiSfxPlayer == null)
            {
                uiSfxPlayer = FindObjectOfType<AudioUISFXPlayer>();
            }
        }

        private void OnEnable()
        {
            RefreshPlayerReference();
            QuantumEvent.Subscribe<EventOnMinigameCountdownEnd>(this, OnMinigameCountdownEnd);
            QuantumEvent.Subscribe<EventOnMinigameCountdownStart>(this, OnMinigameCountdownStart);
            QuantumEvent.Subscribe<EventOnMinigameEnd>(this, OnMinigameEnd);

        }

        private void OnDisable()
        {
            QuantumEvent.UnsubscribeListener(this);
        }
    }
}
