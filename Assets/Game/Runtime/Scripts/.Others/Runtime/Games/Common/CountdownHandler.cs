using Quantum;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

namespace Kumu.Kulitan.KumuJumper
{
    /// <summary>
    /// Polls the countdown timer from the simulation
    /// and sends an event to display the update.
    /// </summary>
    public class CountdownHandler : MonoBehaviour
    {
        [SerializeField] private PlayableDirector countdownTimeline;
        [SerializeField] private GameControllerDataAsset gameData;

        private bool isCountingDown;
        
        [SerializeField] private UnityEvent onCountdownFinish;

        private unsafe void UpdateCountdown()
        {
            var f = QuantumRunner.Default.Game.Frames.Verified;
            var gameController = f.Global->gameController;
            var time = gameController.countdownTimer.AsDouble;
            countdownTimeline.time = time;
            countdownTimeline.Evaluate();
            
            if (gameController.countdownTimer >= gameData.Settings.countdownToStart)
            {
                isCountingDown = false;
                onCountdownFinish.Invoke();
            }
        }

        private void Awake()
        {
            isCountingDown = true;
        }

        private void Update()
        {
            if (QuantumRunner.Default == null || QuantumRunner.Default.Game.Frames.Verified == null || !isCountingDown)
            {
                return;
            }
            
            UpdateCountdown();
        }
    }
}
