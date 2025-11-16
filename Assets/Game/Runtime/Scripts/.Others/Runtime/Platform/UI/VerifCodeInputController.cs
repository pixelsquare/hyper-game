using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Kumu.Kulitan.UI
{
    public class VerifCodeInputController : MonoBehaviour
    {
        public UnityAction<string> OnVerifCodeUpdated;

        [SerializeField] private TMP_InputField[] inputs;
        [SerializeField] private Button button;
        [SerializeField] private TMP_InputField fakeInputField;
        private string currentText = "";
        
        public void ClearText()
        {
            foreach (var input in inputs)
            {
                input.text = "";
            }

            fakeInputField.text = "";
        }

        public void DeactivateField()
        {
            fakeInputField.DeactivateInputField();
        }

        private void OnValueChanged(string value)
        {
            if (currentText.Equals(value))
            {
                return;
            }

            for (var i = 0; i < inputs.Length; i++)
            {
                if (i >= value.Length)
                {
                    inputs[i].text = "";
                    continue;
                }
                var str = value[i].ToString();
                inputs[i].text = str;
            }
            currentText = value;

            if (value.Length >= 6)
            {
                SendResult(currentText);
            }
        }

        private void SendResult(string result)
        {
            OnVerifCodeUpdated?.Invoke(result);
        }

        private IEnumerator MoveCursorToEndOfLine()
        {
            yield return new WaitForEndOfFrame();
            fakeInputField.MoveToEndOfLine(false, false);
        }

        private void OnSelected()
        {
            if (fakeInputField.isFocused)
            {
                return;
            }

            fakeInputField.ActivateInputField();
            StartCoroutine(MoveCursorToEndOfLine());
        }

        private void OnEnable()
        {
            fakeInputField.onValueChanged.AddListener(OnValueChanged);
            button.onClick.AddListener(OnSelected);
            fakeInputField.ActivateInputField();
        }

        private void OnDisable()
        {
            fakeInputField.onValueChanged.RemoveListener(OnValueChanged);
            button.onClick.RemoveListener(OnSelected);
        }
    }
}
