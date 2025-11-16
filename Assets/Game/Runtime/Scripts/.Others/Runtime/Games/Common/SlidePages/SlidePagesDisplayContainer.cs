using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kumu.Kulitan.Common
{
    [Serializable]
    public class SlidePagesDisplayContainer : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;

        [SerializeField] private Image displayImage;

        [SerializeField] private TMP_Text displayText;

        public float Alpha
        {
            get => canvasGroup.alpha;
            set => canvasGroup.alpha = Mathf.Clamp01(value);
        }

        public void SetData(string text, Sprite sprite)
        {
            displayImage.sprite = sprite;
            displayText.text = text;
        }
    }
}
