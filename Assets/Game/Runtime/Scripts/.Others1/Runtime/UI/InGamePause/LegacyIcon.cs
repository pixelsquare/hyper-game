using UniRx;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class LegacyIcon : MonoBehaviour
    {
        [SerializeField] private RinawaImage _iconImage;

        [Inject] private ReactiveTextureProperty _iconProp;

        private CompositeDisposable _compositeDisposable = new();

        public void Setup(string iconName)
        {
            _iconProp.Value = iconName;
        }

        private void OnEnable()
        {
            _iconProp.Subscribe(x =>
            {
                _iconImage.sprite = x;
                _iconImage.gameObject.SetActive(x != null);
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
