using System;
using System.Collections.Generic;
using FancyCarouselView.Runtime.Scripts;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Santelmo.Rinsurv
{
    /// <summary>
    ///     Referenced from `DotCarouselProgressView.cs`
    /// </summary>
    [RequireComponent(typeof(HorizontalLayoutGroup))]
    public class MissionBiomeDotProgressView : ClickableCarouselProgressView
    {
        [SerializeField] private SpriteDotProgressElement _dotElementPrefab;

        public override event Action<int> ElementClicked;

        [InjectLocal] private DiContainer _diContainer;

        private int _activeIndex = -1;
        private CompositeDisposable _compositeDisposable = new();
        private List<SpriteDotProgressElement> _progressElementInstances = new();

        public override void Setup(int elementCount)
        {
            // Remove all instances if exists
            _progressElementInstances.ForEach(Destroy);
            _progressElementInstances.Clear();
            _progressElementInstances = new List<SpriteDotProgressElement>(elementCount);

            for (var i = 0; i < elementCount; i++)
            {
                var index = i;
                var instance = _diContainer.InstantiatePrefabForComponent<SpriteDotProgressElement>(_dotElementPrefab, transform);
                instance.SetActive(false);
                instance.Button
                        .OnClickAsObservable()
                        .Subscribe(_ => { OnElementClicked(index); })
                        .AddTo(_compositeDisposable);
                _progressElementInstances.Add(instance);
            }

            if (_activeIndex != -1)
            {
                SetActiveIndex(_activeIndex);
            }
        }

        public override void SetActiveIndex(int elementIndex)
        {
            if (_activeIndex != -1 && _progressElementInstances.Count - 1 >= _activeIndex)
            {
                var instance = _progressElementInstances[_activeIndex];
                instance.SetActive(false);
            }

            if (_progressElementInstances.Count - 1 >= elementIndex)
            {
                var instance = _progressElementInstances[elementIndex];
                instance.SetActive(true);
            }

            _activeIndex = elementIndex;
        }

        private void OnElementClicked(int index)
        {
            ElementClicked?.Invoke(index);
        }

        private void OnDisable()
        {
            _compositeDisposable.Clear();
        }

        private void OnDestroy()
        {
            _compositeDisposable.Dispose();
            _compositeDisposable = null;
        }
    }
}
