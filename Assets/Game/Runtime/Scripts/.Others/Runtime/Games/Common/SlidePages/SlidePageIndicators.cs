using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Kumu.Kulitan.Common
{
    public class SlidePageIndicators : MonoBehaviour
    {
        [SerializeField] private RectTransform indicatorPrefab;

        [SerializeField] private float animationDuration = 0.5f;

        [SerializeField] private float expandFactor = 3f;

        private RectTransform selfRectTransform;

        private readonly List<Sequence> sequenceAnimators = new();
        
        /// <summary>
        /// Clear out any old indicators and create new ones.
        /// </summary>
        /// <param name="count">Amount of page indicators.</param>
        public void SetPageIndicators(int count)
        {
            ClearIndicators();
            CreateIndicators(count);
        }
        
        /// <summary>
        /// Play expand animation for currentIndex and shrink animation for prevIndex indicators.
        /// </summary>
        /// <param name="currentIndex">Index of indicator which will play the expand animation.</param>
        /// <param name="prevIndex">Index of indicator which will play the shrink animation.</param>
        /// <param name="instantAnimation">If true, will instantly finish the animation.</param>
        public void PlayAnimation(int currentIndex, int prevIndex, bool instantAnimation = false)
        {
            if (instantAnimation)
            {
                sequenceAnimators[prevIndex].Rewind();
                sequenceAnimators[currentIndex].Complete();
                return;
            }
            
            sequenceAnimators[prevIndex].PlayBackwards();
            sequenceAnimators[currentIndex].PlayForward();
        }

        private void ClearIndicators()
        {
            for (var i = transform.childCount - 1; i >= 0; i--)
            {
                Destroy(transform.GetChild(i).gameObject);
                sequenceAnimators[i].Kill();
            }
            
            sequenceAnimators.Clear();
        }

        private void CreateIndicators(int count)
        {
            for (var i = 0; i < count; i++)
            {
                var pageIndicator = Instantiate(indicatorPrefab, Vector3.zero, Quaternion.identity, transform);
                sequenceAnimators.Add(CreateSequence(pageIndicator));
            }
        }

        private Sequence CreateSequence(RectTransform pageIndicator)
        {
            var rect = indicatorPrefab.rect;
            return DOTween.Sequence()
                          .Insert(0f, pageIndicator.DOSizeDelta(new Vector2(rect.width * expandFactor, rect.height), animationDuration).SetEase(Ease.Linear))
                          .Insert(0f, DOTween.To(() => 0f, _ => LayoutRebuilder.MarkLayoutForRebuild(selfRectTransform), 0f, animationDuration).SetEase(Ease.Linear))
                          .SetAutoKill(false)
                          .Pause();
        }

        private void Awake()
        {
            selfRectTransform = GetComponent<RectTransform>();
        }
    }
}
