using System.Collections;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class UICollapsibleDrawer : MonoBehaviour
{
    [SerializeField] private RectTransform targetRectT;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform.Edge origin;

    [Header("Events")]
    [SerializeField] private UnityEvent onPanelOpen;

    [SerializeField] private UnityEvent onPanelOpened;
    [SerializeField] private UnityEvent onPanelClose;
    [SerializeField] private UnityEvent onPanelClosed;

    [Header("Animation Settings")]
    [SerializeField] private float expandDuration = 0.5f;

    [SerializeField] private float collapseDuration = 0.1f;

    [SerializeField] private Ease expandEasing = Ease.OutBack;
    [SerializeField] private Ease collapseEasing = Ease.InBack;

    private bool isOpen;
    private bool isInitialized;
    private Vector2 originalSizeDelta;

    public async void ExpandPanel()
    {
        // HACK: waits for simple scroll snap
        // to do its panel calculation.
        while (!isInitialized)
        {
            // Engine delta time in milliseconds.
            await Task.Delay(Mathf.RoundToInt(Time.deltaTime * 1000f));
        }

        onPanelOpen?.Invoke();
        canvasGroup.alpha = 1.0f;
        canvasGroup.interactable = false;

        targetRectT.DOSizeDelta(originalSizeDelta, expandDuration)
                   .SetEase(expandEasing)
                   .OnComplete(() =>
                    {
                        onPanelOpened?.Invoke();
                        canvasGroup.interactable = true;
                    });
    }

    public void CollapsePanel()
    {
        onPanelClose?.Invoke();
        canvasGroup.alpha = 1.0f;
        canvasGroup.interactable = false;

        var startingSizeDelta = GetStartingSizeDelta();
        targetRectT.DOSizeDelta(startingSizeDelta, collapseDuration)
                   .SetEase(collapseEasing)
                   .OnComplete(() =>
                    {
                        onPanelClosed?.Invoke();
                        canvasGroup.alpha = 0.0f;
                    });
    }

    public void TogglePanel()
    {
        isOpen = !isOpen;

        if (isOpen)
        {
            ExpandPanel();
            return;
        }

        CollapsePanel();
    }

    private void ForceClose(bool sendEvent = true)
    {
        isOpen = false;
        canvasGroup.alpha = 0.0f;
        canvasGroup.interactable = false;
        targetRectT.sizeDelta = GetStartingSizeDelta();

        if (sendEvent)
        {
            onPanelClosed?.Invoke();
        }
    }

    private Vector2 GetStartingSizeDelta()
    {
        var sizeDelta = Vector2.zero;

        sizeDelta.x = origin switch
        {
            RectTransform.Edge.Top => originalSizeDelta.x,
            RectTransform.Edge.Bottom => originalSizeDelta.x,
            _ => 0.0f
        };

        sizeDelta.y = origin switch
        {
            RectTransform.Edge.Left => originalSizeDelta.y,
            RectTransform.Edge.Right => originalSizeDelta.y,
            _ => 0.0f
        };

        return sizeDelta;
    }

    private void Awake()
    {
        if (targetRectT == null)
        {
            Debug.LogError($"[{nameof(UICollapsibleDrawer)} -> {gameObject.name}]: Missing target rect transform. Disabling component.");
            enabled = false;
        }

        canvasGroup.alpha = 0.0f;
        canvasGroup.interactable = false;
    }

    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();

        var rect = targetRectT.rect;
        originalSizeDelta = rect.size;

        var pivot = targetRectT.pivot;
        pivot.x = origin == RectTransform.Edge.Right ? 1.0f : 0.0f;
        pivot.y = origin == RectTransform.Edge.Top ? 1.0f : 0.0f;
        targetRectT.pivot = pivot;

        targetRectT.anchorMin = Vector2.zero;
        targetRectT.anchorMax = Vector2.zero;

        var anchoredPos = targetRectT.anchoredPosition;
        anchoredPos.x = origin == RectTransform.Edge.Right ? originalSizeDelta.x : 0.0f;
        anchoredPos.y = origin == RectTransform.Edge.Top ? originalSizeDelta.y : 0.0f;
        targetRectT.anchoredPosition = anchoredPos;

        targetRectT.sizeDelta += originalSizeDelta;

        ForceClose(false);
        isInitialized = true;
    }
}
