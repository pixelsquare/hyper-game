using System.Threading.Tasks;
using Kumu.Kulitan.Backend;
using NUnit.Framework;

namespace Kumu.Kulitan.Editor.Tests
{
    [TestFixture]
    public class MockedModerationServiceTests
    {
        private const string ACCOUNT_ID = "97f06af3-4a1e-4c62-95c8-0501661c91f1";
        private const string CATEGORY = "Mocked Category";
        private const string SUBCATEGORY = "Mocked Subcategory";
        private const string INFO = "Mocked Information";
        private const bool SHOULD_BLOCK = false;
        private const string PHOTON_ROOM_ID = "Mocked Room Id";
        private const string PHOTON_ROOM_NAME = "Mocked Room Name";
        private const string USER_ID = "97f06af3-4a1e-4c62-95c8-0501661c91f1";

        private MockedModerationService MockedModerationService => moderationService as MockedModerationService;

        private IModerationService moderationService;
        private ReportUserRequest reportUserRequest;
        private ReportHangoutRequest reportHangoutRequest;
        private BlockPlayerRequest blockPlayerRequest;
        private UnblockPlayerRequest unblockPlayerRequest;

        [OneTimeSetUp]
        public void Init()
        {
            moderationService = new MockedModerationService
            {
                ResponseTimeInMilliseconds = 0
            };

            reportUserRequest = new ReportUserRequest
            {
                accountId = ACCOUNT_ID,
                category = CATEGORY,
                subcategory = SUBCATEGORY,
                info = INFO,
                shouldBlock = SHOULD_BLOCK
            };

            reportHangoutRequest = new ReportHangoutRequest
            {
                accountId = ACCOUNT_ID,
                photonRoomId = PHOTON_ROOM_ID,
                photonRoomName = PHOTON_ROOM_NAME,
                category = CATEGORY,
                subcategory = SUBCATEGORY,
                info = INFO,
                shouldBlock = SHOULD_BLOCK
            };

            blockPlayerRequest = new BlockPlayerRequest
            {
                userId = USER_ID
            };

            unblockPlayerRequest = new UnblockPlayerRequest
            {
                userId = USER_ID
            };
        }

        [SetUp]
        public void Setup()
        {
            MockedModerationService.ErrorFlags = MockedModerationService.ResultErrorFlags.None;
        }

    #region Report User Async

        [Test]
        public async Task ReportUserAsync_HasNoError_WhenErrorFlagsIsNone()
        {
            var result = await moderationService.ReportUserAsync(reportUserRequest);
            Assert.That(result.HasError, Is.False);
            Assert.That(result.Error, Is.Null);
        }

        [Test]
        public async Task ReportUserAsync_HasUnknownError_WhenUnknownErrorFlagIsSet()
        {
            MockedModerationService.ErrorFlags = MockedModerationService.ResultErrorFlags.UnknownError;

            var result = await moderationService.ReportUserAsync(reportUserRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.UNKNOWN_ERROR));
        }

        [Test]
        public async Task ReportUserAsync_HasNetworkError_WhenNetworkErrorFlagsAreSet()
        {
            ServiceResultWrapper<ReportUserResult> result;

            MockedModerationService.ErrorFlags = MockedModerationService.ResultErrorFlags.NetworkTimeoutError;
            result = await moderationService.ReportUserAsync(reportUserRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.NETWORK_TIMEOUT_ERROR));

