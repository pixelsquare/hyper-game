using System.Threading.Tasks;
using Kumu.Kulitan.Backend;

namespace Kumu.Kulitan.VisualScripting
{
    public class UnitRefreshTokens : UnitServiceBase<ResultBase>
    {
        protected override Task<ServiceResultWrapper<ResultBase>> GetServiceOperation()
        {
            return TokenRefresh.RefreshTokensAsync<ResultBase>(null);
        }
    }
}
