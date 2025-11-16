using System.Threading.Tasks;
using Kumu.Kulitan.Backend;
using NUnit.Framework;

namespace Kumu.Kulitan.Editor.Tests
{
    [TestFixture]
    public class MockedInventoryServiceTests
    {
        private MockedInventoryService inventoryService;
        
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            inventoryService = new MockedInventoryService
            {
                ResponseTimeInMilliseconds = 1
            };
        }

        [SetUp]
        public void SetUp()
        {
            inventoryService.ErrorFlags = MockedInventoryService.ResultErrorFlags.None;
        }

        [Test]
        public async Task GetInventoryAsync_HasUnknownError_WhenUnknownErrorFlagIsSet()
        {
            inventoryService.ErrorFlags = MockedInventoryService.ResultErrorFlags.UnknownError;
            
            var result = await inventoryService.GetInventoryAsync(new GetInventoryRequest());
            
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.UNKNOWN_ERROR));
        }
        
        [Test]
        public async Task GetInventoryAsync_HasNetworkError_WhenNetworkErrorFlagsAreSet()
        {
            ServiceResultWrapper<GetInventoryResult> result;

            inventoryService.ErrorFlags = MockedInventoryService.ResultErrorFlags.NetworkTimeoutError;
            result = await inventoryService.GetInventoryAsync(new GetInventoryRequest());
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.NETWORK_TIMEOUT_ERROR));

            inventoryService.ErrorFlags = MockedInventoryService.ResultErrorFlags.NetworkUnknownError;
            result = await inventoryService.GetInventoryAsync(new GetInventoryRequest());
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.NETWORK_UNKNOWN_ERROR));

            inventoryService.ErrorFlags = MockedInventoryService.ResultErrorFlags.NetworkUnreachableError;
            result = await inventoryService.GetInventoryAsync(new GetInventoryRequest());
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.NETWORK_UNREACHABLE_ERROR));
        }

        [Test]
        public async Task GetInventoryAsync_HasAppInMaintenanceModeError_WhenErrorFlagIsSet()
        {
            inventoryService.ErrorFlags = MockedInventoryService.ResultErrorFlags.AppInMaintenanceError;

            var result = await inventoryService.GetInventoryAsync(new GetInventoryRequest());
            
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.APP_IN_MAINTENANCE));
        }

        [Test]
        public async Task SetEquippedItemsAsync_HasUnknownError_WhenUnknownErrorFlagIsSet()
        {
            inventoryService.ErrorFlags = MockedInventoryService.ResultErrorFlags.UnknownError;

            var result = await inventoryService.EquipItemsAsync(new EquipItemsRequest());
            
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.UNKNOWN_ERROR));
        }
        
        [Test]
        public async Task SetEquippedItemsAsync_HasNetworkError_WhenNetworkErrorFlagsAreSet()
        {
            ServiceResultWrapper<EquipItemsResult> result;

            inventoryService.ErrorFlags = MockedInventoryService.ResultErrorFlags.NetworkTimeoutError;
            result = await inventoryService.EquipItemsAsync(new EquipItemsRequest());
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.NETWORK_TIMEOUT_ERROR));

            inventoryService.ErrorFlags = MockedInventoryService.ResultErrorFlags.NetworkUnknownError;
            result = await inventoryService.EquipItemsAsync(new EquipItemsRequest());
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.NETWORK_UNKNOWN_ERROR));

            inventoryService.ErrorFlags = MockedInventoryService.ResultErrorFlags.NetworkUnreachableError;
            result = await inventoryService.EquipItemsAsync(new EquipItemsRequest());
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.NETWORK_UNREACHABLE_ERROR));
        }

        [Test]
        public async Task SetEquippedItemsAsync_HasAppInMaintenanceModeError_WhenErrorFlagIsSet()
        {
            inventoryService.ErrorFlags = MockedInventoryService.ResultErrorFlags.AppInMaintenanceError;

            var result = await inventoryService.EquipItemsAsync(new EquipItemsRequest());
            
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.APP_IN_MAINTENANCE));
        }

        [Test]
        public async Task GetWalletBalanceAsync_HasNoErrors_WhenNoErrorFlagsAreSet()
        {
            inventoryService.ErrorFlags = MockedInventoryService.ResultErrorFlags.None;

            var result = await inventoryService.GetWalletBalanceAsync(new GetWalletBalanceRequest());
            
            Assert.That(result.HasError, Is.False);
        }

        [Test]
        public async Task GetWalletBalanceAsync_HasNetworkError_WhenFlagsAreSet()
        {
            ServiceResultWrapper<GetWalletBalanceResult> result;
            
            inventoryService.ErrorFlags = MockedInventoryService.ResultErrorFlags.NetworkTimeoutError;
            result = await inventoryService.GetWalletBalanceAsync(new GetWalletBalanceRequest());
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.NETWORK_TIMEOUT_ERROR));

            inventoryService.ErrorFlags = MockedInventoryService.ResultErrorFlags.NetworkUnknownError;
            result = await inventoryService.GetWalletBalanceAsync(new GetWalletBalanceRequest());
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.NETWORK_UNKNOWN_ERROR));

            inventoryService.ErrorFlags = MockedInventoryService.ResultErrorFlags.NetworkUnreachableError;
            result = await inventoryService.GetWalletBalanceAsync(new GetWalletBalanceRequest());
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.NETWORK_UNREACHABLE_ERROR));
        }

        [Test]
        public async Task GetWalletBalanceAsync_HasUnknownError_WhenFlagIsSet()
        {
            inventoryService.ErrorFlags = MockedInventoryService.ResultErrorFlags.UnknownError;

            var result = await inventoryService.GetWalletBalanceAsync(new GetWalletBalanceRequest());
            
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.UNKNOWN_ERROR));
        }
    }
}
