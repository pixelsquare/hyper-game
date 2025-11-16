using System.Threading.Tasks;

namespace Kumu.Kulitan.Backend
{
    public interface IFCMService
    {
        public bool IsInitialized { get; }
        public void Init();
        public Task<ServiceResultWrapper<RegisterFCMTokenResult>> RegisterFCMTokenAsync(RegisterFCMTokenRequest request);
    }
}
