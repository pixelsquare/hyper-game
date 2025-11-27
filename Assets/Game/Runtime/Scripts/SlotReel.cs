using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

namespace Game
{
    public class SlotReel : MonoBehaviour
    {
        [SerializeField] private RectTransform content;
        [SerializeField] private VerticalLayoutGroup layoutGroup;
        [SerializeField] private ContentSizeFitter contentSizeFitter;
        [SerializeField] private float symbolHeight = 200f;
        [SerializeField] private float scrollDuration = 0.15f;
        [SerializeField] private int symbolCount = 6;

        private List<RectTransform> symbols = new();
        private Dictionary<int, RectTransform> symbolsMap = new();
        private bool isSpinning = false;
        private Tweener spinTween;
        private int stopIndex;
        
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
            
            targetSymbol = symbols[0];
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
                StopAt(1, 1f);
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

            // contentSizeFitter.enabled = false;
            // layoutGroup.enabled = false;
            content.pivot = new Vector2(0.5f, 0.0f);

            var r = content.GetChild(0) as RectTransform;
            var y = r.anchoredPosition.y;
            var rand = new Random();

            for (var i = 0; i < 10; i++)
            {
                var pre = i == 5 ? targetSymbol : symbols[rand.Next(0, symbolCount)];
                var sym = Instantiate(pre, content);
                var anc = sym.anchoredPosition;
                anc.y = y + symbolHeight;
                sym.anchoredPosition = anc;
                y = anc.y;
                sym.SetAsFirstSibling();
            }
            
            

            content.DOAnchorPosY(-(content.childCount - 3) * (symbolHeight + layoutGroup.spacing), stopTime)
                .SetEase(Ease.OutBack)
                .OnComplete(() => { targetSymbol = null; })
                .Play();
        }
    }
}
