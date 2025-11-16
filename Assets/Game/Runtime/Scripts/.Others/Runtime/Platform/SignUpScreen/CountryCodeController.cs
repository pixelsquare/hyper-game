using System.Collections.Generic;
using Kumu.Kulitan.Backend;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Kumu.Kulitan.UI
{
    public class CountryCodeController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private TMP_Dropdown dropdown;
        public UnityEvent<int> onSelectedUpdated;
        
        public void Initialize()
        {
            var newOptionsData = new List<TMP_Dropdown.OptionData>();
            foreach (var cc in MobileNumberUtil.GetCountryCodes())
            {
                var newData = new TMP_Dropdown.OptionData
                {
                    text = $"{cc.alpha} (+{cc.code})"
                };
                newOptionsData.Add(newData);
            }
            dropdown.ClearOptions();
            dropdown.AddOptions(newOptionsData);
            UpdateLabelWithCurrentCode(0);
        }

        public void UpdateLabelWithCurrentCode(int idx)
        {
            label.text = $"+{MobileNumberUtil.GetCountryCodes()[idx].code}";
        }

        private void OnValueChanged(int value)
        {
            UpdateLabelWithCurrentCode(value);
            onSelectedUpdated?.Invoke(value);
        }
        
        private void OnEnable()
        {
            dropdown.onValueChanged.AddListener(OnValueChanged);
        }

        private void OnDisable()
        {
            dropdown.onValueChanged.RemoveListener(OnValueChanged);
        }
    }
}
