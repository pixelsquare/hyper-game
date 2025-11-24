using System;
using UniRx;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    using SaveKey = GameConstants.SaveKeys;

    public class SettingsHud : BaseHud
    {
        [Header("Sfx")]
        [SerializeField] private RinawaSlider _sfxSlider;
        [SerializeField] private RinawaText _sfxValueText;

        [Header("Bgm")]
        [SerializeField] private RinawaSlider _bgmSlider;
        [SerializeField] private RinawaText _bgmValueText;

        private float SfxVolume
        {
            get => float.TryParse(_saveManager.Load(SaveKey.SfxVolumeKey), out var volume)
                    ? volume
                    : _audioManager.SfxVolume;
            set => _saveManager.Save(SaveKey.SfxVolumeKey, $"{value}");
        }

        private float BgmVolume
        {
            get => float.TryParse(_saveManager.Load(SaveKey.BgmVolumeKey), out var volume)
                    ? volume
                    : _audioManager.BgmVolume;
            set => _saveManager.Save(SaveKey.BgmVolumeKey, $"{value}");
        }

        [Inject] private ISaveManager _saveManager;
        [Inject] private IAudioManager _audioManager;

        private CompositeDisposable _compositeDisposable = new();

        private readonly ReactiveProperty<float> _bgmValueProp = new(1.0f);
        private readonly ReactiveProperty<float> _sfxValueProp = new(1.0f);

        private void Initialize()
        {
            _sfxSlider.minValue = 0f;
            _sfxSlider.maxValue = 1f;

            _bgmSlider.minValue = 0f;
            _bgmSlider.maxValue = 1f;
        }

        private void HandleSfxValueChanged(float value)
        {
            _sfxValueProp.Value = value;
        }

        private void HandleBgmValueChanged(float value)
        {
            _bgmValueProp.Value = value;
        }

        private void Awake()
        {
            HandleSfxValueChanged(SfxVolume);
            HandleBgmValueChanged(BgmVolume);
        }

        private void OnEnable()
        {
            _sfxValueProp.Subscribe(x =>
            {
                SfxVolume = x;
                _sfxSlider.SetValueWithoutNotify(x);
                _sfxValueText.text = $"{Math.Truncate(x * 100f)}";
                _audioManager.SetSfxVolume(x);
            }).AddTo(_compositeDisposable);

            _bgmValueProp.Subscribe(x =>
            {
                BgmVolume = x;
                _bgmSlider.SetValueWithoutNotify(x);
                _bgmValueText.text = $"{Math.Truncate(x * 100f)}";
                _audioManager.SetBgmVolume(x);
            }).AddTo(_compositeDisposable);

            _sfxSlider.OnValueChangedAsObservable()
                      .Subscribe(HandleSfxValueChanged)
                      .AddTo(_compositeDisposable);

            _bgmSlider.OnValueChangedAsObservable()
                      .Subscribe(HandleBgmValueChanged)
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

        private void Start()
        {
            Initialize();
        }
    }
}
