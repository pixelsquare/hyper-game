using UniRx;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    public class ItemButtonBasic : MonoBehaviour
    {
        [SerializeField] private RinawaText _amountText;

        private CompositeDisposable _compositeDisposable = new();

        private readonly ReactiveProperty<int> _amountProp = new(0);

        public void Setup(int amount)
        {
            _amountProp.Value = amount;
        }

        private void OnEnable()
        {
            _amountProp.Subscribe(x =>
            {
                var amount = x.ToString();
                _amountText.text = amount;
                _amountText.gameObject.SetActive(!string.IsNullOrEmpty(amount));
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
