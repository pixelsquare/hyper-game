using System.Collections;
using Quantum;
using TMPro;
using UnityEngine;

namespace Kumu.Kulitan.Minigame
{
    public class CancelledMinigameHandler : MonoBehaviour
    {
        [SerializeField] private float hideTimer = 3f;
        [SerializeField] private TextMeshProUGUI textPrompt;

        private Coroutine promptCoroutine;

        private void ResetPrompt()
        {
            if (promptCoroutine != null)
            {
                StopCoroutine(promptCoroutine);
                promptCoroutine = null;
            }
            textPrompt.gameObject.SetActive(false);   
        }

        private void OnMinigameCancel(EventOnMinigameCancel eventData)
        {
            promptCoroutine = StartCoroutine(TimedHideText());
        }
        
        private void OnMinigameStart(EventOnMinigameStart callback)
        {
            ResetPrompt();
        }
        
        private void OnMinigameCountdownStart(EventOnMinigameCountdownStart callback)
        {
            ResetPrompt();
        }

        private IEnumerator TimedHideText()
        {
            textPrompt.gameObject.SetActive(true);
            yield return new WaitForSeconds(hideTimer);
            textPrompt.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            QuantumEvent.Subscribe<EventOnMinigameStart>(this, OnMinigameStart);
            QuantumEvent.Subscribe<EventOnMinigameCountdownStart>(this, OnMinigameCountdownStart);
            QuantumEvent.Subscribe<EventOnMinigameCancel>(this, OnMinigameCancel);
        }
        
        private void OnDisable()
        {
            QuantumEvent.UnsubscribeListener(this);
        }        
    }
}
