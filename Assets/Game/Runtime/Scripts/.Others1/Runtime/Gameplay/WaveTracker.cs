using UnityEngine;

namespace Santelmo.Rinsurv
{
    public class WaveTracker : MonoBehaviour
    {
        [SerializeField] private float _waveDuration = 30f;

        public delegate void OnWaveUpdate(uint waveIdx);
        public event OnWaveUpdate OnWaveUpdateEvent;

        private float _waveTime;
        private uint _waveIdx;
        private bool _isRunning;

        private void Init()
        {
            _waveTime = 0f;
            _waveIdx = 0;
            OnWaveUpdateEvent?.Invoke(_waveIdx);
            _isRunning = true;
        }

        private void OnGameStart(IMessage message)
        {
            Init();
        }

        private void OnFinaleWaveStart(IMessage message)
        {
            _isRunning = false;
        }

        private void Update()
        {
            if (!_isRunning)
            {
                return;
            }
            
            if (_waveTime > _waveDuration)
            {
                _waveTime = 0f;
                _waveIdx++;
                OnWaveUpdateEvent?.Invoke(_waveIdx);
            }

            _waveTime += Time.deltaTime;
        }

        private void OnEnable()
        {
            Dispatcher.AddListener(GameEvents.AppState.OnGameStartEvent, OnGameStart, true);
            Dispatcher.AddListener(GameEvents.Gameplay.OnFinaleWaveStart, OnFinaleWaveStart, true);

        }

        private void OnDisable()
        {
            Dispatcher.RemoveListener(GameEvents.AppState.OnGameStartEvent, OnGameStart, true);
            Dispatcher.RemoveListener(GameEvents.Gameplay.OnFinaleWaveStart, OnFinaleWaveStart, true);
        }
    }
}
