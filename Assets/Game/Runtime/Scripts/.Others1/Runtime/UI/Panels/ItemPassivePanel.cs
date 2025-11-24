using UniRx;

namespace Santelmo.Rinsurv
{
    public class ItemPassivePanel : ItemStatPanel
    {
        protected override void OnEnable()
        {
            base.OnEnable();

            _valueProp.Dispose();
            _valueProp = new ReactiveProperty<int>(0);

            _valueProp.Subscribe(x =>
            {
                var valueStr = $"+{x}%";
                _valueText.text = valueStr;
                _valueText.gameObject.SetActive(!string.IsNullOrEmpty(valueStr));
            }).AddTo(_compositeDisposable);
        }
    }
}
