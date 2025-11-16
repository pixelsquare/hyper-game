using System.Threading.Tasks;
using Kumu.Kulitan.Backend;
using NUnit.Framework;

namespace Kumu.Kulitan.Editor.Tests
{
    [TestFixture]
    public class MockedSocialServiceTests
    {
        private const string USER_ID = "97f06af3-4a1e-4c62-95c8-0501661c91f1";

        private MockedSocialService MockedSocialService => socialService as MockedSocialService;

        private ISocialService socialService;

        private GetOtherUserProfileRequest getOtherUserProfileRequest;

        [OneTimeSetUp]
        public void Init()
        {
            getOtherUserProfileRequest = new GetOtherUserProfileRequest
            {
                userId = USER_ID
            };
        }

        [SetUp]
        public void Setup()
        {
            MockedSocialService.ErrorFlags = MockedSocialService.ResultErrorFlags.None;
        }

    #region Get Other User Profile Async

        [Test]
        public async Task GetOtherUserProfile_HasNoError_WhenErrorFlagsIsNone()
        {
            var result = await socialService.GetOtherUserProfileAsync(getOtherUserProfileRequest);
            Assert.That(result.HasError, Is.False);
            Assert.That(result.Error, Is.Null);
        }

        [Test]
        public async Task GetOtherUserProfile_HasUnknownError_WhenUnknownErrorFlagIsSet()
        {
            MockedSocialService.ErrorFlags = MockedSocialService.ResultErrorFlags.UnknownError;

            var result = await socialService.GetOtherUserProfileAsync(getOtherUserProfileRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.UNKNOWN_ERROR));
        }

        [Test]
        public async Task GetOtherUserProfile_HasNetworkError_WhenNetworkErrorFlagsAreSet()
        {
            ServiceResultWrapper<GetOtherUserProfileResult> result;

            MockedSocialService.ErrorFlags = MockedSocialService.ResultErrorFlags.NetworkTimeoutError;
            result = await socialService.GetOtherUserProfileAsync(getOtherUserProfileRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.NETWORK_TIMEOUT_ERROR));

            MockedSocialService.ErrorFlags = MockedSocialService.ResultErrorFlags.NetworkUnreachableError;
            result = await socialService.GetOtherUserProfileAsync(getOtherUserProfileRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.NETWORK_UNREACHABLE_ERROR));

            MockedSocialService.ErrorFlags = MockedSocialService.ResultErrorFlags.NetworkUnknownError;
            result = await socialService.GetOtherUserProfileAsync(getOtherUserProfileRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.NETWORK_UNKNOWN_ERROR));
        }

        [Test]
        public async Task GetOtherUserProfile_HasInvalidMobileNumberError_WhenInvalidMobileNumberErrorFlagIsSet()
        {
            MockedSocialService.ErrorFlags = MockedSocialService.ResultErrorFlags.InvalidDataError;

            var result = await socialService.GetOtherUserProfileAsync(getOtherUserProfileRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.INVALID_DATA));
        }

        [Test]
        public async Task GetOtherUserProfile_WhenAppInMaintenanceErrorFlagIsSet()
        {
            MockedSocialService.ErrorFlags = MockedSocialService.ResultErrorFlags.AppInMaintenanceError;

            var result = await socialService.GetOtherUserProfileAsync(getOtherUserProfileRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.APP_IN_MAINTENANCE));
        }

    #endregion
    }
}
