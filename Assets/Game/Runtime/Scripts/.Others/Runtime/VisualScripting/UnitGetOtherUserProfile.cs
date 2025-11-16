using System.Threading.Tasks;
using Kumu.Kulitan.Backend;
using Unity.VisualScripting;

namespace Kumu.Kulitan.VisualScripting
{
    public class UnitGetOtherUserProfile : UnitServiceBase<GetOtherUserProfileResult>
    {
        [DoNotSerialize]
        public ValueInput accountId;

        [DoNotSerialize]
        public ValueInput profileConstraints;

        protected override Task<ServiceResultWrapper<GetOtherUserProfileResult>> GetServiceOperation()
        {
            var request = new GetOtherUserProfileRequest
            {
                userId = flow.GetValue<string>(accountId),
                profileConstraints = flow.GetValue<string[]>(profileConstraints)
            };

            return Services.SocialService.GetOtherUserProfileAsync(request);
        }

        protected override void Definition()
        {
            base.Definition();
            accountId = ValueInput(nameof(accountId), string.Empty);
            profileConstraints = ValueInput<string[]>(nameof(profileConstraints), null);
        }
    }
}
