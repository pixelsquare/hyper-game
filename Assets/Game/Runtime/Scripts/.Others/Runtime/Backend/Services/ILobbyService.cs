using System.Threading.Tasks;

namespace Kumu.Kulitan.Backend
{
    public interface ILobbyService
    {
        public Task<ServiceResultWrapper<GetLobbyConfigResult>> GetLobbyConfigsAsync(GetLobbyConfigsRequest request);
    }
}