            MockedModerationService.ErrorFlags = MockedModerationService.ResultErrorFlags.NetworkUnreachableError;
            result = await moderationService.ReportUserAsync(reportUserRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.NETWORK_UNREACHABLE_ERROR));

            MockedModerationService.ErrorFlags = MockedModerationService.ResultErrorFlags.NetworkUnknownError;
            result = await moderationService.ReportUserAsync(reportUserRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.NETWORK_UNKNOWN_ERROR));
        }

        [Test]
        public async Task ReportUserAsync_HasInvalidMobileNumberError_WhenInvalidMobileNumberErrorFlagIsSet()
        {
            MockedModerationService.ErrorFlags = MockedModerationService.ResultErrorFlags.InvalidDataError;

            var result = await moderationService.ReportUserAsync(reportUserRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.INVALID_DATA));
        }

        [Test]
        public async Task ReportUserAsync_WhenAppInMaintenanceErrorFlagIsSet()
        {
            MockedModerationService.ErrorFlags = MockedModerationService.ResultErrorFlags.AppInMaintenanceError;

            var result = await moderationService.ReportUserAsync(reportUserRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.APP_IN_MAINTENANCE));
        }

    #endregion

    #region Report Hangout Async

        [Test]
        public async Task ReportHangoutAsync_HasNoError_WhenErrorFlagsIsNone()
        {
            var result = await moderationService.ReportHangoutAsync(reportHangoutRequest);
            Assert.That(result.HasError, Is.False);
            Assert.That(result.Error, Is.Null);
        }

        [Test]
        public async Task ReportHangoutAsync_HasUnknownError_WhenUnknownErrorFlagIsSet()
        {
            MockedModerationService.ErrorFlags = MockedModerationService.ResultErrorFlags.UnknownError;

            var result = await moderationService.ReportHangoutAsync(reportHangoutRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.UNKNOWN_ERROR));
        }

        [Test]
        public async Task ReportHangoutAsync_HasNetworkError_WhenNetworkErrorFlagsAreSet()
        {
            ServiceResultWrapper<ReportHangoutResult> result;

            MockedModerationService.ErrorFlags = MockedModerationService.ResultErrorFlags.NetworkTimeoutError;
            result = await moderationService.ReportHangoutAsync(reportHangoutRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.NETWORK_TIMEOUT_ERROR));

            MockedModerationService.ErrorFlags = MockedModerationService.ResultErrorFlags.NetworkUnreachableError;
            result = await moderationService.ReportHangoutAsync(reportHangoutRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.NETWORK_UNREACHABLE_ERROR));

            MockedModerationService.ErrorFlags = MockedModerationService.ResultErrorFlags.NetworkUnknownError;
            result = await moderationService.ReportHangoutAsync(reportHangoutRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.NETWORK_UNKNOWN_ERROR));
        }

        [Test]
        public async Task ReportHangoutAsync_HasInvalidMobileNumberError_WhenInvalidMobileNumberErrorFlagIsSet()
        {
            MockedModerationService.ErrorFlags = MockedModerationService.ResultErrorFlags.InvalidDataError;

            var result = await moderationService.ReportHangoutAsync(reportHangoutRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.INVALID_DATA));
        }

        [Test]
        public async Task ReportHangoutAsync_WhenAppInMaintenanceErrorFlagIsSet()
        {
            MockedModerationService.ErrorFlags = MockedModerationService.ResultErrorFlags.AppInMaintenanceError;

            var result = await moderationService.ReportHangoutAsync(reportHangoutRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.APP_IN_MAINTENANCE));
        }

    #endregion

    #region Block Player Async

        [Test]
        public async Task BlockPlayerAsync_HasNoError_WhenErrorFlagsIsNone()
        {
            var result = await moderationService.BlockPlayerAsync(blockPlayerRequest);
            Assert.That(result.HasError, Is.False);
            Assert.That(result.Error, Is.Null);
        }

        [Test]
        public async Task BlockPlayerAsync_HasUnknownError_WhenUnknownErrorFlagIsSet()
        {
            MockedModerationService.ErrorFlags = MockedModerationService.ResultErrorFlags.UnknownError;

            var result = await moderationService.BlockPlayerAsync(blockPlayerRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.UNKNOWN_ERROR));
        }

        [Test]
        public async Task BlockPlayerAsync_HasNetworkError_WhenNetworkErrorFlagsAreSet()
        {
            ServiceResultWrapper<BlockPlayerResult> result;

            MockedModerationService.ErrorFlags = MockedModerationService.ResultErrorFlags.NetworkTimeoutError;
            result = await moderationService.BlockPlayerAsync(blockPlayerRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.NETWORK_TIMEOUT_ERROR));

            MockedModerationService.ErrorFlags = MockedModerationService.ResultErrorFlags.NetworkUnreachableError;
            result = await moderationService.BlockPlayerAsync(blockPlayerRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.NETWORK_UNREACHABLE_ERROR));

            MockedModerationService.ErrorFlags = MockedModerationService.ResultErrorFlags.NetworkUnknownError;
            result = await moderationService.BlockPlayerAsync(blockPlayerRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.NETWORK_UNKNOWN_ERROR));
        }

        [Test]
        public async Task BlockPlayerAsync_HasInvalidMobileNumberError_WhenInvalidMobileNumberErrorFlagIsSet()
        {
            MockedModerationService.ErrorFlags = MockedModerationService.ResultErrorFlags.InvalidDataError;

            var result = await moderationService.BlockPlayerAsync(blockPlayerRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.INVALID_DATA));
        }

        [Test]
        public async Task BlockPlayerAsync_WhenAppInMaintenanceErrorFlagIsSet()
        {
            MockedModerationService.ErrorFlags = MockedModerationService.ResultErrorFlags.AppInMaintenanceError;

            var result = await moderationService.BlockPlayerAsync(blockPlayerRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.APP_IN_MAINTENANCE));
        }

    #endregion

    #region Unblock Player Async

        [Test]
        public async Task UnblockPlayerAsync_HasNoError_WhenErrorFlagsIsNone()
        {
            var result = await moderationService.UnblockPlayerAsync(unblockPlayerRequest);
            Assert.That(result.HasError, Is.False);
            Assert.That(result.Error, Is.Null);
        }

        [Test]
        public async Task UnblockPlayerAsync_HasUnknownError_WhenUnknownErrorFlagIsSet()
        {
            MockedModerationService.ErrorFlags = MockedModerationService.ResultErrorFlags.UnknownError;

            var result = await moderationService.UnblockPlayerAsync(unblockPlayerRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.UNKNOWN_ERROR));
        }

        [Test]
        public async Task UnblockPlayerAsync_HasNetworkError_WhenNetworkErrorFlagsAreSet()
        {
            ServiceResultWrapper<UnblockPlayerResult> result;

            MockedModerationService.ErrorFlags = MockedModerationService.ResultErrorFlags.NetworkTimeoutError;
            result = await moderationService.UnblockPlayerAsync(unblockPlayerRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.NETWORK_TIMEOUT_ERROR));

            MockedModerationService.ErrorFlags = MockedModerationService.ResultErrorFlags.NetworkUnreachableError;
            result = await moderationService.UnblockPlayerAsync(unblockPlayerRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.NETWORK_UNREACHABLE_ERROR));

            MockedModerationService.ErrorFlags = MockedModerationService.ResultErrorFlags.NetworkUnknownError;
            result = await moderationService.UnblockPlayerAsync(unblockPlayerRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.NETWORK_UNKNOWN_ERROR));
        }

        [Test]
        public async Task UnblockPlayerAsync_HasInvalidMobileNumberError_WhenInvalidMobileNumberErrorFlagIsSet()
        {
            MockedModerationService.ErrorFlags = MockedModerationService.ResultErrorFlags.InvalidDataError;

            var result = await moderationService.UnblockPlayerAsync(unblockPlayerRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.INVALID_DATA));
        }

        [Test]
        public async Task UnblockPlayerAsync_WhenAppInMaintenanceErrorFlagIsSet()
        {
            MockedModerationService.ErrorFlags = MockedModerationService.ResultErrorFlags.AppInMaintenanceError;

            var result = await moderationService.UnblockPlayerAsync(unblockPlayerRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.APP_IN_MAINTENANCE));
        }

    #endregion
    }
}
