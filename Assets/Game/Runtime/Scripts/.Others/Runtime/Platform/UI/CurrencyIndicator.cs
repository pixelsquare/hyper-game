using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Kumu.Kulitan.UI
{
    /// <summary>
    /// UI component that handles displaying a currency.
    /// </summary>
    public class CurrencyIndicator : MonoBehaviour
    {
        [SerializeField] private UnityEvent onCurveStart;
        [SerializeField] private UnityEvent onCurveEnd;
        [SerializeField] private AnimationCurve updateCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        [SerializeField] private TextMeshProUGUI label;

        private int value;
        private int currentValue;

        private Coroutine updateLabelTask;

        /// <summary>
        /// Updates the currency value instantly.
        /// </summary>
        /// <param name="newValue">The new value of the currency value.</param>
        public void SetValue(int newValue)
        {
            value = newValue;
            SetLabel(newValue);
        }

        /// <summary>
        /// Eases the currency value.
        /// </summary>
        /// <param name="newValue">The new value of the currency value.</param>
        public void UpdateValue(int newValue)
        {
            if (updateLabelTask != null)
            {
                StopCoroutine(updateLabelTask);
                value = currentValue;
                updateLabelTask = null;
            }
            updateLabelTask = StartCoroutine(UpdateLabelTask(newValue));
        }

        /// <summary>
        /// Adds to the current currency value and updates the display instantly.
        /// </summary>
        /// <param name="amount">Amount to add to the current value.</param>
        public void AddValue(int amount)
        {
            value += amount;
            SetLabel(value);
        }

        /// <summary>
        /// Sets the UI element label of the currency display.
        /// </summary>
        /// <param name="newValue"></param>
        private void SetLabel(int newValue)
        {
            label.text = $"<sprite=12/> {newValue}"; //TODO: generate TMP inline icons for displaying currency icons as text
        }

        private IEnumerator UpdateLabelTask(int newValue)
        {
            onCurveStart?.Invoke();
            var firstKey = updateCurve.keys[0];
            var lastKey = updateCurve.keys[updateCurve.length - 1];
            var duration = firstKey.time;
            var progress = 0f;
            do
            {
                duration += Time.deltaTime;
                progress = updateCurve.Evaluate(duration);
                currentValue = (int)Mathf.Lerp(value, newValue, progress);
                SetLabel(currentValue);
                yield return null;
            } while (duration < lastKey.time);
            value = currentValue;
            onCurveEnd?.Invoke();
        }
    }
}
