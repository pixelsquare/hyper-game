using System;
using System.Linq;
using UniRx;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    using UIEvent = GameEvents.UserInterface;

    public class MissionSelectHud : BaseHud
    {
        [Header("Carousel")]
        [SerializeField] private MissionBiomeView _missionBiomeView;
        [SerializeField] private RinawaButton _carouselLeftButton;
        [SerializeField] private RinawaButton _carouselRightButton;

        private int MaxLevelCount => _missionManager.Missions.Sum(x => x.MissionLevels.Count()) - 1;

        [Inject] private IMissionManager _missionManager;
        [Inject] private IAudioManager _audioManager;

        private bool _isInitialized;
        private CompositeDisposable _compositeDisposable = new();

        private void InitializeBiome()
        {
            var biomes = _missionManager.Missions
                                        .Select(x => new MissionSelectBiome(x.Biome.Background))
                                        .ToArray();

            _missionBiomeView.Setup(biomes);
        }

        private (Mission, MissionLevel) GetActiveMission(int levelIdx)
        {
            var levelCount = Mathf.Clamp(levelIdx, 0, MaxLevelCount);

            foreach (var mission in _missionManager.Missions)
            {
                var missionLevelCount = mission.MissionLevels.Count();
                levelCount -= missionLevelCount;

                if (levelCount >= 0)
                {
                    continue;
                }

                var missionLevelIdx = levelCount + missionLevelCount;
                var missionLevel = mission.MissionLevels.ElementAt(missionLevelIdx);
                return new ValueTuple<Mission, MissionLevel>(mission, missionLevel);
            }

            throw new IndexOutOfRangeException($"Index out of range. {levelIdx}");
        }

        private void HandleActiveCellChanged(int index)
        {
            if (!_isInitialized)
            {
                return;
            }

            var selectedMission = _missionManager.Missions[index];
            _missionManager.SetActiveMission(selectedMission);
            Dispatcher.SendMessageData(UIEvent.OnActiveMissionSelected, selectedMission);
        }

        private void OnEnable()
        {
            var duration = _missionBiomeView.SnapAnimationDuration;
            var easing = _missionBiomeView.SnapAnimationType;

            _carouselLeftButton.OnClickAsObservable()
                               .Subscribe(_ => _missionBiomeView.ScrollToBefore(duration, easing))
                               .AddTo(_compositeDisposable);

            _carouselRightButton.OnClickAsObservable()
                                .Subscribe(_ => _missionBiomeView.ScrollToAfter(duration, easing))
                                .AddTo(_compositeDisposable);

            _missionBiomeView.ActiveCellChanged += HandleActiveCellChanged;

            _audioManager.PlaySound(Bgm.MissionSelect);
        }

        private void OnDisable()
        {
            _compositeDisposable.Clear();
            _missionBiomeView.ActiveCellChanged -= HandleActiveCellChanged;
        }

        private void OnDestroy()
        {
            _compositeDisposable.Dispose();
            _compositeDisposable = null;
        }

        private void Start()
        {
            InitializeBiome();

            // TODO: [TONY] Store and retrieve level index on some save data for level progression.
            var (mission, missionLevel) = GetActiveMission(0);
            _missionManager.SetActiveMission(mission);
            _missionManager.SetActiveMissionLevel(missionLevel);

            var duration = _missionBiomeView.SnapAnimationDuration;
            var easing = _missionBiomeView.SnapAnimationType;
            var missionBiomeIdx = Array.FindIndex(_missionManager.Missions, x => x.Id.Equals(mission.Id));
            _missionBiomeView.ScrollTo(missionBiomeIdx, duration, easing);

            Dispatcher.SendMessageData(UIEvent.OnActiveMissionSelected, mission);

            _isInitialized = true;
        }
    }
}
