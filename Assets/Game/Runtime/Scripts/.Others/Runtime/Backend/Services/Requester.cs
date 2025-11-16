using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Kumu.Extensions;
using UnityEngine;
using UnityEngine.Networking;

namespace Kumu.Kulitan.Backend
{
    public abstract class Requester<TRequest, TResult> where TRequest : RequestCommon where TResult : ResultBase
    {
        public const int DefaultTimeoutBaseValue = 5;
        public const int DefaultMaxRetries = 3;
        
        public int MaxRetries { get; set; } = DefaultMaxRetries;
        public int TimeoutBaseValue { get; set; } = DefaultTimeoutBaseValue;
        public TRequest RequestObject { get; }
        public virtual Task TokenGetter => Services.AuthService.GetUserTokenAsync();
        
        public abstract UnityWebRequest FormUnityWebRequest();
        public abstract TResult FormResultFromPayload(HttpResponseObject payload);

        public Requester(TRequest requestObject)
        {
            RequestObject = requestObject;
        }
        
        public virtual bool UseCachedResult(out TResult result)
        {
            result = default;
            return false;
        }

        public async Task<ServiceResultWrapper<TResult>> ExecuteRequestAsync(CancellationToken cToken = default)
        {
            try
            {
                var retry = 0;

                while (true)
                {
                    // Use cached result if applicable
                    if (UseCachedResult(out var cachedResult))
                    {
                        return new ServiceResultWrapper<TResult>(cachedResult);
                    }
                    
                    // Prepare token
                    await TokenGetter;

                    // Form request
                    using var request = FormUnityWebRequest().WithBackoff(TimeoutBaseValue, retry);
                    $"[{GetType().Name}] Sending WebRequest - Retry:{retry}/{MaxRetries}, Timeout:{request.timeout}".Log();

                    // Send request
                    var op = request.SendWebRequest();
                    while (!op.isDone)
                    {
                        if (cToken.IsCancellationRequested)
                        {
                            $"[{GetType().Name}] Request cancelled! {request.url}".LogError();
                            request.Abort();
                            return new ServiceResultWrapper<TResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, "Task canceled."));
                        }

                        await Task.Yield();
                    }

                    // Return result if successful
                    if (BackendUtil.TryGetPayload<TResult>(request, out var payload, out var errorWrapper))
                    {
                        $"[{GetType().Name}] WebRequest received response!".Log();
                        return new ServiceResultWrapper<TResult>(FormResultFromPayload(payload));
                    }

                    // Error handling
                    retry++;
                    switch (errorWrapper.Error.Code)
                    {
                        case ServiceErrorCodes.NETWORK_TIMEOUT_ERROR when retry > MaxRetries:
                            $"[{GetType().Name}] Timeouts exceeded.".LogError();
                            return new ServiceResultWrapper<TResult>(ServiceErrors.networkTimeoutError);
                        case ServiceErrorCodes.NETWORK_TIMEOUT_ERROR:
                            $"[{GetType().Name}] Timeout - retrying.".LogError();
                            continue;
                        // Token failed for some reason
                        case ServiceErrorCodes.TOKEN_EXPIRED:
                            $"[{GetType().Name}] Unexpected invalid token.".LogError();
                            return new ServiceResultWrapper<TResult>(new ServiceError(ServiceErrorCodes.TOKEN_EXPIRED, "Invalid token."));
                        default:
                            // Some other error occured
                            $"[{GetType().Name}] Unexpected error occured. Error:{errorWrapper.Error}, Message: {errorWrapper.Error.Message}".LogError();
                            return errorWrapper;
                    }
                }
            }
            catch(Exception e)
            {
                return new ServiceResultWrapper<TResult>(new ServiceError(ServiceErrorCodes.UNKNOWN_ERROR, e.Message));
            }
        }
    }

    internal static class ExtensionMethods
    {
        public static UnityWebRequest WithBackoff(this UnityWebRequest request, int baseTimeout, int retry)
        {
            retry = Mathf.Max(0, retry);
            var timeout = baseTimeout * (int)Mathf.Pow(2,  retry);
            request.timeout = timeout >= 0 ? timeout : request.timeout;
            
            return request;
        }
    }
}
