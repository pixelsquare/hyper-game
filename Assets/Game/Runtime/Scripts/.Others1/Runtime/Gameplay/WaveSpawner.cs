using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class WaveSpawner : MonoBehaviour
    {
        [SerializeField] private EnemyWaveConfig[] _enemyWaves;

        public delegate void OnRequestSpawn(SpawnData[] spawnData);
        public event OnRequestSpawn OnRequestSpawnEvent;

        private EnemyWaveConfig _currentWave;
        private float _interval;
        private float _duration;

        private IMissionManager _missionManager;
        private LazyInject<WaveTracker> _waveTracker;

        [Inject]
        public void Construct(IMissionManager missionManager, LazyInject<WaveTracker> waveTracker)
        {
            _missionManager = missionManager;
            _waveTracker = waveTracker;
        }

        private void Init()
        {
            if (_missionManager != null)
            {
                _enemyWaves = _missionManager.ActiveMissionLevel.GameLevel._enemyWaves;
            }
        }

        private void OnWaveUpdate(uint waveidx)
        {
            if (waveidx < _enemyWaves.Length)
            {
                _currentWave = _enemyWaves[waveidx];
            }
            else
            {
                _currentWave = null;
            }

            if (_currentWave is IWaveInterval waveInterval)
            {
                _interval = waveInterval.WaveInterval;
            }

            if (_currentWave is IWaveInitial waveInitial)
            {
                var spawnData = waveInitial.GetInitialWave(Vector2.zero);
                OnRequestSpawnEvent?.Invoke(spawnData);  // todo [jef] : supply spawn point origin (likely player position)
            }

            if (_currentWave is IWaveFinale waveFinale)
            {
                waveFinale.OnWaveFinale();
                _currentWave = null;
            }

            _duration = 0f;
        }

        private void Update()
        {
            if (!_currentWave)
            {
                return;
            }

            if (_duration > 0f)
            {
                _duration -= Time.deltaTime;
            }
            else if (_currentWave is IWaveInterval waveInterval)
            {
                var spawnData = waveInterval.GetIntervalWave(Vector2.zero);
                OnRequestSpawnEvent?.Invoke(spawnData);  // todo [jef] : supply spawn point origin (likely player position)

                _duration = _interval;
            }
        }

        private void OnEnable()
        {
            _waveTracker.Value.OnWaveUpdateEvent += OnWaveUpdate;
        }

        private void OnDisable()
        {
            _waveTracker.Value.OnWaveUpdateEvent -= OnWaveUpdate;
        }

        private void Start()
        {
            Init();
        }
    }
}
