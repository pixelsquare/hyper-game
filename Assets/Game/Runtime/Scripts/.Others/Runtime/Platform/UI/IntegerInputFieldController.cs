using UnityEngine;
using TMPro;

namespace Kumu.Kulitan.UI
{
    /// <summary>
    /// Used to control number/integer input fields
    /// with plus/minus controller buttons.
    /// Must be attached to an object with a TextMeshPro - InputField component
    /// </summary>
    [RequireComponent(typeof(TMP_InputField))]
    public class IntegerInputFieldController : MonoBehaviour
    {
        [SerializeField] private int maxValue = 8;
        [SerializeField] private int minValue = 1;

        [SerializeField] private TMP_InputField txtCount;

        private int curValue = 1;

        public void IncreaseValue()
        {
            curValue++;
            Validate();
        }

        public void DecreaseValue()
        {
            curValue--;
            Validate();
        }

        public void OnValueChanged(string txt)
        {
            curValue = int.Parse(txt);
            Validate();
        }

        private void Validate()
        {
            curValue = Mathf.Clamp(curValue, minValue, maxValue);
            txtCount.text = curValue.ToString();
        }
    }
}
