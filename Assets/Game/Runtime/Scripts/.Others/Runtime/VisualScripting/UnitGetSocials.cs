using System.Threading.Tasks;
using Kumu.Kulitan.Backend;

namespace Kumu.Kulitan.VisualScripting
{
    public class UnitGetSocials : UnitServiceBase<GetSocialRelationshipsResult>
    {
        protected override Task<ServiceResultWrapper<GetSocialRelationshipsResult>> GetServiceOperation()
        {
            return Services.SocialService.GetSocialRelationshipsAsync(new GetSocialRelationshipsRequest());
        }
    }
    
    public class UnitGetFriends : UnitServiceBase<GetSocialRelationshipsResult>
    {
        protected override Task<ServiceResultWrapper<GetSocialRelationshipsResult>> GetServiceOperation()
        {
            return Services.SocialService.GetSocialRelationshipsAsync(new GetSocialRelationshipsRequest
            {
                userConstraint = "friends"
            });
        }
    }
    
    public class UnitGetFollowers : UnitServiceBase<GetSocialRelationshipsResult>
    {
        protected override Task<ServiceResultWrapper<GetSocialRelationshipsResult>> GetServiceOperation()
        {
            return Services.SocialService.GetSocialRelationshipsAsync(new GetSocialRelationshipsRequest
            {
                userConstraint = "followers"
            });
        }
    }
    
    public class UnitGetFollowing : UnitServiceBase<GetSocialRelationshipsResult>
    {
        protected override Task<ServiceResultWrapper<GetSocialRelationshipsResult>> GetServiceOperation()
        {
            return Services.SocialService.GetSocialRelationshipsAsync(new GetSocialRelationshipsRequest
            {
                userConstraint = "following"
            });
        }
    }
}
