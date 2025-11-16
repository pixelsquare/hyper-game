using System;
using System.Threading.Tasks;

namespace Kumu.Kulitan.Backend
{
    public class MockedUserProfileService : IUserProfileService
    {
        [Flags]
        public enum ResultErrorFlags
        {
            None = 0,
            UnknownError = 0xb1 << 1,
            NetworkUnknownError = 0xb1 << 2,
            NetworkTimeoutError = 0xb1 << 3,
            NetworkUnreachableError = 0xb1 << 4,
            UserNameExhaustedError = 0xb1 << 5,
            ProfileAlreadyExistsError = 0xb1 << 6,
            InvalidDataError = 0xb1 << 7, // Data provided is invalid
            AppInMaintenanceError = 0x01 << 8,
            MismatchedVersionError = 0x01 << 9,
        }

        public int ResponseTimeInMilliseconds { get; set; }

        public bool IsProfileReady { get; set; } = true;

        public bool HasLinkedKumuAccount { get; set; }

        public ResultErrorFlags ErrorFlags { get; set; }

        public async Task<ServiceResultWrapper<GetUserProfileResult>> GetUserProfileAsync(GetUserProfileRequest request)
        {
            await Task.Delay(ResponseTimeInMilliseconds);

            var profile = MockedServicesUtil.GetMockedProfileInPrefs();

            if (!IsProfileReady)
            {
                profile.userName = new UserName
                {
                    discriminator = "0000",
                    name = "null"
                }.ToString();
                profile.nickName = null;
                profile.ageRange = -1;
                profile.gender = -1;
            }

            profile.hasLinkedKumuAccount = HasLinkedKumuAccount;
            profile.playerId = (uint)Guid.NewGuid().GetHashCode();

            if (ErrorFlags.HasFlag(ResultErrorFlags.UnknownError))
            {
                return new ServiceResultWrapper<GetUserProfileResult>(ServiceErrors.unknownError);
            }

            if (TryGetNetworkError(out var networkError))
            {
                return new ServiceResultWrapper<GetUserProfileResult>(networkError);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.AppInMaintenanceError))
            {
                return new ServiceResultWrapper<GetUserProfileResult>(MockedErrors.appInMaintenanceError);
            }
            
            if (ErrorFlags.HasFlag(ResultErrorFlags.MismatchedVersionError))
            {
                return new ServiceResultWrapper<GetUserProfileResult>(ServiceErrors.mismatchedVersionError);
            }

            return new ServiceResultWrapper<GetUserProfileResult>(new GetUserProfileResult(profile));
        }

        public async Task<ServiceResultWrapper<CreateUserProfileResult>> CreateUserProfileAsync(CreateUserProfileRequest request)
        {
            await Task.Delay(ResponseTimeInMilliseconds);

            if (ErrorFlags.HasFlag(ResultErrorFlags.UnknownError))
            {
                return new ServiceResultWrapper<CreateUserProfileResult>(ServiceErrors.unknownError);
            }

            if (TryGetNetworkError(out var networkError))
            {
                return new ServiceResultWrapper<CreateUserProfileResult>(networkError);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.UserNameExhaustedError))
            {
                return new ServiceResultWrapper<CreateUserProfileResult>(MockedErrors.usernameExhaustedError);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.ProfileAlreadyExistsError))
            {
                return new ServiceResultWrapper<CreateUserProfileResult>(MockedErrors.profileAlreadyExistsError);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.InvalidDataError))
            {
                return new ServiceResultWrapper<CreateUserProfileResult>(MockedErrors.invalidDataError);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.AppInMaintenanceError))
            {
                return new ServiceResultWrapper<CreateUserProfileResult>(MockedErrors.appInMaintenanceError);
            }
            
            if (ErrorFlags.HasFlag(ResultErrorFlags.MismatchedVersionError))
            {
                return new ServiceResultWrapper<CreateUserProfileResult>(ServiceErrors.mismatchedVersionError);
            }

            MockedServicesUtil.SetMockedProfileInPrefs(request.username, request.nickname, request.ageRange, request.gender);

            return new ServiceResultWrapper<CreateUserProfileResult>(new CreateUserProfileResult());
        }

        private bool TryGetNetworkError(out ServiceError error)
        {
            error = null;

            if (ErrorFlags.HasFlag(ResultErrorFlags.NetworkUnreachableError))
            {
                error = ServiceErrors.networkUnreachableError;
                return true;
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.NetworkTimeoutError))
            {
                error = ServiceErrors.networkTimeoutError;
                return true;
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.NetworkUnknownError))
            {
                error = ServiceErrors.networkUnknownError;
                return true;
            }

            return false;
        }
    }
}
