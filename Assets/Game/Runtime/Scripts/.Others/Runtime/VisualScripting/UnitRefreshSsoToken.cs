using System.Threading.Tasks;
using Kumu.Kulitan.Backend;

namespace Kumu.Kulitan.VisualScripting
{
    public class UnitRefreshSsoToken : UnitServiceBase<ResultBase>
    {
        protected override Task<ServiceResultWrapper<ResultBase>> GetServiceOperation()
        {
            return TokenRefresh.RefreshSsoTokensAsync<ResultBase>(null);
        }
    }
}
