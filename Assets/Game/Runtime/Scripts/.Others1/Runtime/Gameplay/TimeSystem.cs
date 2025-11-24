using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class TimeSystem : MonoBehaviour
    {
        [SerializeField] private float _maxTime;
        [SerializeField] private GUIStyle _style;

        private float _time;
        private bool _isRunning;

        private IMissionManager _missionManager;

        [Inject]
        public void Construct(IMissionManager missionManager)
        {
            _missionManager = missionManager;
        }

        private void OnGameStart(IMessage message)
        {
            if (_missionManager != null)
            {
                _maxTime = _missionManager.ActiveMissionLevel.GameLevel._maxTime;
            }

            _time = 0f;
            _isRunning = true;
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

            if (_time < _maxTime)
            {
                _time += Time.deltaTime;
            }
            else
            {
                Dispatcher.SendMessage(GameEvents.Gameplay.OnGameWin);
                _isRunning = false;
            }
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

        private void OnGUI()
        {
            var rect = new Rect
            {
                x = 0,
                y = 80f,
                width = Screen.width,
                height = 80f
            };

            GUI.Label(rect, _time.ToString("0.00"), _style);
        }
    }
}
