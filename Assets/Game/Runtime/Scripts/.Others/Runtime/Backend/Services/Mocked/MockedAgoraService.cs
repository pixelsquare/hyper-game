using System;
using System.Threading.Tasks;

namespace Kumu.Kulitan.Backend
{
    class MockedAgoraService : IAgoraService
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
        }

        public int ResponseTimeInMilliseconds { get; set; }

        public bool IsProfileReady { get; set; } = true;

        public bool HasLinkedKumuAccount { get; set; }

        public ResultErrorFlags ErrorFlags { get; set; }
        
        public async Task<ServiceResultWrapper<GetRTCTokenResult>> GetRTCTokenAsync(GetRTCTokenRequest request)
        {
            await Task.Delay(ResponseTimeInMilliseconds);

            if (ErrorFlags.HasFlag(ResultErrorFlags.UnknownError))
            {
                return new ServiceResultWrapper<GetRTCTokenResult>(ServiceErrors.unknownError);
            }

            if (TryGetNetworkError(out var error))
            {
                return new ServiceResultWrapper<GetRTCTokenResult>(error);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.AppInMaintenanceError))
            {
                return new ServiceResultWrapper<GetRTCTokenResult>(MockedErrors.appInMaintenanceError);
            }

            var result = new GetRTCTokenResult();
            result.token = null;

            return new ServiceResultWrapper<GetRTCTokenResult>(result);
        }

        public async Task<ServiceResultWrapper<GetRTMTokenResult>> GetRTMTokenAsync(GetRTMTokenRequest request)
        {
            await Task.Delay(ResponseTimeInMilliseconds);

            if (ErrorFlags.HasFlag(ResultErrorFlags.UnknownError))
            {
                return new ServiceResultWrapper<GetRTMTokenResult>(ServiceErrors.unknownError);
            }

            if (TryGetNetworkError(out var error))
            {
                return new ServiceResultWrapper<GetRTMTokenResult>(error);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.AppInMaintenanceError))
            {
                return new ServiceResultWrapper<GetRTMTokenResult>(MockedErrors.appInMaintenanceError);
            }

            var result = new GetRTMTokenResult();
            result.token = null;

            return new ServiceResultWrapper<GetRTMTokenResult>(result);
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
