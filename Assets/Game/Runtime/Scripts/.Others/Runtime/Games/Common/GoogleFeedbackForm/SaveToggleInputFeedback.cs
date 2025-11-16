using UnityEngine;
using UnityEngine.UI;

namespace Kumu.Kulitan.Common
{
    public class SaveToggleInputFeedback : MonoBehaviour
    {
        [SerializeField] private Toggle[] toggles;
        private ToggleGroup toggleGroup;

        public string toggleValue;

        private void Start()
        {
            if (toggleGroup == null)
            {
                toggleGroup = GetComponent<ToggleGroup>();
            }

            foreach (var toggle in toggles)
            {
                toggle.onValueChanged.AddListener(delegate {OnToggleValueChanged(toggle);});
            }
        }

        private void OnToggleValueChanged(Toggle value)
        {
            foreach (var toggle in toggles)
            {
                if (toggle.isOn)
                {
                    toggleValue = toggle.name;
                    break;
                }

                toggleValue = null;
            }
        }
    }
}
