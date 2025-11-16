using Kumu.Kulitan.Avatar;

namespace Kumu.Kulitan.Backend
{
    public struct OtherUserProfile
    {
        public UserProfile profile;
        public SocialState social_state;
        public string room_id; // should only be returned under certain circumstances
        public AvatarItemState[] equipped_items; // equipped items for the purposes of assembling avatar preview in profile screen
    }
}
