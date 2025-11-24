using System;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    [RequireComponent(typeof(RinawaButton))]
    public abstract class BaseButton : MonoBehaviour
    {
        [SerializeField] protected bool _preventSpamClick = true;
        [SerializeField] protected GameObject _buttonLoader;

        public UniTask Task { get; protected set; }
        public ReactiveProperty<bool> ButtonLoaderActiveProp { get; private set; }
        public ReactiveProperty<bool> ButtonInteractableProp { get; private set; }

        protected RinawaImage _image;
        protected RinawaButton _button;
        protected CompositeDisposable _compositeDisposable = new();

        protected abstract void OnButtonClicked();

        protected virtual void Awake()
        {
            if (!TryGetComponent(out _image))
            {
                throw new NullReferenceException("RinawaImage component does not exist.");
            }

            if (!TryGetComponent(out _button))
            {
                throw new NullReferenceException("RinawaButton component does not exist.");
            }

            ButtonLoaderActiveProp = new ReactiveProperty<bool>(_buttonLoader.activeInHierarchy);
            ButtonInteractableProp = new ReactiveProperty<bool>(_button.interactable);
        }

        protected virtual void OnEnable()
        {
            _button.OnClickAsObservable()
                   .Subscribe(_ => HandleButtonClicked())
                   .AddTo(_compositeDisposable);

            ButtonLoaderActiveProp.Where(_ => _buttonLoader != null)
                                  .Subscribe(x => _buttonLoader.SetActive(x))
                                  .AddTo(_compositeDisposable);

            ButtonInteractableProp.Where(_ => _button != null)
                                  .Subscribe(x => _button.interactable = x)
                                  .AddTo(_compositeDisposable);
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

        protected virtual void Start()
        {
            Task = UniTask.Create(async () => await _button.OnClickAsync());
        }

        private void HandleButtonClicked()
        {
            if (_preventSpamClick)
            {
                ButtonInteractableProp.Value = false;
            }

            OnButtonClicked();
        }
    }
}
