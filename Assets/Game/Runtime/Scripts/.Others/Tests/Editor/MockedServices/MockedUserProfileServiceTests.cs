using System.Threading.Tasks;
using Kumu.Kulitan.Backend;
using NUnit.Framework;

namespace Kumu.Kulitan.Editor.Tests
{
    [TestFixture]
    public class MockedUserProfileServiceTests
    {
        private MockedUserProfileService profileService;
        private CreateUserProfileRequest createUserProfileRequest;

        private UserProfile defaultProfile;
        private const string mobile = "+649479999999";
        private const string name = "johnsmith";
        private const string nick = "jack";
        private const int ageRange = 0;
        private const int gender = 1;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            profileService = new MockedUserProfileService
            {
                ResponseTimeInMilliseconds = 0
            };
            defaultProfile = MockedServicesUtil.GetDefaultProfile();
            createUserProfileRequest = new CreateUserProfileRequest
            {
                username = name,
                nickname = nick,
                ageRange = ageRange,
                gender = gender
            };
        }

        [SetUp]
        public void SetUp()
        {
            profileService.ErrorFlags = MockedUserProfileService.ResultErrorFlags.None;
            profileService.HasLinkedKumuAccount = false;
        }

        [TearDown]
        public void TearDown()
        {
            MockedServicesUtil.ClearMockedProfileInPrefs();
        }

        #region GetUserProfileAsync

        [Test]
        public async Task GetUserProfileAsync_HasNoErrors_WhenErrorFlagIsNone()
        {
            var result = await profileService.GetUserProfileAsync(new GetUserProfileRequest());

            Assert.That(result.HasError, Is.False);
            Assert.That(result.Error, Is.Null);
        }

