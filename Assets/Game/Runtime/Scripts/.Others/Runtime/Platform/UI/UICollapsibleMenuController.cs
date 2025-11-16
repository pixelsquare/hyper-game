using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Kumu.Kulitan.UI
{
    public class UICollapsibleMenuController : MonoBehaviour
    {
        [SerializeField] private RectTransform rectMenuBg;
        [SerializeField] private Button[] menuButtons;
        [SerializeField] private Vector2 buttonSpacing = new(0f, 150f);

        [Header("Animation Settings")] [SerializeField]
        private float expandDuration = 0.5f;

        [SerializeField] private float iconRotationDuration = 0.5f;
        [SerializeField] private Ease expandEasing = Ease.OutBack;
        [SerializeField] private Ease collapseEasing = Ease.InBack;

        [Header("Main Button Settings")] [SerializeField]
        private Button mainButton;

        [SerializeField] private Image imgClosedIcon;
        [SerializeField] private Image imgOpenedIcon;
        [SerializeField] private RectTransform rectClosedIcon;
        [SerializeField] private RectTransform rectOpenedIcon;

        private bool isOpen = false;
        private RectTransform mainButtonRect;
        private RectTransform[] menuButtonsRect;
        private Vector2 menuBgOrigSizeDelta;

        public void OnMenuToggled()
        {
            isOpen = !isOpen;
            if (isOpen)
            {
                ExpandMenu();
                return;
            }

            CollapseMenu();
        }

        public void CollapseMenu()
        {
            mainButton.interactable = false;
            EnableButtons(false);
            for (var i = 0; i < menuButtons.Length; i++)
            {
                menuButtonsRect[i].DOAnchorPos(mainButtonRect.anchoredPosition, expandDuration).SetEase(collapseEasing)
                    .OnComplete(() =>
                    {
                        mainButton.interactable = true;
                        ShowHideButtons(false);
                    });
                rectMenuBg.DOSizeDelta(menuBgOrigSizeDelta, expandDuration).SetEase(collapseEasing);
            }

            SetMainIcon(false);
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

        private void SetMainIcon(bool toExpandedIcon)
        {
            imgOpenedIcon.DOFade(toExpandedIcon ? 1 : 0, iconRotationDuration);
            imgClosedIcon.DOFade(toExpandedIcon ? 0 : 1, iconRotationDuration);
            rectOpenedIcon.DORotate(toExpandedIcon ? Vector3.forward : Vector3.back * 180f, iconRotationDuration);
        }

        private void ExpandMenu()
        {
            ShowHideButtons(true);
            EnableButtons(false);
            mainButton.interactable = false;
            for (var i = 0; i < menuButtons.Length; i++)
            {
                var targetBtnPos = mainButtonRect.anchoredPosition + buttonSpacing * (i + 1);
                var targetBgSizeDelta = rectMenuBg.sizeDelta + buttonSpacing * (i + 1);
                menuButtonsRect[i].DOAnchorPos(targetBtnPos, expandDuration).SetEase(expandEasing).OnComplete(() =>
                {
                    mainButton.interactable = true;
                    EnableButtons(true);
                });
                rectMenuBg.DOSizeDelta(targetBgSizeDelta, expandDuration).SetEase(expandEasing);
            }

            SetMainIcon(true);

        }

        private void Init()
        {
            var buttons = new RectTransform[menuButtons.Length];
            for (int i = 0; i < menuButtons.Length; i++)
            {
                buttons[i] = menuButtons[i].GetComponent<RectTransform>();
            }
            menuButtonsRect = buttons;
        }

        private void Start()
        {
            ShowHideButtons(false);
            mainButtonRect = mainButton.GetComponent<RectTransform>();
            menuBgOrigSizeDelta = rectMenuBg.sizeDelta;
            Init();
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
}
