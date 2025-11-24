using System;
using UniRx;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    public class ItemStatPanel : MonoBehaviour
    {
        [SerializeField] protected RinawaImage _iconImage;
        [SerializeField] protected RinawaText _nameText;
        [SerializeField] protected RinawaText _valueText;
        [SerializeField] protected RinawaText _valueChangeText;

        [SerializeField] private Color _positiveChangeColor = Color.green;
        [SerializeField] private Color _negativeChangeColor = Color.red;

        protected CompositeDisposable _compositeDisposable = new();

        protected readonly ReactiveProperty<Sprite> _iconProp = new(null);
        protected readonly ReactiveProperty<string> _nameProp = new(null);
        protected readonly ReactiveProperty<int> _valueChangeProp = new(0);
        protected ReactiveProperty<int> _valueProp = new(0);

        public void Setup(Sprite icon, string name, int value, int valueChange = 0)
        {
            _iconProp.Value = icon;
            _nameProp.Value = name;
            _valueProp.Value = value;
            _valueChangeProp.Value = value + valueChange;
        }

        protected virtual void OnEnable()
        {
            _iconProp.Subscribe(x =>
            {
                _iconImage.sprite = x;
                _iconImage.gameObject.SetActive(x != null);
            }).AddTo(_compositeDisposable);

            _nameProp.Subscribe(x =>
            {
                _nameText.text = x;
                _nameText.gameObject.SetActive(!string.IsNullOrEmpty(x));
            }).AddTo(_compositeDisposable);

            _valueProp.Subscribe(x =>
            {
                x = Math.Max(0, x);
                var valueStr = $"{x}";
                _valueText.text = valueStr;
                _valueText.gameObject.SetActive(!string.IsNullOrEmpty(valueStr));
            }).AddTo(_compositeDisposable);

            _valueChangeProp.Subscribe(x =>
            {
                x = Math.Max(0, x);
                var value = _valueProp.Value;
                var valueChange = _valueChangeProp.Value;
                var valChangeStr = x != 0 && value != valueChange ? $"{x}" : "";
                _valueChangeText.text = $"=> {valChangeStr}";
                _valueChangeText.gameObject.SetActive(!string.IsNullOrEmpty(valChangeStr));
                _valueChangeText.color = x == 0 ? Color.white : x > value ? _positiveChangeColor : _negativeChangeColor;
            }).AddTo(_compositeDisposable);
        }

        protected virtual void OnDisable()
        {
            _compositeDisposable.Clear();
        }

        protected virtual void OnDestroy()
        {
            _compositeDisposable.Dispose();
            _compositeDisposable = null;
        }
    }
}
