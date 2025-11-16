using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UICollapsibleSubMenuController : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform rectMenuBg;
    [SerializeField] private Button[] menuButtons;
    [SerializeField] private Vector2 buttonOriginPos = new(0f, -73f);
    [SerializeField] private Vector2 buttonSpacing = new(0f, 85f);
    [Header("Animation Settings")]
    [SerializeField] private float expandDuration = 0.5f;
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private float fadeOutDuration = 0.5f;
    [SerializeField] private float iconRotationDuration = 0.5f;
    [SerializeField] private Ease expandEasing = Ease.OutBack;
    [SerializeField] private Ease collapseEasing = Ease.InBack;
    [Header("Main Button Settings")]
    [SerializeField] private Button mainButton;
    [SerializeField] private Image imgClosedIcon;
    [SerializeField] private Image imgOpenedIcon;
    [SerializeField] private RectTransform rectClosedIcon;
    [SerializeField] private RectTransform rectOpenedIcon;
    
    private bool isOpen = false;
    private RectTransform mainButtonRect;
    private Vector2 menuBgOrigSizeDelta;

    private void SetMainIcon(bool toExpandedIcon)
    {
        imgOpenedIcon.DOFade(toExpandedIcon?1:0, iconRotationDuration);
        imgClosedIcon.DOFade(toExpandedIcon?0:1, iconRotationDuration);
        rectOpenedIcon.DORotate(toExpandedIcon?Vector3.forward : Vector3.back * 180f, iconRotationDuration);
    }
    
    private void ShowHideButtons(bool toShow)
    {
        foreach (var btn in menuButtons)
        {
            btn.gameObject.SetActive(toShow);
        }
    }
    
    private void EnableButtons(bool toEnable)
    {
        foreach (var btn in menuButtons)
        {
            btn.interactable = toEnable;
        }
    }

    private void ExpandMenu()
    {
        canvasGroup.interactable = false;
        mainButton.interactable = false;
        ShowHideButtons(true);
        for (var i = 0; i < menuButtons.Length; i++)
        {
            var targetBgSizeDelta = rectMenuBg.sizeDelta + -buttonSpacing * i;
            rectMenuBg.DOSizeDelta(targetBgSizeDelta, expandDuration).SetEase(expandEasing);

            if (i <= 0)
            {
                continue;
            }

            var btnRect = menuButtons[i].GetComponent<RectTransform>();
            var targetBtnPos =  buttonOriginPos + buttonSpacing * i;

            btnRect.DOAnchorPos(targetBtnPos, expandDuration).SetEase(expandEasing).OnComplete(() =>
            {
                EnableButtons(true);
                mainButton.interactable = true;
                canvasGroup.interactable = true;
            });
        }

        SetMainIcon(true);
        canvasGroup.DOFade(1, fadeInDuration);
    }

    private void CollapseMenu()
    {
        mainButton.interactable = false;
        EnableButtons(false);
        for (var i = 0; i < menuButtons.Length; i++)
        {
            if (i <= 0)
            {
                continue;
            }
            var btnRect = menuButtons[i].GetComponent<RectTransform>();
            rectMenuBg.DOSizeDelta(menuBgOrigSizeDelta, expandDuration).SetEase(collapseEasing);
            btnRect.DOAnchorPos(buttonOriginPos, expandDuration).SetEase(collapseEasing).OnComplete(() =>
            { 
                mainButton.interactable = true;
                canvasGroup.interactable = false;
                ShowHideButtons(false);
            });
        }
        canvasGroup.DOFade(0, fadeOutDuration);
        SetMainIcon(false);
    }

    private void OnMenuToggled()
    {
        isOpen = !isOpen;
        if (isOpen)
        {
            ExpandMenu();
            return;
        }
        CollapseMenu();
    }

    private void Start()
    {
        ShowHideButtons(false);
        EnableButtons(false);
        buttonOriginPos = menuButtons[0].GetComponent<RectTransform>().anchoredPosition;
        menuBgOrigSizeDelta = rectMenuBg.sizeDelta;
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
    }

    private void OnEnable()
    {
        mainButton.onClick.AddListener(OnMenuToggled);
    }
    
    private void OnDisable()
    {
        mainButton.onClick.RemoveListener(OnMenuToggled);
    }
}
