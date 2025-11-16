using Kumu.Extensions;
using Kumu.Kulitan.Backend;
using Kumu.Kulitan.Common;

namespace Kumu.Kulitan.Gifting
{
    public class VGRewardsTracker : SingletonMonoBehaviour<VGRewardsTracker>
    {
        private int totalRewards = 0;

        public bool IsPopupEnabled { get; set; }
        public int TotalRewards => totalRewards;

        public void ResetTotalRewards()
        {
            totalRewards = 0;
            IsPopupEnabled = false;
        }

        private void OnReceiveGiftEvent(VirtualGiftEventInfo vgEventInfo)
        {
            var localProfile = UserProfileLocalDataManager.Instance.GetLocalUserProfile();

            var isLocalUserAGiftee = false;
            foreach (var giftee in vgEventInfo.giftees)
            {
                if (localProfile.accountId != giftee)
                {
                    continue;
                }
                isLocalUserAGiftee = true;
                break;
            }

            if (!isLocalUserAGiftee)
            {
                return;
            }

            IsPopupEnabled = true;
            var share = vgEventInfo.ComputeOwnShare();
            totalRewards += share;
            $"[VirtualGifts] VG received. Your share: {share}. Total: {totalRewards}".Log();
        }

        private void Awake()
        {
            ResetTotalRewards();
        }

        private void OnEnable()
        {
            Services.VirtualGiftService.VirtualGiftReceivedEvent += OnReceiveGiftEvent;
        }

        private void OnDisable()
        {
            Services.VirtualGiftService.VirtualGiftReceivedEvent -= OnReceiveGiftEvent;
        }
    }
}
