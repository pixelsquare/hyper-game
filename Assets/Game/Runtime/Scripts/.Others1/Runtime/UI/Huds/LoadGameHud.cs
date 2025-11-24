using System;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    public class LoadGameHud : BaseHud
    {
        [SerializeField] private RinawaText _loadingText;
        [SerializeField] private RinawaSlider _loadingSlider;

        private CompositeDisposable _compositeDisposable = new();
        private float _duration = 1f;

        private readonly ReactiveProperty<float> _loadingProp = new(0.0f);

        public LoadGameHud Setup(float duration = 1f)
        {
            _duration = duration;
            Observable.EveryUpdate()
                      .Timeout(TimeSpan.FromSeconds(_duration))
                      .Subscribe(_ => _loadingProp.Value += Time.deltaTime)
                      .AddTo(_compositeDisposable);

            Task = UniTask.WaitForSeconds(_duration);
            return this;
        }

        private void OnEnable()
        {
            _loadingProp.Subscribe(x =>
            {
                var progress = x / _duration;
                _loadingSlider.value = progress;
                _loadingText.text = $"Loading ... {progress * 100f:F}%";
            }).AddTo(_compositeDisposable);
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
