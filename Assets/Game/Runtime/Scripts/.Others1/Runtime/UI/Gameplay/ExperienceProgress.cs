using UniRx;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class ExperienceProgress : MonoBehaviour
    {
        [SerializeField] private RinawaSlider _progressImage;
        [Inject] private ExpTracker _expTracker;

        private CompositeDisposable _compositeDisposable = new();

        private readonly ReactiveProperty<float> _progressProp = new(0.0f);

        private void OnEnable()
        {
            _progressProp.Subscribe(x => _progressImage.value = x).AddTo(_compositeDisposable);

            Observable.EveryUpdate().Subscribe(x =>
            {
                _progressProp.Value = _expTracker.PercentProgress;
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

        private void Start()
        {
            _progressProp.Value = _expTracker.PercentProgress;
        }
    }
}
