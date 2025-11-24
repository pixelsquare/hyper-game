using UniRx;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    public class LegacyPanel : MonoBehaviour
    {
        [SerializeField] private RinawaText _nameText;
        [SerializeField] private RinawaText _descriptionText;
        [SerializeField] private RinawaText _legacyLayerText;
        [SerializeField] private RinawaText _legacyLayerTypeText;
        [SerializeField] private LegacyIconProgress _iconProgress;

        private readonly CompositeDisposable _compositeDisposable = new();

        private readonly ReactiveProperty<string> _titleProp = new(null);
        private readonly ReactiveProperty<string> _descriptionProp = new(null);
        private readonly ReactiveProperty<string> _legacyLayerProp = new(null);
        private readonly ReactiveProperty<string> _legacyLayerTypeProp = new(null);

        public void Setup(ILegacy legacy, bool hideLegacyType = false)
        {
            _titleProp.Value = legacy.LegacyName;
            _descriptionProp.Value = legacy.LegacyDescription;
            _legacyLayerProp.Value = legacy.LegacySlot.ToString();
            _legacyLayerTypeProp.Value = hideLegacyType ? null : legacy.LegacySlot == LegacySlot.Passive ? "Passive" : "Active";
            _iconProgress.Setup(legacy);
        }

        private void OnEnable()
        {
            _titleProp.Subscribe(x =>
            {
                _nameText.text = x;
                _nameText.gameObject.SetActive(!string.IsNullOrEmpty(x));
            }).AddTo(_compositeDisposable);

            _descriptionProp.Subscribe(x =>
            {
                _descriptionText.text = x;
                _descriptionText.gameObject.SetActive(!string.IsNullOrEmpty(x));
            }).AddTo(_compositeDisposable);

            _legacyLayerProp.Subscribe(x =>
            {
                _legacyLayerText.text = x;
                _legacyLayerText.gameObject.SetActive(!string.IsNullOrEmpty(x));
            }).AddTo(_compositeDisposable);

            _legacyLayerTypeProp.Subscribe(x =>
            {
                _legacyLayerTypeText.text = x;
                _legacyLayerTypeText.gameObject.SetActive(!string.IsNullOrEmpty(x));
            }).AddTo(_compositeDisposable);
        }

        private void OnDisable()
        {
            _compositeDisposable.Clear();
        }

        private void OnDestroy()
        {
            _compositeDisposable.Dispose();
        }
    }
}
