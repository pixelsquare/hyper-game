using UniRx;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class LegacyIconProgress : MonoBehaviour
    {
        [SerializeField] private RinawaImage _iconImage;
        [SerializeField] private RinawaImage _typeImage;
        [SerializeField] private RinawaImage _progressImage;

        [Inject] private ReactiveTextureProperty _iconProp;

        private CompositeDisposable _compositeDisposable = new();

        private readonly ReactiveProperty<Sprite> _typeProp = new(null);
        private readonly ReactiveProperty<int> _progressProp = new(0);

        public void Setup(ILegacy legacy)
        {
            _iconProp.Value = legacy.LegacyIconName;
            _typeProp.Value = legacy.LegacyTypeSprite;
            _progressProp.Value = (int)legacy.CurrentLevel;
        }

        private void OnEnable()
        {
            _iconProp.Subscribe(x =>
            {
                _iconImage.sprite = x;
                _iconImage.gameObject.SetActive(x != null);
            }).AddTo(_compositeDisposable);

            _typeProp.Subscribe(x =>
            {
                _typeImage.sprite = x;
                _typeImage.gameObject.SetActive(x != null);
            }).AddTo(_compositeDisposable);

            _progressProp.Subscribe(x => _progressImage.fillAmount = x * 0.2f)
                         .AddTo(_compositeDisposable);
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
