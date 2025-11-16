using System.Collections;
using UnityEngine;
using DanielLochner.Assets.SimpleScrollSnap;

namespace Kumu.Kulitan.UI
{
    /// <summary>
    /// Adds an autoscroller to the SimpleScrollSnap.cs
    /// </summary>
    public class AutoScroller : MonoBehaviour
    {
        [SerializeField] private float delay = 3.0f;
        [SerializeField] private bool isScrollEnabled = false;
        [SerializeField] private SimpleScrollSnap scrollSnap;

        private Coroutine coroutine;

        public void BeginAutoScroll()
        {
            StopAutoScroll();
            isScrollEnabled = true;
            coroutine = StartCoroutine(NextPageTimer());
        }

        public void StopAutoScroll()
        {
            isScrollEnabled = false;
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
        }

        private IEnumerator NextPageTimer()
        {
            while (isScrollEnabled)
            {
                yield return new WaitForSeconds(delay);
                scrollSnap.GoToNextPanel();
            }
        }

        private void Start()
        {
            BeginAutoScroll();
        }
    }
}
