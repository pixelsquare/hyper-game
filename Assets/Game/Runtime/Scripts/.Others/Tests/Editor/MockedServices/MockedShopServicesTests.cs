using System.Threading.Tasks;
using Kumu.Kulitan.Backend;
using NUnit.Framework;

namespace Kumu.Kulitan.Editor.Tests
{
    [TestFixture]
    public class MockedShopServicesTests
    {
        private MockedShopService shopService;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            shopService = new MockedShopService
            {
                ResponseTimeInMilliseconds = 0
            };
        }

        [SetUp]
        public void SetUp()
        {
            shopService.ErrorFlags = MockedShopService.ResultErrorFlags.None;
        }

        [Test]
        public async Task BuyShopItemsAsync_HasUnknownError_WhenUnknownErrorFlagIsSet()
        {
            shopService.ErrorFlags = MockedShopService.ResultErrorFlags.UnknownError;
            
            var result = await shopService.BuyShopItemsAsync(new BuyShopItemsRequest());
            
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.UNKNOWN_ERROR));;
        }

        [Test]
        public async Task BuyShopItemsAsync_HasNetworkError_WhenNetworkErrorFlagsAreSet()
        {
            ServiceResultWrapper<BuyShopItemsResult> result;

            shopService.ErrorFlags = MockedShopService.ResultErrorFlags.NetworkTimeoutError;
            result = await shopService.BuyShopItemsAsync(new BuyShopItemsRequest());
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.NETWORK_TIMEOUT_ERROR));

            shopService.ErrorFlags = MockedShopService.ResultErrorFlags.NetworkUnknownError;
            result = await shopService.BuyShopItemsAsync(new BuyShopItemsRequest());
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.NETWORK_UNKNOWN_ERROR));

            shopService.ErrorFlags = MockedShopService.ResultErrorFlags.NetworkUnreachableError;
            result = await shopService.BuyShopItemsAsync(new BuyShopItemsRequest());
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.NETWORK_UNREACHABLE_ERROR));
        }

        [Test]
        public async Task BuyShopItemsAsync_HasAppInMaintenanceError_WhenErrorFlagIsSet()
        {
            shopService.ErrorFlags = MockedShopService.ResultErrorFlags.AppInMaintenanceError;

            var result = await shopService.BuyShopItemsAsync(new BuyShopItemsRequest());
            
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.APP_IN_MAINTENANCE));
        }

        [Test]
        public async Task GetShopItemCostsAsync_HasUnknownError_WhenUnknownErrorFlagIsSet()
        {
            shopService.ErrorFlags = MockedShopService.ResultErrorFlags.UnknownError;

            var result = await shopService.GetShopItemCostsAsync(new GetShopItemCostsRequest());
            
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.UNKNOWN_ERROR));;
        }

        [Test]
        public async Task GetShopItemCostsAsync_HasNetworkError_WhenNetworkErrorFlagsAreSet()
        {
            ServiceResultWrapper<GetShopItemsCostResult> result;

            shopService.ErrorFlags = MockedShopService.ResultErrorFlags.NetworkTimeoutError;
            result = await shopService.GetShopItemCostsAsync(new GetShopItemCostsRequest());
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.NETWORK_TIMEOUT_ERROR));
            
            shopService.ErrorFlags = MockedShopService.ResultErrorFlags.NetworkUnknownError;
            result = await shopService.GetShopItemCostsAsync(new GetShopItemCostsRequest());
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.NETWORK_UNKNOWN_ERROR));
            
            shopService.ErrorFlags = MockedShopService.ResultErrorFlags.NetworkUnreachableError;
            result = await shopService.GetShopItemCostsAsync(new GetShopItemCostsRequest());
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.NETWORK_UNREACHABLE_ERROR));
        }

        [Test]
        public async Task GetShopItemCostsAsync_HasAppInMaintenanceError_WhenErrorFlagIsSet()
        {
            shopService.ErrorFlags = MockedShopService.ResultErrorFlags.AppInMaintenanceError;

            var result = await shopService.GetShopItemCostsAsync(new GetShopItemCostsRequest());
            
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.APP_IN_MAINTENANCE));
        }
    }
}
