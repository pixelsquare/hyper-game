using System;
using System.Collections;
using UnityEngine;

namespace Kumu.Kulitan.Gifting
{
    public class VirtualGiftBasicAnimVFXDisplay : MonoBehaviour, IVirtualGiftBasicVFXDisplay
    {
        [SerializeField] private Animator myAnimator;
        private const string PlayTrigger = "Play";

        public float Lifetime { get; set; }
        public event Action OnCompleted;

        public void ShowVFX(int multiplier)
        {
            myAnimator.SetTrigger(PlayTrigger);
            StartCoroutine(WaitThenDestroy());
        }

        private IEnumerator WaitThenDestroy()
        {
            yield return new WaitForSeconds(Lifetime);
            OnCompleted?.Invoke();
            Destroy(gameObject);
        }
    }
}
