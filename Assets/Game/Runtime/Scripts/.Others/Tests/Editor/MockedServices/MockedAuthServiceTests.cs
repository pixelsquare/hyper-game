using Kumu.Kulitan.Backend;
using NUnit.Framework;
using Task = System.Threading.Tasks.Task;

namespace Kumu.Kulitan.Editor.Tests
{
    [TestFixture]
    public class MockedAuthServiceTests
    {
        private MockedAuthService authService;
        private const string KUMU_USERNAME = "username";
        private const string MOBILE = "+639470000000";
        private const string OTP = "123456";
        private RegisterUserRequestOtpRequest registerUserRequestOtpRequest;
        private RegisterUserSendOtpRequest registerUserSendOtpRequest;
        private LoginUserRequestOtpRequest loginUserRequestOtpRequest;
        private LoginUserSendOtpRequest loginUserSendOtpRequest;
        private LinkUserRequestOtpRequest linkUserRequestOtpRequest;
        private LinkUserSendOtpRequest linkUserSendOtpRequest;
        private RefreshLinkRequestOtpRequest refreshLinkRequestOtpRequest;
        private RefreshLinkSendOtpRequest refreshLinkSendOtpRequest;
        private UnlinkUserRequest unlinkUserRequest;
        private LogoutUserRequest logoutUserRequest;

        [OneTimeSetUp]
        public void Init()
        {
            authService = new MockedAuthService
            {
                ResponseTimeInMilliseconds = 0
            };
            registerUserRequestOtpRequest = new RegisterUserRequestOtpRequest
            {
                mobile = MOBILE
            };
            registerUserSendOtpRequest = new RegisterUserSendOtpRequest
            {
                mobile = MOBILE,
                otp = OTP
            };
            loginUserRequestOtpRequest = new LoginUserRequestOtpRequest
            {
                mobile = MOBILE
            };
            loginUserSendOtpRequest = new LoginUserSendOtpRequest
            {
                mobile = MOBILE,
                otp = OTP
            };
            linkUserRequestOtpRequest = new LinkUserRequestOtpRequest
            {
                username = KUMU_USERNAME
            };
            linkUserSendOtpRequest = new LinkUserSendOtpRequest
            {
                username = KUMU_USERNAME
            };
            refreshLinkRequestOtpRequest = new RefreshLinkRequestOtpRequest();
            refreshLinkSendOtpRequest = new RefreshLinkSendOtpRequest
            {
                otp = OTP
            };
            unlinkUserRequest = new UnlinkUserRequest();
            logoutUserRequest = new LogoutUserRequest();
        }

        [SetUp]
        public void SetUp()
        {
            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.None;
        }

        #region RegisterUserRequestOtpAsync

        [Test]
        public async Task RegisterUserRequestOtpAsync_HasNoError_WhenErrorFlagsIsNone()
        {
            var result = await authService.RegisterUserRequestOtpAsync(registerUserRequestOtpRequest);
            Assert.That(result.HasError, Is.False);
            Assert.That(result.Error, Is.Null);
        }

        [Test]
        public async Task RegisterUserRequestOtpAsync_HasUnknownError_WhenUnknownErrorFlagIsSet()
        {
            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.UnknownError;

            var result = await authService.RegisterUserRequestOtpAsync(registerUserRequestOtpRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.UNKNOWN_ERROR));
        }

