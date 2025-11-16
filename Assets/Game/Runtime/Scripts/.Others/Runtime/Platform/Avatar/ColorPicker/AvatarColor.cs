using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Kumu.Kulitan.Avatar
{
    public class AvatarColor : MonoBehaviour
    {
        [SerializeField] private Color color;
        [SerializeField] private Toggle toggle;
        [SerializeField] private Image myImage;

        [SerializeField] private UnityEvent<bool> onColorToggle;

        public Color myColor => color;

        public bool IsActive => toggle.isOn;

        public void Initialize(Color _color, AvatarColorPicker _colorPicker, ToggleGroup toggleGroup)
        {
            color = _color;
            myImage.color = color;
            
            toggle.group = toggleGroup;
            toggle.onValueChanged.AddListener(isOn => ToggleSwatch(_colorPicker, isOn));
            
            gameObject.SetActive(true);
        }

        public void Deinitialize()
        {
            toggle.group = null;
            toggle.onValueChanged.RemoveAllListeners();

            toggle.SetIsOnWithoutNotify(false);
            gameObject.SetActive(false);
        }

        public void NotifySwatch(bool isOn)
        {
            toggle.SetIsOnWithoutNotify(isOn);
        }

        private void ToggleSwatch(AvatarColorPicker colorPicker, bool isOn)
        {
            if (isOn)
            {
                colorPicker.SelectColor(this);
            }
        }
    }
}
