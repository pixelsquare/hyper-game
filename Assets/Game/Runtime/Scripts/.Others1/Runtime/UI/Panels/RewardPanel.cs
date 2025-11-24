using UnityEngine;

namespace Santelmo.Rinsurv
{
    public class RewardPanel : MonoBehaviour
    {
        [SerializeField] private RinawaImage _rewardIcon;
        [SerializeField] private RinawaImage _rewardOutline;
        [SerializeField] private RinawaText _rewardAmount;

        public void Setup(MissionReward missionReward)
        {
            SetRewardIcon(missionReward.Icon);
            SetRewardAmount(missionReward.Amount);
        }

        public void ResetDefaults()
        {
            SetRewardIcon(null);
            SetRewardAmount(0);
            SetRewardOutlineColor(Color.white);
        }

        private void SetRewardIcon(Sprite rewardIcon)
        {
            _rewardIcon.sprite = rewardIcon;
        }

        private void SetRewardOutlineColor(Color color)
        {
            _rewardOutline.color = color;
        }

        private void SetRewardAmount(int amount)
        {
            _rewardAmount.text = amount.ToString();
        }
    }
}
