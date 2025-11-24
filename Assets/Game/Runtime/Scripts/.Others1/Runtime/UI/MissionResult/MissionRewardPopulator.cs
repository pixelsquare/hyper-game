using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class MissionRewardPopulator : MonoBehaviour
    {
        [SerializeField] private Transform _parentTransform;
        [SerializeField] private ItemButton _missionRewardPrefab;

        private RewardIconPool RewardPool => _rewardPool ??= new RewardIconPool(_diContainer, _missionRewardPrefab, _parentTransform);

        [Inject] private DiContainer _diContainer;

        private RewardIconPool _rewardPool;

        public MissionRewardPopulator Setup(IEnumerable<IItem> rewardItems)
        {
            if (rewardItems == null)
            {
                return this;
            }

            PopulateMissionRewards(rewardItems);
            return this;
        }

        private void PopulateMissionRewards(IEnumerable<IItem> rewardItems)
        {
            var rewards = from reward in rewardItems
                          group reward by reward
                          into g
                          let count = g.Count()
                          select new { Value = g.Key, Count = count };

            foreach (var rewardItem in rewards)
            {
                var itemButton = RewardPool.Rent();
                itemButton.Setup(rewardItem.Value, rewardItem.Count, null);
            }
        }

        private void OnDisable()
        {
            RewardPool.ReturnAll();
        }

        private class RewardIconPool : RinawaObjectPool<ItemButton>
        {
            public RewardIconPool(DiContainer diContainer, ItemButton prefab, Transform parent)
                    : base(diContainer, prefab, parent)
            {
            }
        }
    }
}
