using System.Collections;
using UnityEngine;


namespace Kumu.Kulitan.UI
{
    public class UIPanelResizer : MonoBehaviour
    {
        [SerializeField] private RectTransform panelRect;
        [SerializeField] private float targetHeight = 200f;
        [SerializeField] private float originalHeight = 0f;
        [SerializeField] private float animationDuration = 20f;

        private Coroutine resizeCoroutine;

        public void TogglePanel(bool isOn)
        {
            Resize(isOn ? targetHeight : originalHeight);
        }

        public void Resize(float _moveAmount)
        {
            if (resizeCoroutine != null)
            {
                StopCoroutine(resizeCoroutine);
                resizeCoroutine = null;
            }
            Vector2 startPos = panelRect.sizeDelta;
            Vector2 targetPos = new Vector2(panelRect.sizeDelta.x, _moveAmount);
            resizeCoroutine = StartCoroutine(LerpPanel(panelRect, startPos, targetPos, animationDuration));
        }

        private IEnumerator LerpPanel(RectTransform rect, Vector2 _startPos,
            Vector2 _targetPos, float duration)
        {
            float time = 0;

            while (time < duration)
            {
                rect.sizeDelta = Vector2.Lerp(_startPos, _targetPos, time);
                time += Time.deltaTime;
                yield return null;
            }
            rect.sizeDelta = _targetPos;
        }
    }
}
