using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Kumu.Kulitan.UI
{
    /// <summary>
    /// Handles spawning and moving visual representations of currency objects from one position to another.
    /// </summary>
    public class CurrencyTweener : MonoBehaviour
    {
        [SerializeField] private AnimationCurve curve = AnimationCurve.EaseInOut(0f, 0f, .2f, 1f);
        [SerializeField] private CurrencyEvent onCurrencyReached;
        [SerializeField] private Camera uiCamera;
        [SerializeField] private Transform origin;
        [SerializeField] private Transform target;
        [SerializeField] private CurrencyType currencyType;
        [SerializeField] private Data[] data;

        private Coroutine spawnCoroutine;

        public enum CurrencyType
        {
            Coin = 1,
            Diamond = 2,
        }

        /// <summary>
        /// Spawn currency Objects. These objects are animated to move from <see cref="CurrencyTweener.origin"/> to <see cref="CurrencyTweener.target"/> position.
        /// </summary>
        /// <param name="amount">Total currency value to spawn for.</param>
        public void SpawnCurrency(int amount)
        {
            if (spawnCoroutine == null)
            {
                spawnCoroutine = StartCoroutine(Spawn(amount));
            }
        }

        /// <summary>
        /// Calculates how much of a specific currency type should be spawned.
        /// </summary>
        /// <param name="value">Total currency value to calculate currency instances for.</param>
        /// <returns><see cref="CurrencyTweener.Data"/> array.</returns>
        private Data[] GetCurrencyAmount(int value)
        {
            var currencySpawnData = new Data[data.Length];
            var currentAmount = value;

            for (int i = 0; i < currencySpawnData.Length; i++)
            {
                currencySpawnData[i] = new Data();
                var current = currencySpawnData[i];
                current.prefab = data[i].prefab;
                current.value = currentAmount / data[i].value;
                Debug.Log($"{current.prefab}: {current.value}");
                currentAmount %= data[i].value;
            }

            return currencySpawnData;
        }

        /// <summary>
        /// Spawns currency Object instances.
        /// The amount is based on their value, configured by the <see cref="CurrencyTweener.data"/> field.
        /// </summary>
        /// <param name="value">Total value of the currency to be spawned.</param>
        private IEnumerator Spawn(int value)
        {
            var currencySpawnData = GetCurrencyAmount(value);

            for (int i = 0; i < currencySpawnData.Length; i++)
            {
                var spawnData = currencySpawnData[i];

                for (int j = 0; j < spawnData.value; j++)
                {
                    var instance = Instantiate(spawnData.prefab);
                    StartCoroutine(Tween(instance.transform, data[i].value));
                    yield return new WaitForSeconds(.1f);
                }
            }


            spawnCoroutine = null;
        }

        /// <summary>
        /// Moves a transform from one point to another.
        /// </summary>
        /// <param name="transform">The Transform of the currency Object to move.</param>
        /// <param name="value">The value of the currency Object.</param>
        private IEnumerator Tween(Transform transform, int value)
        {
            var firstKey = curve.keys[0];
            var lastkey = curve.keys[curve.keys.Length - 1];
            var duration = firstKey.time;
            var endTime = lastkey.time;
            var progress = 0f;

            do
            {
                duration += Time.deltaTime;
                progress = curve.Evaluate(duration);
                transform.position = Vector3.Lerp(origin.position, target.position, progress);
                yield return null;
            }
            while (duration < endTime);

            onCurrencyReached.Invoke(value);

            Destroy(transform.gameObject);
        }

        [Serializable]
        private class Data
        {
            public GameObject prefab;
            public int value;
        }

        [Serializable]
        private class CurrencyEvent : UnityEvent<int> { }
    }
}