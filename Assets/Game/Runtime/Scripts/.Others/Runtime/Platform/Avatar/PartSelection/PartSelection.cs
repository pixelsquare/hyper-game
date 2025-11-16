using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Kumu.Kulitan.Avatar
{
    [Serializable]
    public class PartSelectionIconData
    {
        [SerializeField] private Sprite iconSprite;
        [SerializeField] private Sprite iconDeselectedSprite;
        [SerializeField] private AvatarItemType itemType;
        [SerializeField] private AvatarCameraViewType cameraViewType;
        [SerializeField] private bool hasColorPicker = false;
        [SerializeField] private Color activeColor = Color.white;
        [SerializeField] private Color inactiveColor = Color.white;

        public Sprite IconSprite => iconSprite;
        public Sprite IconDeselectedSprite => iconDeselectedSprite;
        public AvatarItemType ItemType => itemType;
        public AvatarCameraViewType CameraViewType => cameraViewType;
        public bool HasColorPicker => hasColorPicker;

        public Color ActiveColor => activeColor;
        public Color InactiveColor => inactiveColor;
    }

    public class PartSelection : MonoBehaviour
    {
        [SerializeField] private PartSelectionScriptableObject activeCollection;
        [SerializeField] private AvatarItemType activeItemType;

        [Header("Avatar System references")]
        [SerializeField] private PartSelectionIcon activePartIcon;
        [SerializeField] private SubPartSelection subPartSelection;
        [SerializeField] private AvatarCameraController cameraViewController;
        [SerializeField] private ContentSizeFitter contentContainer;
        [SerializeField] private ToggleGroup toggleGroup;

        [Header("Part Icons Data")]
        [SerializeField] private List<PartSelectionIcon> icons;
        [SerializeField] private GameObject iconPrefab;
        [SerializeField] private RectTransform iconContainer;

        [SerializeField] private UnityEvent<AvatarItemType> onPartSelected;
        [SerializeField] private UnityEvent<bool> onColorableSelected;

        public PartSelectionIcon ActivePartIcon => activePartIcon;
        public AvatarItemType ActiveItemType => activeItemType;
        public PartSelectionScriptableObject ActiveCollection => activeCollection;
        public List<PartSelectionIconData> ActiveCollectionData => activeCollection.IconsData;

        /// <summary>
        /// Initializes all part selection icons.
        /// Called once items are loaded.
        /// </summary>
        public void InitializeParts()
        {
            GenerateIconsFromCollection(activeCollection);
        }

        public void SelectIcon(PartSelectionIcon _selectedIcon)
        {
            activePartIcon = _selectedIcon;
            activeItemType = _selectedIcon.ItemType;
            
            SwtichAvatarViewType();
            RefreshSubTypesDisplay();
            
            onPartSelected.Invoke(activeItemType);
            onColorableSelected.Invoke(_selectedIcon.HasColorPicker);
        }

        /// <summary>
        /// Checks and controls SubTypes display.
        /// </summary>
        public void RefreshSubTypesDisplay()
        {
            if(activePartIcon.ItemType > 0) // if any item type or item subtype is flagged true
            {
                subPartSelection.Open();
            }
            else
            {
                subPartSelection.Close();
            }

            subPartSelection.RefreshList(activePartIcon);
        }

        public void SwtichAvatarViewType()
        {
            if(cameraViewController.CurrentView != activePartIcon.CameraViewType)
            {
                cameraViewController.SwitchView(activePartIcon.CameraViewType);
            }
        }

        public void UpdateCollection(PartSelectionScriptableObject collection)
        {
            activeCollection = collection;
            GenerateIconsFromCollection(activeCollection);
        }

        /// <summary>
        /// Generates icons element displays from given collection.
        /// </summary>
        public void GenerateIconsFromCollection(PartSelectionScriptableObject collection)
        {
            ClearIcons();

            var data = new List<PartSelectionIconData>();
            data.AddRange(collection.IconsData);

            //GenerateIcon(collection.NewItemsIconData, PartMode.New); // disabled for POC
            GenerateIcon(collection.OwnedItemsIconData, PartMode.Owned);

            foreach (var iconData in data)
            {
                GenerateIcon(iconData, PartMode.Normal);
            }
        }

        private void GenerateIcon(PartSelectionIconData iconData, PartMode partMode)
        {
            var obj = Instantiate(iconPrefab, iconContainer);
            var objIcon = obj.GetComponent<PartSelectionIcon>();
            objIcon.Initialize(iconData, this, partMode, toggleGroup, contentContainer);
            icons.Add(objIcon);
        }

        /// <summary>
        /// Clears icons list.
        /// </summary>
        private void ClearIcons()
        {
            foreach (RectTransform child in iconContainer.transform)
            {
                Destroy(child.gameObject);
            }
            icons.Clear();
        }

        public enum PartMode
        {
            Normal,
            New,
            Owned
        }
    }
}