        [Test]
        public async Task GetUserProfileAsync_HasUnknownError_WhenUnknownErrorFlagIsSet()
        {
            profileService.ErrorFlags = MockedUserProfileService.ResultErrorFlags.UnknownError;

            var result = await profileService.GetUserProfileAsync(new GetUserProfileRequest());

            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.UNKNOWN_ERROR));
        }

        [Test]
        public async Task GetUserProfileAsync_HasNetworkErrors_WhenNetworkErrorsFlagsAreSet()
        {
            ServiceResultWrapper<GetUserProfileResult> result;

            profileService.ErrorFlags = MockedUserProfileService.ResultErrorFlags.NetworkUnknownError;
            result = await profileService.GetUserProfileAsync(new GetUserProfileRequest());
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.NETWORK_UNKNOWN_ERROR));
            
            profileService.ErrorFlags = MockedUserProfileService.ResultErrorFlags.NetworkTimeoutError;
            result = await profileService.GetUserProfileAsync(new GetUserProfileRequest());
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.NETWORK_TIMEOUT_ERROR));
            
            profileService.ErrorFlags = MockedUserProfileService.ResultErrorFlags.NetworkUnreachableError;
            result = await profileService.GetUserProfileAsync(new GetUserProfileRequest());
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.NETWORK_UNREACHABLE_ERROR));
        }

        [Test]
        public async Task GetUserProfileAsync_ReturnsDefaultProfile_WhenNotSignedUp()
        {
            var result = await profileService.GetUserProfileAsync(new GetUserProfileRequest());

            Assert.That(result.HasError, Is.False);
            Assert.That(result.Result.UserProfile.userName, Is.EqualTo(defaultProfile.userName));
            Assert.That(result.Result.UserProfile.nickName, Is.EqualTo(defaultProfile.nickName));
            Assert.That(result.Result.UserProfile.gender, Is.EqualTo(defaultProfile.gender));
            Assert.That(result.Result.UserProfile.ageRange, Is.EqualTo(defaultProfile.ageRange));
        }

        [Test]
        public async Task GetUserProfileAsync_ReturnsProfileFromPrefs_WhenSignedUp()
        {
            var authService = new MockedAuthService();
            await authService.RegisterUserRequestOtpAsync(new RegisterUserRequestOtpRequest
            {
                mobile = mobile
            });
            await authService.RegisterUserSendOtpAsync(new RegisterUserSendOtpRequest
            {
                mobile = mobile,
                otp = "000000"
            });
            await profileService.CreateUserProfileAsync(createUserProfileRequest);

            var result = await profileService.GetUserProfileAsync(new GetUserProfileRequest());

            Assert.That(result.HasError, Is.False);
            Assert.That(result.Result.UserProfile.mobile, Is.EqualTo(mobile));
            Assert.That(result.Result.UserProfile.userName, Is.EqualTo(name));
            Assert.That(result.Result.UserProfile.nickName, Is.EqualTo(nick));
            Assert.That(result.Result.UserProfile.ageRange, Is.EqualTo(ageRange));
            Assert.That(result.Result.UserProfile.gender, Is.EqualTo(gender));
        }

        [Test]
        public async Task GetUserProfileAsync_HasLinkedKumuAccount_IfSetToTrue()
        {
            ServiceResultWrapper<GetUserProfileResult> result;
            
            profileService.HasLinkedKumuAccount = true;
            result = await profileService.GetUserProfileAsync(new GetUserProfileRequest());
            Assert.That(result.Result.UserProfile.hasLinkedKumuAccount);
            
            profileService.HasLinkedKumuAccount = false;
            result = await profileService.GetUserProfileAsync(new GetUserProfileRequest());
            Assert.That(!result.Result.UserProfile.hasLinkedKumuAccount);
        }

        #endregion

        #region UpdateUserProfileAsync

        [Test]
        public async Task CreateUserProfileAsync_HasNoError_WhenErrorFlagIsNone()
        {
            var result = await profileService.CreateUserProfileAsync(createUserProfileRequest);
            
            Assert.That(result.HasError, Is.False);
            Assert.That(result.Error, Is.Null);
        }
        
        [Test]
        public async Task CreateUserProfileAsync_HasProfileInPlayerPrefs_WhenErrorFlagIsNone()
        {
            var authService = new MockedAuthService();

            await authService.RegisterUserSendOtpAsync(new RegisterUserSendOtpRequest
            {
                mobile = mobile,
                otp = "000000"
            });
            var result = await profileService.CreateUserProfileAsync(createUserProfileRequest);
            var profileInPrefs = MockedServicesUtil.GetMockedProfileInPrefs();
            
            Assert.That(result.HasError, Is.False);
            Assert.That(profileInPrefs.mobile, Is.EqualTo(mobile));
            Assert.That(profileInPrefs.userName, Is.EqualTo(name));
            Assert.That(profileInPrefs.nickName, Is.EqualTo(nick));
            Assert.That(profileInPrefs.ageRange, Is.EqualTo(ageRange));
            Assert.That(profileInPrefs.gender, Is.EqualTo(gender));
        }
        
        [Test]
        public async Task CreateUserProfileAsync_HasNetworkError_WhenNetworkErrorFlagsAreSet()
        {
            ServiceResultWrapper<CreateUserProfileResult> result;

            profileService.ErrorFlags = MockedUserProfileService.ResultErrorFlags.NetworkTimeoutError;
            result = await profileService.CreateUserProfileAsync(createUserProfileRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.NETWORK_TIMEOUT_ERROR));
            
            profileService.ErrorFlags = MockedUserProfileService.ResultErrorFlags.NetworkUnknownError;
            result = await profileService.CreateUserProfileAsync(createUserProfileRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.NETWORK_UNKNOWN_ERROR));
            
            profileService.ErrorFlags = MockedUserProfileService.ResultErrorFlags.NetworkUnreachableError;
            result = await profileService.CreateUserProfileAsync(createUserProfileRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.NETWORK_UNREACHABLE_ERROR));
        }
        
        [Test]
        public async Task CreateUserProfileAsync_HasProfileAlreadyExistsError_WhenProfileAlreadyUpdatedExistsFlagIsSet()
        {
            profileService.ErrorFlags = MockedUserProfileService.ResultErrorFlags.ProfileAlreadyExistsError;
            
            var result = await profileService.CreateUserProfileAsync(createUserProfileRequest);
            
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.PROFILE_ALREADY_EXISTS));
        }
        
        [Test]
        public async Task CreateUserProfileAsync_HasUserNameExhaustedError_WhenUserNameExhaustedErrorFlagIsSet()
        {
            profileService.ErrorFlags = MockedUserProfileService.ResultErrorFlags.UserNameExhaustedError;
            
            var result = await profileService.CreateUserProfileAsync(createUserProfileRequest);
            
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.USERNAME_EXHAUSTED));
        }

        #endregion
    }
}
