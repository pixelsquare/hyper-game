using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Game
{
    public class SlotReel : MonoBehaviour
    {
        [SerializeField] private RectTransform content;
        [SerializeField] private float symbolHeight = 200f;
        [SerializeField] private float scrollDuration = 0.15f;
        [SerializeField] private int symbolCount = 6;

        private List<RectTransform> symbols = new();
        private Dictionary<int, RectTransform> symbolsMap = new();
        private bool isSpinning = false;
        private Tweener spinTween;
        
        private RectTransform targetSymbol;

        private void Start()
        {
            var idx = 0;
            foreach (RectTransform child in content)
            {
                symbols.Add(child);
                symbolsMap.Add(idx, child);
                idx++;
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                Spin();
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                targetSymbol = symbolsMap[0];
                StopAt(0, 1f);
            }
        }

        public void Spin()
        {
            isSpinning = true;
            StartSpinning();
        }

        private void StartSpinning()
        {
            if (!isSpinning)
            {
                return;
            }

            // if (targetSymbol != null)
            // {
            //     var contentOffset = ((RectTransform)content.parent).rect.height / 2f * symbolHeight / 2f;
            //     var targetPosY = -targetSymbol.localPosition.y;// + contentOffset;
            //
            //     Debug.Log($"{content.parent.localPosition.y} | {targetPosY}");
            //     Debug.Log(Mathf.Abs(content.parent.localPosition.y - targetPosY));
            //     if (Mathf.Abs(content.parent.localPosition.y + targetPosY) < 110f)
            //     {
            //         StopAt(-targetPosY / 2f, 1f);
            //         return;
            //     }
            // }

            var startPos = content.anchoredPosition;
            var endPos = startPos - Vector2.up * symbolHeight;
            
            spinTween = content.DOAnchorPos(endPos, scrollDuration)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    RecycleSymbol();
                    StartSpinning();
                })
                .Play();
        }

        private void RecycleSymbol()
        {
            content.anchoredPosition += Vector2.up * symbolHeight;
            
            var top = symbols[0];
            symbols.RemoveAt(0);
            symbols.Add(top);
            top.SetAsLastSibling();
        }

        public void StopAt(int index, float stopTime)
        {
            isSpinning = false;

            if (spinTween != null)
            {
                spinTween.Kill();
            }
            
            // if (!symbolsMap.TryGetValue(index, out var symbol))
            // {
            //     return;
            // }
            // index = symbols.IndexOf(symbol);
            // Debug.Log(index);
            // Debug.Log(symbol.name);
            //
            // var curIdx = symbols.IndexOf(symbol);
            // symbol.SetSiblingIndex(0);
            //
            // symbols.Sort((a,b) => a.GetSiblingIndex().CompareTo(b.GetSiblingIndex()));
            //
            // var newIdx = symbols.IndexOf(symbol);
            // var targetY = -(newIdx * symbolHeight);
            
            Debug.Log("Stopping at " + index);
            var targetY = -(index * symbolHeight);
            Debug.Log(targetY);

            content.DOAnchorPosY(targetY, stopTime)
                .SetEase(Ease.OutBack)
                .OnComplete(() => { targetSymbol = null; })
                .Play();
        }
    }
}
