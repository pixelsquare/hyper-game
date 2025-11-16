using Kumu.Kulitan.Events;

namespace Kumu.Kulitan.Tracking
{
    public class UserJourneyEvent : Event<string>
    {
        public const string EVENT_ID = "UserJourneyEvent";

        public enum Checkpoint
        {
            LandingPage,
            MobileNumber,
            Otp,
            Age,
            Gender,
            Username,
            Nickname,
            AvatarCustomization,
            JoinHangout,
            SocialScreen,
            UserProfile,
        }

        public UserJourneyEvent(Checkpoint checkpoint) : base(EVENT_ID)
        {
            Journey = checkpoint;
        }

        public Checkpoint Journey { get; }
    }
    
    public interface IUserJourneyHandle
    {
        public void OnUserJourneyEvent(UserJourneyEvent eventData);
    }
}
