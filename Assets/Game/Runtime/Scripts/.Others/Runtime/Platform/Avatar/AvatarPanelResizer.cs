using System.Collections;
using UnityEngine;

namespace Kumu.Kulitan.Avatar
{
    public class AvatarPanelResizer : MonoBehaviour
    {
        [SerializeField] private RectTransform subpartPanel;
        [SerializeField] private RectTransform itemsPanel;

        [SerializeField] private float animationDuration = 20f;
        [SerializeField] private float panelMoveAmount = -50f;

        private Coroutine subPartCoroutine;
        private Coroutine itemsCoroutine;

        public void Expand()
        {
            MoveSubPartPanel(panelMoveAmount);
            MoveItemsPanel(panelMoveAmount);
        }

        public void Contract()
        {
            MoveSubPartPanel(0f);
            MoveItemsPanel(0f);
        }

        private void MoveItemsPanel(float targetTop)
        {
            if(itemsCoroutine != null)
            {
                StopCoroutine(itemsCoroutine);
                itemsCoroutine = null;
            }
            // offsetMax changes "top" rect value
            var startPosition = itemsPanel.offsetMax;
            var targetPosition = new Vector2(itemsPanel.offsetMax.x, targetTop);
            InvokeItemsCoroutine(startPosition, targetPosition);
        }

        private void InvokeItemsCoroutine(Vector2 startPosition, Vector2 targetPosition)
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }
            
            itemsCoroutine = StartCoroutine(LerpItemsPanel(itemsPanel, startPosition, targetPosition, animationDuration));
        }

        private void MoveSubPartPanel(float targetBottom)
        {
            if(subPartCoroutine != null)
            {
                StopCoroutine(subPartCoroutine);
                subPartCoroutine = null;
            }
            var startPosition = subpartPanel.offsetMin;
            var targetPosition = new Vector2(subpartPanel.offsetMin.x, targetBottom);
            InvokeSubPartCoroutine(startPosition, targetPosition);
        }

        private void InvokeSubPartCoroutine(Vector2 startPosition, Vector2 targetPosition)
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }
            
            subPartCoroutine = StartCoroutine(LerpSubPartPanel(subpartPanel, startPosition, targetPosition, animationDuration));
        }

        private IEnumerator LerpItemsPanel(RectTransform rect, Vector2 _startPosition, Vector2 _targetPosition, float _duration)
        {
            float time = 0;

            while (time < _duration)
            {
                rect.offsetMax = Vector2.Lerp(_startPosition, _targetPosition, time);
                time += Time.deltaTime;
                yield return null;
            }

            rect.offsetMax = _targetPosition;
        }

        private IEnumerator LerpSubPartPanel(RectTransform rect, Vector2 _startPosition, Vector2 _targetPosition, float _duration)
        {
            float time = 0;

            while (time < _duration)
            {
                rect.offsetMin = Vector2.Lerp(_startPosition, _targetPosition, time);
                time += Time.deltaTime;
                yield return null;
            }

            rect.offsetMin = _targetPosition;
        }
    }
}
