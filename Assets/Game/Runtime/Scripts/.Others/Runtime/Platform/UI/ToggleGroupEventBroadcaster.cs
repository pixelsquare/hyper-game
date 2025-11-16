using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Kumu.Kulitan.UI
{
    [RequireComponent(typeof(ToggleGroup))]
    public class ToggleGroupEventBroadcaster : MonoBehaviour
    {
        [Serializable]
        public class ToggleEvent : UnityEvent<Toggle> { }

        [SerializeField] private ToggleEvent OnActiveToggleChanged;

        private ToggleGroup toggleGroup;
        private Toggle[] toggles;

        private Toggle ActiveToggle => toggleGroup.ActiveToggles().FirstOrDefault();

        private void HandleToggleValueChanged(bool isOn)
        {
            if (isOn)
            {
                OnActiveToggleChanged?.Invoke(ActiveToggle);
            }
        }

        private void Awake()
        {
            toggleGroup = GetComponent<ToggleGroup>();
            toggles = GetComponentsInChildren<Toggle>();
        }

        private void OnEnable()
        {
            foreach (var toggle in toggles)
            {
                toggle.group = toggleGroup;
                toggle.onValueChanged.AddListener(HandleToggleValueChanged);
            }
        }

        private void OnDisable()
        {
            foreach (var toggle in toggles)
            {
                toggle.onValueChanged.RemoveListener(HandleToggleValueChanged);
            }
        }

        private void Start()
        {
            OnActiveToggleChanged?.Invoke(ActiveToggle);
        }
    }
}
