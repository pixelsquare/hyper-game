using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    using UIEvent = GameEvents.UserInterface;

    public class MissionSelectPopulator : MonoBehaviour
    {
        [SerializeField] private Transform _missionLevelsParent;
        [SerializeField] private MissionLevelButton _missionLevelButtonPrefab;

        private MissionLevelPool MissionButtonPool => _missionLevelPool ??= new MissionLevelPool(_diContainer, _missionLevelButtonPrefab, _missionLevelsParent);

        [Inject] private DiContainer _diContainer;
        [Inject] private IMissionManager _missionManager;

        private Mission _selectedMission;
        private MissionLevel _selectedLevel;
        private MissionLevelButton _selectedLevelButton;
        private MissionLevelPool _missionLevelPool;

        private void PopulateMissionLevels(IEnumerable<MissionLevel> missionLevels)
        {
            foreach (var missionLevel in missionLevels)
            {
                var levelButton = MissionButtonPool.Rent();
                levelButton.Setup(missionLevel, true);

                if (missionLevel.Id.Equals(_selectedLevel.Id))
                {
                    _selectedLevelButton = levelButton;
                }
            }

            _selectedLevelButton.SetButtonSelected(true);
        }

        private void HandleActiveMissionSelected(IMessage message)
        {
            if (message.Data is not Mission mission)
            {
                return;
            }

            MissionButtonPool.ReturnAll();
            _selectedMission = _missionManager.ActiveMission;
            _selectedLevel = Array.FindIndex(_selectedMission.MissionLevels.ToArray(), x => _missionManager.ActiveMissionLevel.Id.Equals(x.Id)) != -1
                    ? _missionManager.ActiveMissionLevel
                    : mission.MissionLevels.First();
            PopulateMissionLevels(mission.MissionLevels);
            Dispatcher.SendMessageData(UIEvent.OnActiveLevelSelected, _selectedLevel);
        }

        private void HandleActiveLevelSelected(IMessage message)
        {
            if (message.Data is not MissionLevel level || message.Sender is not MissionLevelButton levelButton)
            {
                return;
            }

            _selectedLevel = level;
            _missionManager.SetActiveMissionLevel(_selectedLevel);

            _selectedLevelButton.SetButtonSelected(false);
            levelButton.SetButtonSelected(true);
            _selectedLevelButton = levelButton;
        }

        private void OnEnable()
        {
            Dispatcher.AddListener(UIEvent.OnActiveMissionSelected, HandleActiveMissionSelected, true);
            Dispatcher.AddListener(UIEvent.OnActiveLevelSelected, HandleActiveLevelSelected);
        }

        private void OnDisable()
        {
            Dispatcher.RemoveListener(UIEvent.OnActiveMissionSelected, HandleActiveMissionSelected, true);
            Dispatcher.RemoveListener(UIEvent.OnActiveLevelSelected, HandleActiveLevelSelected, true);
        }

        private class MissionLevelPool : RinawaObjectPool<MissionLevelButton>
        {
            public MissionLevelPool(DiContainer diContainer, MissionLevelButton prefab, Transform parent)
                    : base(diContainer, prefab, parent)
            {
            }
        }
    }
}
