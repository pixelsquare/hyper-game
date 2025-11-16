using System;
using System.Threading.Tasks;
using Kumu.Kulitan.Gifting;

namespace Kumu.Kulitan.Backend
{
    public interface IVirtualGiftService
    {
        public Task<ServiceResultWrapper<GetVirtualGiftCostsResult>> GetVirtualGiftCostsAsync(GetVirtualGiftCostsRequest request);
        public Task<ServiceResultWrapper<SendVirtualGiftResult>> SendVirtualGiftAsync(SendVirtualGiftRequest request);
        public Action<VirtualGiftEventInfo> VirtualGiftReceivedEvent { get; set; }
    }
}
