using System;
using System.Threading.Tasks;

namespace Kumu.Kulitan.Backend
{
    public class MockedLobbyService : ILobbyService
    {
        [Flags]
        public enum ResultErrorFlags
        {
            None = 0,
            UnknownError = 0xb1 << 1,
            NetworkUnknownError = 0xb1 << 2,
            NetworkTimeoutError = 0xb1 << 3,
            NetworkUnreachableError = 0xb1 << 4,
        }

        public int ResponseTimeInMilliseconds { get; set; }

        public ResultErrorFlags ErrorFlags { get; set; }

        public async Task<ServiceResultWrapper<GetLobbyConfigResult>> GetLobbyConfigsAsync(GetLobbyConfigsRequest request)
        {
            await Task.Delay(ResponseTimeInMilliseconds);

            if (ErrorFlags.HasFlag(ResultErrorFlags.UnknownError))
            {
                return new ServiceResultWrapper<GetLobbyConfigResult>(ServiceErrors.unknownError);
            }

            if (TryGetNetworkError(out var error))
            {
                return new ServiceResultWrapper<GetLobbyConfigResult>(error);
            }

            var result = new GetLobbyConfigResult
            {
                lobbyConfigs = new[]
                {
                    new LobbyConfig { id = "haloHalo", name = "Halo-Halo" },
                    new LobbyConfig { id = "mangoSago", name = "Mango Sago" },
                    new LobbyConfig { id = "bukoPandan", name = "Buko Pandan" },
                    new LobbyConfig { id = "lecheFlan", name = "Leche Flan" },
                    new LobbyConfig { id = "biko", name = "Biko" },
                    new LobbyConfig { id = "lengua", name = "Lengua De Gato" },
                    new LobbyConfig { id = "polvoron", name = "Polvoron" },
                    new LobbyConfig { id = "suman", name = "Suman" }
                }
            };

            return new ServiceResultWrapper<GetLobbyConfigResult>(result);
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
