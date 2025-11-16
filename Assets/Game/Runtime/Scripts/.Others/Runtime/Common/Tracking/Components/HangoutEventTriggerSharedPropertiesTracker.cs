using System;
using System.Threading;
using System.Threading.Tasks;
using Kumu.Extensions;
using Kumu.Kulitan.Backend;
using Kumu.Kulitan.Gifting;
using Kumu.Kulitan.Multiplayer;
using UnityEngine;

namespace Kumu.Kulitan.Tracking.Components
{
    public class HangoutEventTriggerSharedPropertiesTracker
    {
        public string HangoutId { get; private set; }
        public string SessionId { get; private set; }
        public int DiamondsReceived { get; private set; }
            
        private float startTime;
        private int startFollowers;
        private Task<int> getStartFollowersTask;

        public HangoutEventTriggerSharedPropertiesTracker()
        {
            HangoutId = ConnectionManager.Client.CurrentRoom.GetRoomDetails().layoutName;
            SessionId = ConnectionManager.Client.CurrentRoom.Name;
            startTime = Time.time;
            Services.VirtualGiftService.VirtualGiftReceivedEvent += OnVirtualGiftReceivedEvent;
            StartTrackingAsync();
        }

        private async void StartTrackingAsync()
        {
            try
            {
                getStartFollowersTask = GetFollowersAsync(CancellationToken.None);
                await getStartFollowersTask;
                startFollowers = getStartFollowersTask.Result;
            }
            catch (Exception e)
            {
                $"[Follower] Error Message:{e.Message}, Source:{e.Source}, Trace:{e.StackTrace}".LogError();
                startFollowers = 0;
            }
        }

        public async Task EndTrackingAsync(Action<HangoutSharedPropertiesDetails> callback = null)
        {
            Services.VirtualGiftService.VirtualGiftReceivedEvent -= OnVirtualGiftReceivedEvent;
            var roomDuration = Time.time - startTime;

            var followersGained = 0;
            
            try
            {
                if (getStartFollowersTask != null && getStartFollowersTask.IsCanceled)
                {
                    followersGained = 0;
                }
                else
                {
                    if (getStartFollowersTask != null && !getStartFollowersTask.IsCompleted)
                    {
                        await getStartFollowersTask;
                    }
                
                    var endFollowers = await GetFollowersAsync(CancellationToken.None);
                    followersGained = endFollowers - startFollowers;
                }
            }
            catch (Exception e)
            {
                $"Error! Message: {e.Message}, Trace: {e.StackTrace}".LogError();
                followersGained = 0;
            }
            finally
            {
                var hangoutSharedPropertiesDetails = new HangoutSharedPropertiesDetails(HangoutId, SessionId, roomDuration, DiamondsReceived, followersGained);
                callback?.Invoke(hangoutSharedPropertiesDetails);
            }
        }

        private async Task<int> GetFollowersAsync(CancellationToken cToken)
        {
            if (cToken.IsCancellationRequested)
            {
                "GetFollowersAsync canceled!".LogError();
                return 0;
            }
            
            Task<ServiceResultWrapper<GetUserProfileResult>> task = null;
            
            try
            {
                task = Services.UserProfileService.GetUserProfileAsync(new GetUserProfileRequest());
                await task;
            }
            catch (Exception e)
            {
                $"[Follower] Exception: {e.Message}".LogError();
                return 0;
            }

            if (task.IsCanceled)
            {
                "GetFollowersAsync canceled!".LogError();
                return 0;
            }

            return task.Result.Result.UserProfile.FollowerCountToDisplay;
        }

        #region Event handlers

        private void OnVirtualGiftReceivedEvent(VirtualGiftEventInfo vgEvent)
        {
            DiamondsReceived += vgEvent.ComputeOwnShare();
        }

        #endregion

        public struct HangoutSharedPropertiesDetails
        {
            public string hangoutId;
            public string sessionId;
            public float roomDuration;
            public int diamondsReceived;
            public int followersGained;

            public HangoutSharedPropertiesDetails(string hangoutId, string sessionId, float roomDuration, int diamondsReceived, int followersGained)
            {
                this.hangoutId = hangoutId;
                this.sessionId = sessionId;
                this.roomDuration = roomDuration;
                this.diamondsReceived = diamondsReceived;
                this.followersGained = followersGained;
            }

            public override string ToString()
            {
                return $"HangoutId: {hangoutId}\n" +
                       $"SessionId: {sessionId}\n" +
                       $"RoomDuration: {roomDuration}\n" +
                       $"DiamondsReceived: {diamondsReceived}\n" +
                       $"FollowersGained: {followersGained}";
            }
        }
    }
}
