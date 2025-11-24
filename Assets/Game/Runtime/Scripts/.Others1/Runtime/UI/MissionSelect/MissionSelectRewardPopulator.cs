using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    using UIEvent = GameEvents.UserInterface;

    public class MissionSelectRewardPopulator : MonoBehaviour
    {
        [SerializeField] private bool _isFirstClearRewards;
        [SerializeField] private Transform _parentTransform;
        [SerializeField] private RewardPanel _rewardPanelPrefab;

        private MissionRewardPanelPool RewardPanelPool => _rewardPanelPool ??= new MissionRewardPanelPool(_diContainer, _rewardPanelPrefab, _parentTransform);

        [Inject] private DiContainer _diContainer;

        private MissionRewardPanelPool _rewardPanelPool;

        private void PopulateRewards(IEnumerable<MissionReward> missionRewards)
        {
            foreach (var missionReward in missionRewards)
            {
                var rewardPanel = RewardPanelPool.Rent();
                rewardPanel.Setup(missionReward);
            }
        }

        private void HandleActiveLevelSelected(IMessage message)
        {
            if (message.Data is not MissionLevel missionLevel)
            {
                return;
            }

            RewardPanelPool.ReturnAll();
            var missionRewards = _isFirstClearRewards ? missionLevel.FirstClearRewards : missionLevel.OtherRewards;
            PopulateRewards(missionRewards);
        }

        private void OnEnable()
        {
            Dispatcher.AddListener(UIEvent.OnActiveLevelSelected, HandleActiveLevelSelected, true);
        }

        private void OnDisable()
        {
            Dispatcher.RemoveListener(UIEvent.OnActiveLevelSelected, HandleActiveLevelSelected, true);
        }

        private class MissionRewardPanelPool : RinawaObjectPool<RewardPanel>
        {
            public MissionRewardPanelPool(DiContainer diContainer, RewardPanel prefab, Transform parent)
                    : base(diContainer, prefab, parent)
            {
            }
        }
    }
}
