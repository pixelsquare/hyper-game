using System.Collections.Generic;
using Kumu.Kulitan.Common;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Kumu.Kulitan.Avatar
{
    /// <summary>
    /// Handles the Color Palette UI
    /// </summary>
    public class AvatarColorPicker : MonoBehaviour
    {
        // swatches reference: https://www.rapidtables.com/web/color/RGB_Color.html
        [SerializeField] private Color activeColor = Color.white;
        [SerializeField] private ToggleGroup toggleGroup;
        [SerializeField] private SwatchTable swatchTable;

        [SerializeField] private AvatarColor iconPrefab;
        [SerializeField] private RectTransform iconContainer;

        [SerializeField] private UnityEvent<bool> onColorPickerShow;
        [SerializeField] private UnityEvent<bool> onColorSwatchShow;
        [SerializeField] private UnityEvent<Color> onColorPicked;

        private Dictionary<Color, AvatarColor> colorTable = new();
        private AvatarSwatchScriptableObject activeSwatch;
        private ObjectPool<AvatarColor> pool;
        private bool isInteracted;

        public Color ActiveColor => activeColor;
        public bool IsInteracted => isInteracted;

        public void ShowHideColorSwatch(bool toShow)
        {
            onColorSwatchShow.Invoke(toShow);
        }

        /// <summary>
        /// Used by FSM
        /// </summary>
        public void ShowHideColorPicker(bool isShown)
        {
            onColorPickerShow?.Invoke(isShown);
        }

        public void SelectColor(AvatarColor swatch)
        {
            activeColor = swatch.myColor;
            isInteracted = true;
            onColorPicked?.Invoke(activeColor);
        }

        public void TryLoadSwatch(AvatarItemType itemType)
        {
            isInteracted = false;
            
            ClearSwatch();

            if (swatchTable.TryGetPalette(itemType, out var swatch))
            {
                activeSwatch = swatch;
                LoadSwatch();
            }
            else
            {
                activeSwatch = null;
            }
        }

        /// <summary>
        /// Used by FSM
        /// </summary>
        public bool TryPickActiveColor(AvatarItemConfig itemConfig)
        {
            if (itemConfig.State.hasColor
                && colorTable.TryGetValue(itemConfig.State.Color, out var avatarColor))
            {
                activeColor = itemConfig.State.Color;
                avatarColor.NotifySwatch(true);
                return true;
            }

            return false;
        }

        public bool TryPickColor(Color color)
        {
            if (colorTable.TryGetValue(activeColor, out var activeIcon))
            {
                activeIcon.NotifySwatch(false);
            }

            if (colorTable.TryGetValue(color, out var avatarColor))
            {
                avatarColor.NotifySwatch(true);
                activeColor = color;
                return true;
            }

            return false;
        }

        private void LoadSwatch()
        {
            foreach (var _color in activeSwatch.Colors)
            {
                AddColor(_color);
            }
        }

        private void ClearSwatch()
        {
            foreach (var colorPair in colorTable)
            {
                var avatarColor = colorPair.Value;
                avatarColor.Deinitialize();
                pool.Return(avatarColor);
            }

            colorTable.Clear();
        }

        private AvatarColor AddColor(Color color)
        {
            var avatarColor = pool.Get();
            avatarColor.Initialize(color, this, toggleGroup);
            avatarColor.transform.SetAsLastSibling();
            colorTable.Add(color, avatarColor);

            return avatarColor;
        }

        private AvatarColor CreateIcon()
        {
            var avatarColor = Instantiate(iconPrefab, iconContainer);
            return avatarColor;
        }

        private void Start()
        {
            pool = new ObjectPool<AvatarColor>(CreateIcon);
        }
    }
}
