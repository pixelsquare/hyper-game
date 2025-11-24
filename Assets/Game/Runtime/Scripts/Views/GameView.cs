using System;
using System.Collections.Generic;
using System.Threading;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game
{
    public class GameView : View
    {
        [SerializeField] private Image answerImage;
        [SerializeField] private Slider timerSlider;
        [SerializeField] private Button homeButton; // Temp

        [Header("Choices")]
        [SerializeField] private Transform choicesParent;
        [SerializeField] private Choice choicesPrefab;
        
        [Header("Hints")]
        [SerializeField] private Transform hintsParent;
        [SerializeField] private Hint hintsPrefab;

        public ReactiveProperty<(float, float)> TimerMinMax { get; set; } = new();
        public ReactiveProperty<float> TimerProgress { get; set; } = new();
        public ReactiveProperty<Sprite> AnswerSprite { get; set; } = new(null);
        public ReactiveProperty<string[]> Choices { get; set; } = new(Array.Empty<string>());
        public ReactiveProperty<string[]> Hints { get; set; } = new(Array.Empty<string>());

        public UnityAction<int, string> OnChoiceSelected;
        
        private IObjectPool<Choice> choicePool;
        private List<Choice> choiceButtons = new(4);
        
        private IObjectPool<Hint> hintPool;
        private List<Hint> hintObjs = new(4);

        private Queue<string> hintQueue = new();
        
        private void Awake()
        {
            choicePool = new ObjectPool<Choice>(
                createFunc: () => CreatePooledObject(choicesPrefab, choicesParent),
                actionOnGet: x => x.gameObject.SetActive(true),
                actionOnRelease: x => x.gameObject.SetActive(false),
                actionOnDestroy: Destroy,
                collectionCheck: true,
                defaultCapacity: 4,
                maxSize: 4
            );

            hintPool = new ObjectPool<Hint>(
                createFunc: () => CreatePooledObject(hintsPrefab, hintsParent),
                actionOnGet: x => x.gameObject.SetActive(true),
                actionOnRelease: x => x.gameObject.SetActive(false),
                actionOnDestroy: Destroy,
                collectionCheck: true,
                defaultCapacity: 4,
                maxSize: 4
            );
            
            var d = Disposable.CreateBuilder();
            
            homeButton.onClick.AsObservable()
                .Subscribe(_ => SceneManager.LoadScene("Title"))
                .AddTo(ref d);
            
            AnswerSprite
                .Subscribe(x => answerImage.sprite = x)
                .AddTo(ref d);

            TimerMinMax
                .Subscribe(x =>
            {
                timerSlider.minValue = x.Item1;
                timerSlider.maxValue = x.Item2;
            }).AddTo(ref d);
            
            TimerProgress
                .Subscribe(x => timerSlider.value = x)
                .AddTo(ref d);
            
            Hints
                .Subscribe(x =>
            {
                hintQueue.Clear();
                foreach (var obj in x)
                {
                    hintQueue.Enqueue(obj);
                }
            }).AddTo(ref d);
            
            Choices.Subscribe(x =>
            {
                using var choicesCts = new  CancellationTokenSource();
                
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(choicesCts.Token, destroyCancellationToken);
                var d = Disposable.CreateBuilder();
                
                choiceButtons.ForEach(x => x.IsInteractable.Value = true);
                
                for (var i = 0; i < x.Length; i++)
                {
                    var idx = i;
                    var choiceString = x[idx];
                    var choiceButton = choicePool.Get();
                    
                    choiceButton.OnClickEvent
                        .Subscribe(_ =>
                        {
                            OnChoiceSelected?.Invoke(idx, choiceString);
                            choiceButtons.ForEach(x => x.IsInteractable.Value = false);
                        })
                        .AddTo(ref d);

                    choiceButton.Text.Value = choiceString;
                    choiceButtons.Add(choiceButton);
                }

                d.RegisterTo(linkedCts.Token);
            }).AddTo(ref d);
            
            d.RegisterTo(destroyCancellationToken);
        }

        public bool TryShowNextHint()
        {
            if (!hintQueue.TryDequeue(out var hintString))
            {
                return false;
            }
            
            var hintObj = hintPool.Get();
            hintObj.transform.SetAsLastSibling();
            var hintText = hintObj.GetComponentInChildren<TextMeshProUGUI>();
            hintText.text = hintString;
            hintObjs.Add(hintObj);
            return true;
        }

        public void SetRoundCleared()
        {
            foreach (var obj in choiceButtons)
            {
                obj.Color.Value = Color.white;
                choicePool.Release(obj);
            }

            foreach (var obj in hintObjs)
            {
                hintPool.Release(obj);
            }
            
            choiceButtons.Clear();
            hintObjs.Clear();
        }

        public void ShowCorrectAnswer(int correctAnswerIdx)
        {
            SetChoiceColor(correctAnswerIdx, Color.green);
        }

        public void SetChoiceColor(int choiceIndex, Color color)
        {
            if (choiceIndex < 0 || choiceIndex >= choicesParent.childCount)
            {
                Debug.Log($"Index out of range. {choiceIndex}");
                return;
            }
            
            choiceButtons[choiceIndex].Color.Value = color;
        }

        public void SetChoicesEnabled(bool enabled)
        {
            foreach (var choiceButton in choiceButtons)
            {
                choiceButton.IsInteractable.Value = enabled;
            }
        }

        private T CreatePooledObject<T>(T prefab, Transform parent) where T : UnityEngine.Object
        {
            return Instantiate(prefab, parent);
        }
    }
}