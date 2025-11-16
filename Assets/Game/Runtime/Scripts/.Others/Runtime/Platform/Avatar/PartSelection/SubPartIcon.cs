using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Kumu.Kulitan.Avatar
{
    public class SubPartIcon : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private TMP_Text label;
        [SerializeField] private TMP_Text selectedLabel;
        [SerializeField] private Toggle toggle;
        [SerializeField] private UnityEvent onClick;

        private SubPartSelection subPartSelection;
        private AvatarItemType itemType;
        private ItemCategoryConfig itemTagConfig;

        public AvatarItemType ItemType => itemType;
        public ItemCategoryConfig ItemTagConfig => itemTagConfig;

        public void Initialize(AvatarItemType itemType, SubPartSelection subPartSelection, ToggleGroup group, string label = null)
        {
            this.itemType = itemType;
            this.subPartSelection = subPartSelection;
            toggle.group = group;
            SetIconText(label ?? itemType.ToString());
        }

        public void Initialize(ItemCategoryConfig itemTagConfig, SubPartSelection subPartSelection, ToggleGroup group, string label = null)
        {
            this.itemTagConfig = itemTagConfig;
            this.subPartSelection = subPartSelection;
            toggle.group = group;
            SetIconText(label ?? itemTagConfig.ItemCategory);
        }

        public void SetIconText(string _name)
        {
            var text = Regex.Replace(_name, "([a-z])([A-Z])", "$1 $2");
            label.text = text;
            selectedLabel.text = text;
        }

        public void Toggle(bool isOn)
        {
            subPartSelection.ToggleIcon(this, isOn);
        }

        public void Notify(bool isOn)
        {
            toggle.SetIsOnWithoutNotify(isOn);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            onClick?.Invoke();
        }
    }
}
