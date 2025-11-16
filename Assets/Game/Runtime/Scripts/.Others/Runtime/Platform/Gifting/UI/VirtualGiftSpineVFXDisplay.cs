using System;
using System.Collections;
using Spine.Unity;
using UnityEngine;

namespace Kumu.Kulitan.Gifting
{
    public class VirtualGiftSpineVFXDisplay : MonoBehaviour, IVirtualGiftBasicVFXDisplay
    {
        [SerializeField] private int trackIndex = 0;
        [SerializeField] private string animationName = "Play";
        private SkeletonGraphic skeletonGraphic;
        private Coroutine waitThenHideCoroutine;
        
        public float Lifetime { get; set; }
        public event Action OnCompleted;

        public void ShowVFX(int multiplier)
        {
            EndCoroutine();
            SetMultiplier(multiplier);
            gameObject.SetActive(true);
            var animState = skeletonGraphic.AnimationState;
            animState.SetAnimation(trackIndex, animationName, false);
            waitThenHideCoroutine = StartCoroutine(WaitThenHide());
        }

        private void EndCoroutine()
        {
            if (waitThenHideCoroutine == null)
            {
                return;
            }
            StopCoroutine(waitThenHideCoroutine);
            waitThenHideCoroutine = null;
        }

        private void SetMultiplier(int multiplier)
        {
            var multiplierDisplay =GetComponent<VirtualGiftVFXModifier>();
            multiplierDisplay.SetMultiplierText(multiplier);
        }

        private IEnumerator WaitThenHide()
        {
            yield return new WaitForSeconds(Lifetime);
            gameObject.SetActive(false);
            EndCoroutine();
            OnCompleted?.Invoke();
        }
        
        private void Awake()
        {
            skeletonGraphic = GetComponent<SkeletonGraphic>();
        }
    }
}
