using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Kumu.Kulitan.Avatar
{
    [RequireComponent(typeof(Image))]
    public class PartSelectionIcon : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image selectedIcon;
        [SerializeField] private Image deselectedIcon;
        [SerializeField] private AvatarItemType itemType;
        [SerializeField] private AvatarCameraViewType cameraViewType;
        [SerializeField] private bool hasColorPicker = false;
        [SerializeField] private Toggle toggle;
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private Vector2 selectedSize;
        [SerializeField] private UnityEvent onClick;

        private PartSelection partSelection;
        private PartSelection.PartMode partMode;
        private ContentSizeFitter contentContainer;
        private Vector2 deselectedSize;

        public AvatarItemType ItemType => itemType;
        public AvatarCameraViewType CameraViewType => cameraViewType;
        public bool HasColorPicker => hasColorPicker;

        public PartSelection.PartMode PartMode => partMode;

        public void Initialize(PartSelectionIconData _data, PartSelection _partSelection, PartSelection.PartMode partMode, ToggleGroup toggleGroup, ContentSizeFitter contentContainer)
        {
            selectedIcon.sprite = _data.IconSprite;
            deselectedIcon.sprite = _data.IconDeselectedSprite;
            selectedIcon.color = _data.ActiveColor;
            deselectedIcon.color = _data.InactiveColor;

            deselectedSize = rectTransform.rect.size;
            itemType = _data.ItemType;
            cameraViewType = _data.CameraViewType;
            hasColorPicker = _data.HasColorPicker;
            partSelection = _partSelection;
            toggle.group = toggleGroup;
            
            toggle.onValueChanged.AddListener(Toggle);
            toggle.onValueChanged.AddListener(ToggleRectSize);

            this.partMode = partMode;
            this.contentContainer = contentContainer;
        }

        private void Toggle(bool isOn)
        {
            if (isOn)
            {
                partSelection.SelectIcon(this);
            }
        }

        private void ToggleRectSize(bool isOn)
        {
            if (isOn)
            {
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, selectedSize.x);
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, selectedSize.y);
            }
            else
            {
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, deselectedSize.x);
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, deselectedSize.y);
            }

            contentContainer.enabled = false;
            Canvas.ForceUpdateCanvases();
            contentContainer.enabled = true;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            onClick?.Invoke();
        }
    }
}
