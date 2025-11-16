using UnityEngine;

namespace Kumu.Kulitan.Common
{
    public class MultitouchToggle : MonoBehaviour
    {
        [SerializeField] private bool isOn;
        [SerializeField] private bool toggleOnStart;

        public void Toggle()
        {
            Toggle(isOn);
        }

        public void Toggle(bool isOn)
        {
            this.isOn = isOn;
            Input.multiTouchEnabled = isOn;
        }
        
        private void Start()
        {
            if (toggleOnStart)
            {
                Toggle();
            }
        }
    }
}
