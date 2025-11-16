using System;
using System.Threading.Tasks;

namespace Kumu.Kulitan.Backend
{
    public class MockedModerationService : IModerationService
    {
        [Flags]
        public enum ResultErrorFlags
        {
            None = 0,
            UnknownError = 0xb1 << 1,
            NetworkUnknownError = 0xb1 << 2,
            NetworkTimeoutError = 0xb1 << 3,
            NetworkUnreachableError = 0xb1 << 4,
            InvalidDataError = 0xb1 << 5, // Data provided is invalid
            AppInMaintenanceError = 0x01 << 6
        }

        public int ResponseTimeInMilliseconds { get; set; }

        public ResultErrorFlags ErrorFlags { get; set; }

        public async Task<ServiceResultWrapper<ReportUserResult>> ReportUserAsync(ReportUserRequest request)
        {
            await Task.Delay(ResponseTimeInMilliseconds);

            if (ErrorFlags.HasFlag(ResultErrorFlags.UnknownError))
            {
                return new ServiceResultWrapper<ReportUserResult>(ServiceErrors.unknownError);
            }

            if (TryGetNetworkError(out var error))
            {
                return new ServiceResultWrapper<ReportUserResult>(error);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.InvalidDataError))
            {
                return new ServiceResultWrapper<ReportUserResult>(MockedErrors.invalidDataError);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.AppInMaintenanceError))
            {
                return new ServiceResultWrapper<ReportUserResult>(MockedErrors.appInMaintenanceError);
            }

            var result = new ReportUserResult();
            return new ServiceResultWrapper<ReportUserResult>(result);
        }

        public async Task<ServiceResultWrapper<ReportHangoutResult>> ReportHangoutAsync(ReportHangoutRequest request)
        {
            await Task.Delay(ResponseTimeInMilliseconds);

            if (ErrorFlags.HasFlag(ResultErrorFlags.UnknownError))
            {
                return new ServiceResultWrapper<ReportHangoutResult>(ServiceErrors.unknownError);
            }

            if (TryGetNetworkError(out var error))
            {
                return new ServiceResultWrapper<ReportHangoutResult>(error);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.InvalidDataError))
            {
                return new ServiceResultWrapper<ReportHangoutResult>(MockedErrors.invalidDataError);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.AppInMaintenanceError))
            {
                return new ServiceResultWrapper<ReportHangoutResult>(MockedErrors.appInMaintenanceError);
            }

            var result = new ReportHangoutResult();
            return new ServiceResultWrapper<ReportHangoutResult>(result);
        }

        public async Task<ServiceResultWrapper<BlockPlayerResult>> BlockPlayerAsync(BlockPlayerRequest request)
        {
            await Task.Delay(ResponseTimeInMilliseconds);

            if (ErrorFlags.HasFlag(ResultErrorFlags.UnknownError))
            {
                return new ServiceResultWrapper<BlockPlayerResult>(ServiceErrors.unknownError);
            }

            if (TryGetNetworkError(out var error))
            {
                return new ServiceResultWrapper<BlockPlayerResult>(error);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.InvalidDataError))
            {
                return new ServiceResultWrapper<BlockPlayerResult>(MockedErrors.invalidDataError);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.AppInMaintenanceError))
            {
                return new ServiceResultWrapper<BlockPlayerResult>(MockedErrors.appInMaintenanceError);
            }

            MockedServicesUtil.AddUserToBlocklist(request.userId);

            var result = new BlockPlayerResult();
            return new ServiceResultWrapper<BlockPlayerResult>(result);
        }

        public async Task<ServiceResultWrapper<UnblockPlayerResult>> UnblockPlayerAsync(UnblockPlayerRequest request)
        {
            await Task.Delay(ResponseTimeInMilliseconds);

            if (ErrorFlags.HasFlag(ResultErrorFlags.UnknownError))
            {
                return new ServiceResultWrapper<UnblockPlayerResult>(ServiceErrors.unknownError);
            }

            if (TryGetNetworkError(out var error))
            {
                return new ServiceResultWrapper<UnblockPlayerResult>(error);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.InvalidDataError))
            {
                return new ServiceResultWrapper<UnblockPlayerResult>(MockedErrors.invalidDataError);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.AppInMaintenanceError))
            {
                return new ServiceResultWrapper<UnblockPlayerResult>(MockedErrors.appInMaintenanceError);
            }

            MockedServicesUtil.RemoveUserToBlocklist(request.userId);

            var result = new UnblockPlayerResult();
            return new ServiceResultWrapper<UnblockPlayerResult>(result);
        }

        public async Task<ServiceResultWrapper<GetBlockedPlayersResult>> GetBlockedPlayersAsync(GetBlockedPlayersRequest request)
        {
            await Task.Delay(ResponseTimeInMilliseconds);

            if (ErrorFlags.HasFlag(ResultErrorFlags.UnknownError))
            {
                return new ServiceResultWrapper<GetBlockedPlayersResult>(ServiceErrors.unknownError);
            }

            if (TryGetNetworkError(out var error))
            {
                return new ServiceResultWrapper<GetBlockedPlayersResult>(error);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.InvalidDataError))
            {
                return new ServiceResultWrapper<GetBlockedPlayersResult>(MockedErrors.invalidDataError);
            }

            if (ErrorFlags.HasFlag(ResultErrorFlags.AppInMaintenanceError))
            {
                return new ServiceResultWrapper<GetBlockedPlayersResult>(MockedErrors.appInMaintenanceError);
            }

            var result = new GetBlockedPlayersResult { blockedUserIds = MockedServicesUtil.GetBlockedlist() };
            return new ServiceResultWrapper<GetBlockedPlayersResult>(result);
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
