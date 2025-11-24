using UniRx;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    [RequireComponent(typeof(RinawaSlider))]
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private RinawaSlider _healthSlider;

        private bool _isDestroyed;
        // private IDisposable _disposable;
        private readonly ReactiveProperty<float> _progressFillProp = new(0f);

        public void SetHealth(float health)
        {
            _progressFillProp.Value = health;

            SetActive(true);

            // Used to disable the healthbar after x seconds. Will keep for now.
            // _disposable?.Dispose();
            // _disposable = Observable.Timer(TimeSpan.FromSeconds(3))
            // .Subscribe(_ => SetActive(false));
        }

        public void SetActive(bool isActive)
        {
            if (_isDestroyed)
            {
                return;
            }

            gameObject.SetActive(isActive);
        }

        private void OnEnable()
        {
            _progressFillProp.Subscribe(x => _healthSlider.value = x);
        }

        private void OnDestroy()
        {
            _isDestroyed = true;
            // _disposable?.Dispose();
            _progressFillProp?.Dispose();
        }
    }
}
