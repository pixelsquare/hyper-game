using System;
using System.Threading.Tasks;
using Kumu.Kulitan.Gifting;
using UnityEngine;

namespace Kumu.Kulitan.Backend
{
    public class MockedVirtualGiftServiceMono : MockedServiceMono, IVirtualGiftService
    {
        [SerializeField] private MockedVirtualGiftService.ResultErrorFlags errorFlags;
        [SerializeField] private int responseTimeInMilliseconds = 1;
        [SerializeField] private int virtualGiftNotificationDelayInMilliseconds = 1;
        [SerializeField] private SendVirtualGiftRequest internalRequest;
        [SerializeField] private bool overrideGifterValue;
        [SerializeField] private bool overrideGifteesValue;

        public SendVirtualGiftRequest InternalRequest => internalRequest;
        public bool OverrideGifterValue => overrideGifterValue;
        public bool OverrideGifteesValue => overrideGifteesValue;
        public int VirtualGiftNotificationDelayInMilliseconds => virtualGiftNotificationDelayInMilliseconds;

        private readonly MockedVirtualGiftService service = new();

        public async Task<ServiceResultWrapper<GetVirtualGiftCostsResult>> GetVirtualGiftCostsAsync(GetVirtualGiftCostsRequest request)
        {
            ConfigService();
            return await service.GetVirtualGiftCostsAsync(request);
        }

        public async Task<ServiceResultWrapper<SendVirtualGiftResult>> SendVirtualGiftAsync(SendVirtualGiftRequest request)
        {
            ConfigService();
            return await service.SendVirtualGiftAsync(request);
        }

        public Action<VirtualGiftEventInfo> VirtualGiftReceivedEvent
        {
            get => service.VirtualGiftReceivedEvent;
            set => service.VirtualGiftReceivedEvent = value;
        }

        public void InvokeVirtualGiftReceivedEvent(SendVirtualGiftRequest request, string gifter = default)
        {
            service.InvokeVirtualGiftReceivedEvent(request, gifter);
        }

        private void ConfigService()
        {
            service.ErrorFlags = errorFlags;
            service.ResponseTimeInMilliseconds = responseTimeInMilliseconds;
            service.VirtualGiftNotificationDelayInMilliseconds = virtualGiftNotificationDelayInMilliseconds;
        }
    }
}
