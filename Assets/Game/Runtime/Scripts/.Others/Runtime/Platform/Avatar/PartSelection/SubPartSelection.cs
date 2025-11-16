using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Kumu.Kulitan.Avatar
{
    /// <summary>
    /// Handles all data and display controller 
    /// for the item subtypes/subcategories.
    /// </summary>
    public class SubPartSelection : MonoBehaviour
    {
        [SerializeField] private RectTransform iconContainer;
        [SerializeField] private AvatarPanelResizer panelResizer;
        [SerializeField] private GameObject iconPrefab;
        [SerializeField] private ItemSelection itemSelection;
        [SerializeField] private ToggleGroup toggleGroup;
        [SerializeField] private bool isOpen = false;
        [SerializeField] private List<SubPartIcon> icons = new List<SubPartIcon>();
        [SerializeField] private ItemCategorySelector itemCategorySelector;

        private PartSelectionIcon activePartData;

        public void RefreshList(PartSelectionIcon partSelection)
        {
            activePartData = partSelection;

            switch (activePartData.PartMode)
            {
                case PartSelection.PartMode.New:
                    GenerateItemTypeIcons(partSelection.ItemType);
                    break;
                case PartSelection.PartMode.Owned:
                    GenerateItemTypeIcons(partSelection.ItemType);
                    break;
                default:
                    GenerateItemTagIcons(partSelection.ItemType);
                    break;
            }
        }

        public void Open()
        {
            if (isOpen)
            {
                return;
            }
            isOpen = true;
            panelResizer.Expand();
        }

        public void Close()
        {
            if (!isOpen)
            {
                return;
            }
            isOpen = false;
            panelResizer.Contract();
        }

        public void ToggleIcon(SubPartIcon subPartIcon, bool isOn)
        {
            if (!isOn)
            {
                return;
            }

            FilterSubparts(subPartIcon);
        }

        private void FilterSubparts(SubPartIcon subPartIcon)
        {
            switch (activePartData.PartMode)
            {
                case PartSelection.PartMode.New:
                    itemSelection.FilterItemsByNew(subPartIcon.ItemType);
                    break;
                case PartSelection.PartMode.Owned:
                    itemSelection.FilterItemsByOwned(subPartIcon.ItemType);
                    break;
                default:
                    if (subPartIcon.ItemTagConfig == null)
                    {
                        itemSelection.FilterItems(subPartIcon.ItemType);
                    }
                    else
                    {
                        itemSelection.FilterItemsByItemCategory(subPartIcon.ItemTagConfig);
                    }
                    break;
            }
        }

        /// <summary>
        /// Generates icons element displays based on Avatar Item Types
        /// </summary>
        /// <param name="allTypes">Bitmask flags for all Avatar item types for this category</param>
        private void GenerateItemTypeIcons(AvatarItemType allTypes)
        {
            ClearIcons();

            var allIcon = AddIconPrefab();
            allIcon.Initialize(allTypes, this, toggleGroup, "All");
            icons.Add(allIcon);

            foreach (AvatarItemType flag in Enum.GetValues(typeof(AvatarItemType)))
            {
                switch(flag)
                {
                    case AvatarItemType.None:
                        continue;
                    case AvatarItemType.AllClothing:
                        continue;
                    case AvatarItemType.AllAccessory:
                        continue;
                    case AvatarItemType.AllBody:
                        continue;
                }

                if ((allTypes & flag) > 0) // Enum.HasFlag(Enum) includes None ItemType; we instead manually check bits
                {
                    var objIcon = AddIconPrefab();
                    objIcon.Initialize(flag, this, toggleGroup);
                    icons.Add(objIcon);
                }
            }

            if (icons.Count > 0)
            {
                icons[0].Toggle(true);
                icons[0].Notify(true);
            }
        }

        private void GenerateItemTagIcons(AvatarItemType itemType)
        {
            ClearIcons();

            var allIcon = AddIconPrefab();
            allIcon.Initialize(itemType, this, toggleGroup, "All");
            icons.Add(allIcon);

            var configs = itemCategorySelector.GetConfigsOfItemType(itemType).ToArray();

            foreach (var config in configs)
            {
                var objIcon = AddIconPrefab();
                objIcon.Initialize(config, this, toggleGroup);
                icons.Add(objIcon);
            }

            if (icons.Count > 0)
            {
                icons[0].Toggle(true);
                icons[0].Notify(true);
            }
        }

        private SubPartIcon AddIconPrefab()
        {
            GameObject obj = Instantiate(iconPrefab, iconContainer);
            SubPartIcon objIcon = obj.GetComponent<SubPartIcon>();
            return objIcon;
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
    }
}
