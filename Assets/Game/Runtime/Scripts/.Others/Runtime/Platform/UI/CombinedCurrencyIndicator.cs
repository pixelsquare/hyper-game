using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Kumu.Kulitan.UI
{
    /// <summary>
    /// UI component that handles displaying both coin and diamond currency.
    /// </summary>
    public class CombinedCurrencyIndicator : MonoBehaviour
    {
        [SerializeField] private bool hasLimit = false;
        [SerializeField] private int coinLimit = 9999999;
        [SerializeField] private int diamondLimit = 999;
        [SerializeField] private CurrencyDisplayState currenciesToDisplay = CurrencyDisplayState.BothDisplay;
        
        [SerializeField] private UnityEvent onCurveStart;
        [SerializeField] private UnityEvent onCurveEnd;
        [SerializeField] private AnimationCurve updateCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        [SerializeField] private TextMeshProUGUI label;

        private int coinValue;
        private int currentCoinValue;
        private int diamondValue;
        private int currentDiamondValue;
        
        private Coroutine updateLabelTask;

        /// <summary>
        /// Updates the currency value instantly.
        /// </summary>
        /// <param name="newCoinValue">The new value of the coin value.</param>
        /// <param name="newDiamondValue">The new value of the diamond value.</param>
        public void SetValue(int newCoinValue, int newDiamondValue)
        {
            coinValue = newCoinValue;
            diamondValue = newDiamondValue;
            SetLabel(newCoinValue, newDiamondValue);
        }

        /// <summary>
        /// Eases the currency value.
        /// </summary>
        /// <param name="newCoinValue">The new value of the coin value.</param>
        /// <param name="newDiamondValue">The new value of the diamond value.</param>
        public void UpdateValue(int newCoinValue, int newDiamondValue)
        {
            if (updateLabelTask != null)
            {
                StopCoroutine(updateLabelTask);
                coinValue = newCoinValue;
                diamondValue = newDiamondValue;
                updateLabelTask = null;
            }
            updateLabelTask = StartCoroutine(UpdateLabelTask(newCoinValue, newDiamondValue));
        }

        /// <summary>
        /// Adds to the current currency value and updates the display instantly.
        /// </summary>
        /// <param name="coinAmount">Amount to add to the current coin value.</param>
        /// <param name="diamondAmount">Amount to add to the current diamond value.</param>
        public void AddValue(int coinAmount, int diamondAmount)
        {
            coinValue += coinAmount;
            diamondValue += diamondAmount;
            SetLabel(coinValue, diamondValue);
        }

        /// <summary>
        /// Sets the UI element label of the currency display.
        /// </summary>
        /// <param name="newCoinValue"></param>
        /// <param name="newDiamondValue"></param>
        private void SetLabel(int newCoinValue, int newDiamondValue)
        {
            if (hasLimit)
            {
                newCoinValue = Mathf.Clamp(newCoinValue, 0, coinLimit);
                newDiamondValue = Mathf.Clamp(newDiamondValue, 0, diamondLimit);
            }

            var textToDisplay = currenciesToDisplay switch
            {
                CurrencyDisplayState.CoinDisplay => $"<sprite=1> {newCoinValue}",
                CurrencyDisplayState.DiamondDisplay => $"<sprite=0> {newDiamondValue}",
                _ => $"<sprite=0> {newDiamondValue}  <sprite=1> {newCoinValue}"
            };

            label.text = textToDisplay;
        }

        private IEnumerator UpdateLabelTask(int newCoinValue, int newDiamondValue)
        {
            onCurveStart?.Invoke();
            var firstKey = updateCurve.keys[0];
            var lastKey = updateCurve.keys[updateCurve.length - 1];
            var duration = firstKey.time;
            do
            {
                duration += Time.deltaTime;
                var progress = updateCurve.Evaluate(duration);
                currentCoinValue = (int)Mathf.Lerp(coinValue, newCoinValue, progress);
                currentDiamondValue = (int)Mathf.Lerp(diamondValue, newDiamondValue, progress);
                SetLabel(currentCoinValue, currentDiamondValue);
                yield return null;
            } while (duration < lastKey.time);
            coinValue = currentCoinValue;
            diamondValue = currentDiamondValue;
            onCurveEnd?.Invoke();
        }

        private enum CurrencyDisplayState
        {
            DiamondDisplay,
            CoinDisplay,
            BothDisplay
        }
    }
}