        [Test]
        public async Task RegisterUserRequestOtpAsync_HasNetworkError_WhenNetworkErrorFlagsAreSet()
        {
            ServiceResultWrapper<RegisterUserRequestOtpResult> result;

            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.NetworkTimeoutError;
            result = await authService.RegisterUserRequestOtpAsync(registerUserRequestOtpRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.NETWORK_TIMEOUT_ERROR));

            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.NetworkUnreachableError;
            result = await authService.RegisterUserRequestOtpAsync(registerUserRequestOtpRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.NETWORK_UNREACHABLE_ERROR));

            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.NetworkUnknownError;
            result = await authService.RegisterUserRequestOtpAsync(registerUserRequestOtpRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.NETWORK_UNKNOWN_ERROR));
        }

        [Test]
        public async Task RegisterUserRequestOtpAsync_HasInvalidMobileNumberError_WhenInvalidMobileNumberErrorFlagIsSet()
        {
            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.InvalidDataError;

            var result = await authService.RegisterUserRequestOtpAsync(registerUserRequestOtpRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.INVALID_DATA));
        }

        [Test]
        public async Task RegisterUserRequestOtpAsync_HasUserAlreadyRegisteredError_WhenUserAlreadyRegisteredErrorFlagIsSet()
        {
            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.MobileAlreadyRegisteredError;

            var result = await authService.RegisterUserRequestOtpAsync(registerUserRequestOtpRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.MOBILE_ALREADY_REGISTERED));
        }

        #endregion

        #region RegisterUserSendOtpAsync

        [Test]
        public async Task RegisterUserSendOtpASync_HasNoError_WhenErrorFlagsIsNone()
        {
            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.None;

            var result = await authService.RegisterUserSendOtpAsync(registerUserSendOtpRequest);
            Assert.That(result.HasError, Is.False);
        }

        [Test]
        public async Task RegisterUserSendOtpAsync_HasUnknownError_WhenUnknownErrorFlagIsSet()
        {
            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.UnknownError;

            var result = await authService.RegisterUserSendOtpAsync(registerUserSendOtpRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.UNKNOWN_ERROR));
        }

        [Test]
        public async Task RegisterUserSendOtpAsync_HasNetworkError_WhenNetworkErrorFlagIsSet()
        {
            var request = new RegisterUserSendOtpRequest() { mobile = MOBILE, otp = OTP };
            ServiceResultWrapper<RegisterUserSendOtpResult> result;

            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.NetworkUnknownError;
            result = await authService.RegisterUserSendOtpAsync(registerUserSendOtpRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.NETWORK_UNKNOWN_ERROR));

            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.NetworkTimeoutError;
            result = await authService.RegisterUserSendOtpAsync(registerUserSendOtpRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.NETWORK_TIMEOUT_ERROR));

            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.NetworkUnreachableError;
            result = await authService.RegisterUserSendOtpAsync(registerUserSendOtpRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.NETWORK_UNREACHABLE_ERROR));
        }

        [Test]
        public async Task RegisterUserSendOtpAsync_HasInvalidMobileNumberError_WhenInvalidMobileNumberErrorFlagIsSet()
        {
            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.InvalidDataError;

            var result = await authService.RegisterUserSendOtpAsync(registerUserSendOtpRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.INVALID_DATA));
        }

        [Test]
        public async Task RegisterUserSendOtpAsync_HasMobileNumberAlreadyRegisteredError_WhenMobileNumberAlreadyRegisteredErrorIsSet()
        {
            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.MobileAlreadyRegisteredError;

            var result = await authService.RegisterUserSendOtpAsync(registerUserSendOtpRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.MOBILE_ALREADY_REGISTERED));
        }

        [Test]
        public async Task RegisterUserSendOtpAsync_HasAuthenticationError_WhenAuthenticationErrorFlagIsSet()
        {
            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.InvalidOtpError;

            var result = await authService.RegisterUserSendOtpAsync(registerUserSendOtpRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.INVALID_OTP));
        }

        #endregion

        #region LoginUserRequestOtpAsync

        [Test]
        public async Task LoginUserRequestOtpAsync_HasNoError_WhenErrorFlagsIsNone()
        {
            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.None;

            var result = await authService.LoginUserRequestOtpAsync(loginUserRequestOtpRequest);
            Assert.That(result.HasError, Is.False);
        }

        [Test]
        public async Task LoginUserRequestOtpAsync_HasUnknownError_WhenUnknownErrorFlagIsSet()
        {
            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.UnknownError;
            var result = await authService.LoginUserRequestOtpAsync(loginUserRequestOtpRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.UNKNOWN_ERROR));
        }

        [Test]
        public async Task LoginUserRequestOtpAsync_HasNetworkErrors_WhenNetworkErrorFlagsAreSet()
        {
            var request = new LoginUserRequestOtpRequest() { mobile = MOBILE };
            ServiceResultWrapper<LoginUserRequestOtpResult> result;

            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.NetworkTimeoutError;
            result = await authService.LoginUserRequestOtpAsync(loginUserRequestOtpRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.NETWORK_TIMEOUT_ERROR));

            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.NetworkUnknownError;
            result = await authService.LoginUserRequestOtpAsync(loginUserRequestOtpRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.NETWORK_UNKNOWN_ERROR));

            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.NetworkUnreachableError;
            result = await authService.LoginUserRequestOtpAsync(loginUserRequestOtpRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.NETWORK_UNREACHABLE_ERROR));
        }

        [Test]
        public async Task LoginUserRequestOtpAsync_HasInvalidMobileNumberError_WhenInvalidMobileNumberErrorFlagIsSet()
        {
            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.InvalidDataError;
            var result = await authService.LoginUserRequestOtpAsync(loginUserRequestOtpRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.INVALID_DATA));
        }

        [Test]
        public async Task LoginUserRequestOtpAsync_HasMobileNumberNotRegisteredError_WhenMobileNumberNotRegisteredErrorFlagIsSet()
        {
            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.MobileNumberNotRegisteredError;
            var result = await authService.LoginUserRequestOtpAsync(loginUserRequestOtpRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.MOBILE_NOT_REGISTERED));
        }

        #endregion

        #region LoginUserSendOtpAsync

        [Test]
        public async Task LoginUserSendOtpAsync_HasNoError_WhenErrorFlagIsNone()
        {
            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.None;

            var result = await authService.LoginUserSendOtpAsync(loginUserSendOtpRequest);
            Assert.That(result.HasError, Is.False);
            Assert.That(result.Error, Is.Null);
        }

        [Test]
        public async Task LoginUserSendOtpAsync_HasUnknownError_WhenUnknownErrorFlagIsSet()
        {
            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.UnknownError;

            var result = await authService.LoginUserSendOtpAsync(loginUserSendOtpRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.UNKNOWN_ERROR));
        }

        [Test]
        public async Task LoginUserSendOtpAsync_HasNetworkErrors_WhenNetworkErrorFlagsAreSet()
        {
            var request = new LoginUserSendOtpRequest() { mobile = MOBILE, otp = OTP };
            ServiceResultWrapper<LoginUserSendOtpResult> result;

            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.NetworkTimeoutError;
            result = await authService.LoginUserSendOtpAsync(loginUserSendOtpRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.NETWORK_TIMEOUT_ERROR));

            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.NetworkUnknownError;
            result = await authService.LoginUserSendOtpAsync(loginUserSendOtpRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.NETWORK_UNKNOWN_ERROR));

            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.NetworkUnreachableError;
            result = await authService.LoginUserSendOtpAsync(loginUserSendOtpRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.NETWORK_UNREACHABLE_ERROR));
        }

        [Test]
        public async Task LoginUserSendOtpAsync_HasInvalidMobileNumberError_WhenInvalidMobileNumberErrorFlagIsSet()
        {
            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.InvalidDataError;

            var result = await authService.LoginUserSendOtpAsync(loginUserSendOtpRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.INVALID_DATA));
        }

        [Test]
        public async Task LoginUserSendOtpAsync_HasMobileNumberNotRegisteredError_WhenMobileNumberNotRegisteredErrorFlagIsSet()
        {
            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.MobileNumberNotRegisteredError;

            var result = await authService.LoginUserSendOtpAsync(loginUserSendOtpRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.MOBILE_NOT_REGISTERED));
        }

        [Test]
        public async Task LoginUserSendOtpAsync_HasAuthenticationError_WhenAuthenticationErrorFlagIsSet()
        {
            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.InvalidOtpError;

            var result = await authService.LoginUserSendOtpAsync(loginUserSendOtpRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.INVALID_OTP));
        }

        #endregion
            
        #region LinkUserRequestOtpAsync
        
        [Test]
        public async Task LinkUserRequestOtpAsync_HasNoError_WhenErrorFlagIsNone()
        {
            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.None;

            var result = await authService.LinkUserRequestOtpAsync(linkUserRequestOtpRequest);
            Assert.That(result.HasError, Is.False);
            Assert.That(result.Error, Is.Null);
        }

        [Test]
        public async Task LinkUserRequestOtpAsync_HasUnknownError_WhenUnknownErrorFlagIsSet()
        {
            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.UnknownError;

            var result = await authService.LinkUserRequestOtpAsync(linkUserRequestOtpRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.UNKNOWN_ERROR));
        }

        [Test]
        public async Task LinkUserRequestOtpAsync_HasNetworkErrors_WhenNetworkErrorFlagsAreSet()
        {
            var request = new LinkUserRequestOtpRequest() { username = KUMU_USERNAME };
            ServiceResultWrapper<LinkUserRequestOtpResult> result;

            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.NetworkTimeoutError;
            result = await authService.LinkUserRequestOtpAsync(linkUserRequestOtpRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.NETWORK_TIMEOUT_ERROR));

            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.NetworkUnknownError;
            result = await authService.LinkUserRequestOtpAsync(linkUserRequestOtpRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.NETWORK_UNKNOWN_ERROR));

            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.NetworkUnreachableError;
            result = await authService.LinkUserRequestOtpAsync(linkUserRequestOtpRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.NETWORK_UNREACHABLE_ERROR));
        }

        [Test]
        public async Task LinkUserRequestOtpAsync_HasInvalidMobileNumberError_WhenInvalidMobileNumberErrorFlagIsSet()
        {
            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.InvalidDataError;

            var result = await authService.LinkUserRequestOtpAsync(linkUserRequestOtpRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.INVALID_DATA));
        }

        [Test]
        public async Task LinkUserRequestOtpAsync_HasMobileNumberNotRegisteredError_WhenMobileNumberNotRegisteredErrorFlagIsSet()
        {
            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.MobileNumberNotRegisteredError;

            var result = await authService.LinkUserRequestOtpAsync(linkUserRequestOtpRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.MOBILE_NOT_REGISTERED));
        }

        [Test]
        public async Task LinkUserRequestOtpAsync_HasAuthenticationError_WhenAuthenticationErrorFlagIsSet()
        {
            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.InvalidOtpError;

            var result = await authService.LinkUserRequestOtpAsync(linkUserRequestOtpRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.INVALID_OTP));
        }
        
        #endregion
            
        #region LinkUserSendOtpAsync
        
        [Test]
        public async Task LinkUserSendOtpAsync_HasNoError_WhenErrorFlagIsNone()
        {
            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.None;

            var result = await authService.LinkUserSendOtpAsync(linkUserSendOtpRequest);
            Assert.That(result.HasError, Is.False);
            Assert.That(result.Error, Is.Null);
        }

        [Test]
        public async Task LinkUserSendOtpAsync_HasUnknownError_WhenUnknownErrorFlagIsSet()
        {
            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.UnknownError;

            var result = await authService.LinkUserSendOtpAsync(linkUserSendOtpRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.UNKNOWN_ERROR));
        }

        [Test]
        public async Task LinkUserSendOtpAsync_HasNetworkErrors_WhenNetworkErrorFlagsAreSet()
        {
            var request = new LinkUserSendOtpRequest() { username = KUMU_USERNAME, otp = OTP };
            ServiceResultWrapper<LinkUserSendOtpResult> result;

            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.NetworkTimeoutError;
            result = await authService.LinkUserSendOtpAsync(linkUserSendOtpRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.NETWORK_TIMEOUT_ERROR));

            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.NetworkUnknownError;
            result = await authService.LinkUserSendOtpAsync(linkUserSendOtpRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.NETWORK_UNKNOWN_ERROR));

            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.NetworkUnreachableError;
            result = await authService.LinkUserSendOtpAsync(linkUserSendOtpRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.NETWORK_UNREACHABLE_ERROR));
        }

        [Test]
        public async Task LinkUserSendOtpAsync_HasInvalidMobileNumberError_WhenInvalidMobileNumberErrorFlagIsSet()
        {
            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.InvalidDataError;

            var result = await authService.LinkUserSendOtpAsync(linkUserSendOtpRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.INVALID_DATA));
        }

        [Test]
        public async Task LinkUserSendOtpAsync_HasMobileNumberNotRegisteredError_WhenMobileNumberNotRegisteredErrorFlagIsSet()
        {
            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.MobileNumberNotRegisteredError;

            var result = await authService.LinkUserSendOtpAsync(linkUserSendOtpRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.MOBILE_NOT_REGISTERED));
        }

        [Test]
        public async Task LinkUserSendOtpAsync_HasAuthenticationError_WhenAuthenticationErrorFlagIsSet()
        {
            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.InvalidOtpError;

            var result = await authService.LinkUserSendOtpAsync(linkUserSendOtpRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.INVALID_OTP));
        }    
        
        #endregion 
            
        #region RefreshLinkRequestOtpAsync
        
        [Test]
        public async Task RefreshLinkRequestOtpAsync_HasNoError_WhenErrorFlagIsNone()
        {
            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.None;

            var result = await authService.RefreshLinkRequestOtpAsync(refreshLinkRequestOtpRequest);
            Assert.That(result.HasError, Is.False);
            Assert.That(result.Error, Is.Null);
        }

        [Test]
        public async Task RefreshLinkRequestOtpAsync_HasUnknownError_WhenUnknownErrorFlagIsSet()
        {
            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.UnknownError;

            var result = await authService.RefreshLinkRequestOtpAsync(refreshLinkRequestOtpRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.UNKNOWN_ERROR));
        }

        [Test]
        public async Task RefreshLinkRequestOtpAsync_HasNetworkErrors_WhenNetworkErrorFlagsAreSet()
        {
            var request = new RefreshLinkRequestOtpRequest();
            ServiceResultWrapper<RefreshLinkRequestOtpResult> result;

            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.NetworkTimeoutError;
            result = await authService.RefreshLinkRequestOtpAsync(refreshLinkRequestOtpRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.NETWORK_TIMEOUT_ERROR));

            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.NetworkUnknownError;
            result = await authService.RefreshLinkRequestOtpAsync(refreshLinkRequestOtpRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.NETWORK_UNKNOWN_ERROR));

            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.NetworkUnreachableError;
            result = await authService.RefreshLinkRequestOtpAsync(refreshLinkRequestOtpRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.NETWORK_UNREACHABLE_ERROR));
        }

        [Test]
        public async Task RefreshLinkRequestOtpAsync_HasInvalidMobileNumberError_WhenInvalidMobileNumberErrorFlagIsSet()
        {
            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.InvalidDataError;

            var result = await authService.RefreshLinkRequestOtpAsync(refreshLinkRequestOtpRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.INVALID_DATA));
        }

        [Test]
        public async Task RefreshLinkRequestOtpAsync_HasMobileNumberNotRegisteredError_WhenMobileNumberNotRegisteredErrorFlagIsSet()
        {
            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.MobileNumberNotRegisteredError;

            var result = await authService.RefreshLinkRequestOtpAsync(refreshLinkRequestOtpRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.MOBILE_NOT_REGISTERED));
        }

        [Test]
        public async Task RefreshLinkRequestOtpAsync_HasAuthenticationError_WhenAuthenticationErrorFlagIsSet()
        {
            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.InvalidOtpError;

            var result = await authService.RefreshLinkRequestOtpAsync(refreshLinkRequestOtpRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.INVALID_OTP));
        }
        
        #endregion
            
        #region RefreshLinkSendOtpAsync
        
        [Test]
        public async Task RefreshLinkSendOtpAsync_HasNoError_WhenErrorFlagIsNone()
        {
            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.None;

            var result = await authService.RefreshLinkSendOtpAsync(refreshLinkSendOtpRequest);
            Assert.That(result.HasError, Is.False);
            Assert.That(result.Error, Is.Null);
        }

        [Test]
        public async Task RefreshLinkSendOtpAsync_HasUnknownError_WhenUnknownErrorFlagIsSet()
        {
            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.UnknownError;

            var result = await authService.RefreshLinkSendOtpAsync(refreshLinkSendOtpRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.UNKNOWN_ERROR));
        }

        [Test]
        public async Task RefreshLinkSendOtpAsync_HasNetworkErrors_WhenNetworkErrorFlagsAreSet()
        {
            var request = new RefreshLinkSendOtpRequest() { otp = OTP };
            ServiceResultWrapper<RefreshLinkSendOtpResult> result;

            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.NetworkTimeoutError;
            result = await authService.RefreshLinkSendOtpAsync(refreshLinkSendOtpRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.NETWORK_TIMEOUT_ERROR));

            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.NetworkUnknownError;
            result = await authService.RefreshLinkSendOtpAsync(refreshLinkSendOtpRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.NETWORK_UNKNOWN_ERROR));

            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.NetworkUnreachableError;
            result = await authService.RefreshLinkSendOtpAsync(refreshLinkSendOtpRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.NETWORK_UNREACHABLE_ERROR));
        }

        [Test]
        public async Task RefreshLinkSendOtpAsync_HasInvalidMobileNumberError_WhenInvalidMobileNumberErrorFlagIsSet()
        {
            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.InvalidDataError;

            var result = await authService.RefreshLinkSendOtpAsync(refreshLinkSendOtpRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.INVALID_DATA));
        }

        [Test]
        public async Task RefreshLinkSendOtpAsync_HasMobileNumberNotRegisteredError_WhenMobileNumberNotRegisteredErrorFlagIsSet()
        {
            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.MobileNumberNotRegisteredError;

            var result = await authService.RefreshLinkSendOtpAsync(refreshLinkSendOtpRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.MOBILE_NOT_REGISTERED));
        }

        [Test]
        public async Task RefreshLinkSendOtpAsync_HasAuthenticationError_WhenAuthenticationErrorFlagIsSet()
        {
            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.InvalidOtpError;

            var result = await authService.RefreshLinkSendOtpAsync(refreshLinkSendOtpRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.INVALID_OTP));
        }    
        
        #endregion 
            
        #region UnlinkUserAsync
        
        [Test]
        public async Task UnlinkUserAsync_HasNoErrors_WhenErrorFlagIsNone()
        {
            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.None;

            var result = await authService.UnlinkUserAsync(unlinkUserRequest);
            Assert.That(result.HasError, Is.False);
            Assert.That(result.Error, Is.Null);
        }

        [Test]
        public async Task UnlinkUserAsync_HasUnknownError_WhenUnknownErrorFlagIsSet()
        {
            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.UnknownError;

            var result = await authService.UnlinkUserAsync(unlinkUserRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.UNKNOWN_ERROR));
        }

        [Test]
        public async Task UnlinkUserAsync_HasNetworkErrors_WhenNetworkErrorFlagsAreSet()
        {
            ServiceResultWrapper<UnlinkUserResult> result;

            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.NetworkTimeoutError;
            result = await authService.UnlinkUserAsync(unlinkUserRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.NETWORK_TIMEOUT_ERROR));

            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.NetworkUnknownError;
            result = await authService.UnlinkUserAsync(unlinkUserRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.NETWORK_UNKNOWN_ERROR));

            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.NetworkUnreachableError;
            result = await authService.UnlinkUserAsync(unlinkUserRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.NETWORK_UNREACHABLE_ERROR));
        }    
        
        #endregion

        #region LogoutUserAsync

        [Test]
        public async Task LogoutUserAsync_HasNoErrors_WhenErrorFlagIsNone()
        {
            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.None;

            var result = await authService.LogOutUserAsync(logoutUserRequest);
            Assert.That(result.HasError, Is.False);
            Assert.That(result.Error, Is.Null);
        }

        [Test]
        public async Task LogoutUserAsync_HasUnknownError_WhenUnknownErrorFlagIsSet()
        {
            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.UnknownError;

            var result = await authService.LogOutUserAsync(logoutUserRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.UNKNOWN_ERROR));
        }

        [Test]
        public async Task LogoutUserAsync_HasNetworkErrors_WhenNetworkErrorFlagsAreSet()
        {
            ServiceResultWrapper<LogoutUserResult> result;

            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.NetworkTimeoutError;
            result = await authService.LogOutUserAsync(logoutUserRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.NETWORK_TIMEOUT_ERROR));

            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.NetworkUnknownError;
            result = await authService.LogOutUserAsync(logoutUserRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.NETWORK_UNKNOWN_ERROR));

            authService.ErrorFlags = MockedAuthService.ResultErrorFlags.NetworkUnreachableError;
            result = await authService.LogOutUserAsync(logoutUserRequest);
            Assert.That(result.HasError, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(ServiceErrorCodes.NETWORK_UNREACHABLE_ERROR));
        }

        #endregion
    }
}
