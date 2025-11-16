using System.Collections.Generic;
using Kumu.Extensions;
using Kumu.Kulitan.Backend;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;

namespace Kumu.Kulitan.Moderation
{
    public class ModerationManager : SingletonMonoBehaviour<ModerationManager>
    {
        private Slot<string> eventSlot;
        private HashSet<string> blockedPlayerIds = new();

        public bool IsPlayerBlocked(string userId)
        {
            return blockedPlayerIds.Contains(userId);
        }

        private async void Initialize(string userId)
        {
            var request = new GetBlockedPlayersRequest();
            var result = await Services.ModerationService.GetBlockedPlayersAsync(request);

            if (result.HasError)
            {
                "Failed to get blocked players".LogError();
                return;
            }

            blockedPlayerIds = new HashSet<string>(result.Result.blockedUserIds);
        }

        private void OnPlayerBlockedEvent(IEvent<string> callback)
        {
            var playerInstantiatedEvent = (PlayerBlockedEvent)callback;

            if (playerInstantiatedEvent.IsBlocked)
            {
                blockedPlayerIds.Add(playerInstantiatedEvent.AccountId);
            }
            else
            {
                blockedPlayerIds.Remove(playerInstantiatedEvent.AccountId);
            }
        }

        protected override void Awake()
        {
            base.Awake();
            eventSlot = new Slot<string>(GlobalNotifier.Instance);
            eventSlot.SubscribeOn(PlayerBlockedEvent.EVENT_NAME, OnPlayerBlockedEvent);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            eventSlot.Dispose();
        }

        private void Start()
        {
            var userProfile = UserProfileLocalDataManager.Instance.GetLocalUserProfile();
            Initialize(userProfile.accountId);
        }
    }
}
