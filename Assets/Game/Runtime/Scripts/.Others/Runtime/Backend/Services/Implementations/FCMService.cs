using System.Threading.Tasks;
using Firebase.Messaging;
using Kumu.Extensions;

namespace Kumu.Kulitan.Backend
{
    public class FCMService : IFCMService
    {
        public static string FcmToken;
        
        public bool IsInitialized { get; private set; }

        public void Init()
        {
            InitAsyncInternal();
        }

        private async Task InitAsyncInternal()
        {
            if (IsInitialized)
            {
                return;
            }
            
            var task = FirebaseMessaging.GetTokenAsync();

            await task;

            if (task.IsCompleted || task.IsCompletedSuccessfully)
            {
                FcmToken = task.Result;
                $"FCM token successfully retrieved - {FcmToken}".Log();
            }
            else
            {
                $"Error retrieving token - {task.Exception}.".Log();
            }

            IsInitialized = true;
        }

        public async Task<ServiceResultWrapper<RegisterFCMTokenResult>> RegisterFCMTokenAsync(RegisterFCMTokenRequest request)
        {
#if !USE_OLD_API_METHODS
            var requester = new RegisterFCMTokenRequester(request);
            return await requester.ExecuteRequestAsync();
#else
            $"FCM token: {BackendUtil.GlobalHeaders["x-ube-fcm-token"]}".Log();

            var fullUrl = BackendUtil.GetFullUrl("/api/v3/player/auth/fcm");
            using var req = BackendUtil.Put(fullUrl);

            try
            {
                using (var tokenSource = CTokenSource.Create())
                {
                    if (!await BackendUtil.TrySendWebRequest(req, tokenSource.Token))
                    {
                        return new ServiceResultWrapper<RegisterFCMTokenResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Failed to send request or cancelled."));
                    }
                }

                if (!BackendUtil.TryGetPayload<RegisterFCMTokenResult>(req, out var payload, out var errorWrapper))
                {
                    return errorWrapper;
                }

                var result = new RegisterFCMTokenResult();
                return new ServiceResultWrapper<RegisterFCMTokenResult>(result);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return new ServiceResultWrapper<RegisterFCMTokenResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, e.Message));
            }
#endif
        }
    }
}
