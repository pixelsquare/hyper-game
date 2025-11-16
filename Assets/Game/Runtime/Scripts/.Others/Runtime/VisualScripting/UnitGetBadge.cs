using System.Threading.Tasks;
using Kumu.Kulitan.Backend;
using Kumu.Kulitan.CDN;

namespace Kumu.Kulitan.VisualScripting
{
    public class UnitGetBadge : UnitServiceBase<GetBadgeResult>
    {
        protected override Task<ServiceResultWrapper<GetBadgeResult>> GetServiceOperation()
        {
            return Services.AuthService.GetBadgeAsync(new GetBadgeRequest());
        }

        protected override void BeforeExit()
        {
            CDNPaths.Badge = task.Result.Result.badge;
        }
    }
}
