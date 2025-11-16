using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace Kumu.Kulitan.UI
{
    public class UICollapsiblePanel : MonoBehaviour
    {
        [SerializeField] private RectTransform rectBg;
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Events")] [SerializeField] private UnityEvent onPanelOpen;
        [SerializeField] private UnityEvent onPanelOpened;
        [SerializeField] private UnityEvent onPanelClose;
        [SerializeField] private UnityEvent onPanelClosed;

        [Header("Animation Settings")] [SerializeField]
        private float expandDuration = 0.5f;

        [SerializeField] private float collapseDuration = 0.1f;

        [SerializeField] private Ease expandEasing = Ease.OutBack;
        [SerializeField] private Ease collapseEasing = Ease.InBack;

        private bool isOpen;
        private bool isInitialized;
        private Vector2 bgOrigSizeDelta;

        public void ExpandPanel()
        {
            onPanelOpen?.Invoke();
            canvasGroup.interactable = false;
            canvasGroup.alpha = 1;
            rectBg.DOSizeDelta(bgOrigSizeDelta, expandDuration).SetEase(expandEasing)
                .OnComplete(() =>
                {
                    onPanelOpened?.Invoke();
                    canvasGroup.interactable = true;
                });
        }

        public void CollapsePanel()
        {
            onPanelClose?.Invoke();
            canvasGroup.interactable = false;
            rectBg.DOSizeDelta(Vector2.zero, collapseDuration).SetEase(collapseEasing)
                .OnComplete(() =>
                {
                    onPanelClosed?.Invoke();
                    canvasGroup.alpha = 0;
                });
        }

        private void ForceClosePanel(bool sendEvent = true)
        {
            canvasGroup.interactable = false;
            rectBg.sizeDelta = Vector2.zero;
            canvasGroup.alpha = 0;
            isOpen = false;

            if (sendEvent)
            {
                onPanelClosed?.Invoke();
            }
        }

        public void OnPanelToggled()
        {
            isOpen = !isOpen;

            if (isOpen)
            {
                ExpandPanel();
                return;
            }

            CollapsePanel();
        }

        private void OnEnable()
        {
            if (isInitialized)
            {
                return;
            }

            isInitialized = true;
            bgOrigSizeDelta = rectBg.sizeDelta;
            ForceClosePanel(false);
        }
    }
}
