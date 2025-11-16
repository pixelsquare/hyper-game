using System;
using DG.Tweening;
using Kumu.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Kumu.Kulitan.Common
{
    public class SlidePagesView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private SlidePageIndicators pageIndicators;
        [SerializeField] private SlidePagesDisplayContainer frontDisplayContainer;
        [SerializeField] private SlidePagesDisplayContainer backDisplayContainer;
        
        [SerializeField] private float crossfadeDuration = 0.5f;

        [Header("Buttons")]
        [SerializeField] private Button nextButton;
        [SerializeField] private Button skipButton;
        [SerializeField] private Button doneButton;

        [Header("Touch inputs")] 
        [SerializeField] private int deltaThreshold;

        public SlidePagesCollection SlidePagesCollection { get; private set; }

        public SlidePagesViewButtonLabels ButtonLabels => buttonLabels;
        
        public int CurrentIndex { get; private set; }
        
        public int PrevIndex { get; private set; }

        public bool IsInitialized => SlidePagesCollection != null;

        private Sequence crossfader;
        private TMP_Text nextButtonText;
        private TMP_Text skipButtonText;
        private TMP_Text doneButtonText;
        private SlidePagesViewButtonLabels buttonLabels;
        private Action onClosedSlidePagesCallback;

        public void SetData(SlidePagesCollection slidePagesCollection, SlidePagesViewButtonLabels labels = null, Action onClosedCallback = null)
        {
            SlidePagesCollection = slidePagesCollection;
            buttonLabels = labels;
            onClosedSlidePagesCallback = onClosedCallback;
            UpdateButtonLabels(buttonLabels ?? SlidePagesViewButtonLabels.Default);
            pageIndicators.SetPageIndicators(slidePagesCollection.Count);
            SetPageTo(0, true);
        }

        public void SetPageTo(int index, bool instantAnimation = false)
        {
            if (!IsInitialized)
            {
                "Slides are uninitialized!".LogError();
                return;
            }

            UpdateIndexes(index);
            UpdateButtonVisibility();
            PreparePages();
            PlayAnimations(instantAnimation);
        }

        private void PlayAnimations(bool instantAnimation = false)
        {
            PlayCrossfader(instantAnimation);
            pageIndicators.PlayAnimation(CurrentIndex, PrevIndex, instantAnimation);
        }

        private void UpdateIndexes(int index)
        {
            PrevIndex = Mathf.Clamp(CurrentIndex, 0, SlidePagesCollection.Count - 1);
            CurrentIndex = index;
        }

        private void PreparePages()
        {
            // set front page to new page
            var frontData = SlidePagesCollection.GetDataAt(CurrentIndex);
            frontDisplayContainer.SetData(frontData.DisplayText, frontData.DisplayImage);

            // set back page to old page
            var backData = SlidePagesCollection.GetDataAt(PrevIndex);
            backDisplayContainer.SetData(backData.DisplayText, backData.DisplayImage);
        }

        private void UpdateButtonLabels(SlidePagesViewButtonLabels labels)
        {
            nextButtonText.text = labels.nextLabel;
            doneButtonText.text = labels.doneLabel;
            skipButtonText.text = labels.skipLabel;
        }

        private void UpdateButtonVisibility()
        {
            if (!IsInitialized)
            {
                "Slides are uninitialized!".LogError();
                return;
            }
            
            skipButton.gameObject.SetActive(CurrentIndex != SlidePagesCollection.Count - 1);
            doneButton.gameObject.SetActive(CurrentIndex == SlidePagesCollection.Count - 1);
            nextButton.gameObject.SetActive(CurrentIndex != SlidePagesCollection.Count - 1);
        }

        private void Close()
        {
            onClosedSlidePagesCallback?.Invoke();
        }

        private void InitCrossfader()
        {
            crossfader = DOTween.Sequence()
                                .Insert(0f, DOTween.To(() => backDisplayContainer.Alpha, a => backDisplayContainer.Alpha = a, 0f, crossfadeDuration))
                                .Insert(0f, DOTween.To(() => frontDisplayContainer.Alpha, a => frontDisplayContainer.Alpha = a, 1f, crossfadeDuration))
                                .SetLoops(1, LoopType.Restart)
                                .SetAutoKill(false)
                                .OnStart(() =>
                                {
                                    frontDisplayContainer.Alpha = 0f;
                                    backDisplayContainer.Alpha = 1f;
                                })
                                .Pause();
        }

        private void PlayCrossfader(bool instantAnimation = false)
        {
            if (instantAnimation)
            {
                crossfader.Complete();
                return;
            }
            
            crossfader.Restart();
        }

        #region EventHandlers

        private void HandleNextPageEvent()
        {
            if (!IsInitialized)
            {
                "Slides are uninitialized!".LogError();
                return;
            }

            if (CurrentIndex == SlidePagesCollection.Count - 1)
            {
                return;
            }

            SetPageTo(CurrentIndex + 1);
        }

        private void HandlePrevPageEvent()
        {
            if (!IsInitialized)
            {
                "Slides are uninitialized!".LogError();
                return;
            }

            if (CurrentIndex == 0)
            {
                return;
            }

            SetPageTo(CurrentIndex - 1);
        }

        private void HandleSkipEvent()
        {
            if (!IsInitialized)
            {
                "Slides are uninitialized!".LogError();
                return;
            }

            Close();
        }

        private void HandleDoneEvent()
        {
            if (!IsInitialized)
            {
                "Slides are uninitialized!".LogError();
                return;
            }
            
            Close();
        }

        #endregion

        #region IEndDragHandler

        [SerializeField] private int dragDeltaThreshold = 50;

        public void OnBeginDrag(PointerEventData eventData)
        {
            // do nothing: all three IDrag interfaces need to be implemented to receive the callbacks
        }

        public void OnDrag(PointerEventData eventData)
        {
            // do nothing: all three IDrag interfaces need to be implemented to receive the callbacks
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (Mathf.Abs(eventData.delta.x) < dragDeltaThreshold)
            {
                return;
            }

            if (Mathf.Sign(eventData.delta.x) > 0)
            {
                HandlePrevPageEvent();
            }
            else
            {
                HandleNextPageEvent();
            }
        }

        #endregion

        #region Monobehaviour

        private void Awake()
        {
            if (nextButton)
            {
                nextButtonText = nextButton.GetComponentInChildren<TMP_Text>();
            }

            if (skipButton)
            {
                skipButtonText = skipButton.GetComponentInChildren<TMP_Text>();
            }

            if (doneButton)
            {
                doneButtonText = doneButton.GetComponentInChildren<TMP_Text>();
            }

            InitCrossfader();
        }

        private void OnEnable()
        {
            nextButton.onClick.AddListener(HandleNextPageEvent);
            skipButton.onClick.AddListener(HandleSkipEvent);
            doneButton.onClick.AddListener(HandleDoneEvent);
        }

        private void OnDisable()
        {
            nextButton.onClick.RemoveListener(HandleNextPageEvent);
            skipButton.onClick.RemoveListener(HandleSkipEvent);
            doneButton.onClick.RemoveListener(HandleDoneEvent);
        }

        #endregion
    }
}
