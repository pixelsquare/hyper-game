using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class ExpTracker : MonoBehaviour
    {
        [SerializeField] private bool _isDebugMode;
        [SerializeField] private uint[] _expTable;
        [SerializeField] private GUIStyle _barStyle;
        [SerializeField] private GUIStyle _labelStyle;

        public delegate void OnLevelUp(uint level);
        public event OnLevelUp OnLevelUpEvent;

        public float PercentProgress => (float)_current / _expCap;

        private uint _level;
        private uint _current;
        private uint _expCap;

        private IAudioManager _audioManager;
        private LazyInject<GemTracker> _gemTracker;

        [Inject]
        private void Construct(IAudioManager audioManager, LazyInject<GemTracker> gemTracker)
        {
            _audioManager = audioManager;
            _gemTracker = gemTracker;
        }

        private void GrantXp(Gem gem)
        {
            _current += gem.Value;

            if (_current > _expCap)
            {
                _current -= _expCap;
                _level++;
                _expCap = _expTable[_level];
                OnLevelUpEvent?.Invoke(_level);
                _audioManager.PlaySound(Sfx.LevelUp);
            }
        }

        [ContextMenu("Force Level Up")]
        private void ForceLevelUp()
        {
            _level++;
            _expCap = _expTable[_level];
            OnLevelUpEvent?.Invoke(_level);
            _audioManager.PlaySound(Sfx.LevelUp);
        }

        private void OnEnable()
        {
            _gemTracker.Value.OnConsumeEvent += GrantXp;
        }

        private void OnDisable()
        {
            _gemTracker.Value.OnConsumeEvent -= GrantXp;
        }

        private void Start()
        {
            _expCap = _expTable[_level];
        }

        private void OnGUI()
        {
            if (!_isDebugMode)
            {
                return;
            }

            var container = new Rect
            {
                x = 0,
                y = 0,
                width = Screen.width,
                height = 80f
            };

            var bar = new Rect
            {
                x = 0,
                y = 0,
                height = 80f,
                width = (float)_current / _expCap * Screen.width
            };

            GUI.Box(container, string.Empty);
            GUI.Box(bar, string.Empty, _barStyle);
            GUI.Box(container, $"level: {_level}", _labelStyle);
        }
    }
}
