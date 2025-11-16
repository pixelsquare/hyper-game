using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Kumu.Extensions;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Hangout;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Kumu.Kulitan.Backend
{
    public class KumuCertificate : CertificateHandler
    {
#if UBE_RELEASE
        private const string PublicKey = "3082010A0282010100EA52CBC8A025CA5D7C1F10A6AC50E929C383" +
                "3626218CA8BE8B1169376D40153CB4B8CCAE6200DBB4F65E146969FC4C52BDD188AE72DFCE1E2FC" +
                "35BB355E95BB978FD49312AF4189D346EC4602098B7B5A62DC941189C5E02432D8050BB04FFADD7" +
                "A12A065432206883F772228493BCEC3DF519826D025E3E5B2A6BCEC0C330D1220CAB89BF6A6E387" +
                "6F91744F4E218864365372D0BFA16A4BCB077375DE6D7A454845070AECFA0C130B566A47486C869" +
                "BA3B6D4CF1805BF6A6CC45BF26E74387876205AF1777F0615C9AAFAF7E756F8E3DCB14265EB3212" +
                "13FBA14DBE27DBD8B009768DE689D3AE46848F170055852DBA5BC80771AB61D73DAA06ED7B2CB00" +
                "0D0203010001";
        
#else
        private const string PublicKey = "3082010A0282010100A3F4D0F70501D1A2970367E4D4ACD49111BA" +
                "6653D5587A0BD542BA4FD17F04650555F242BB55CCDC486921A5350EF071826DB1430495CA8111F" +
                "5E6AFE0C13CD00F0846B9549C53502B6DEBF6238A0CD2338E52AA17579F591A8ED76D722E3A740A" +
                "F78C9A236F26C3F0A5A53E2B5B33205FB1D3B4BDA90EEEF1517BE6BF39871F7C35663BABA0D3193" +
                "DA87498B5DE7095935D87F9259A579632E8EFAB4A27D272092E477918B5B8AB18C59F8E15F20372" +
                "DF5529128D08DF86F45DAC21189C74895B9FF47518544DA0DE4DA236605DF45F490D43DF53FCCCD" +
                "487D37E5F9A8BFCC328630AEACC4F9F8F47B8E478C2A34357553298287E6065115868A1E99F7D17" +
                "350203010001";
#endif

        protected override bool ValidateCertificate(byte[] certificateData)
        {
            var cert = new X509Certificate2(certificateData);
            var pk = cert.GetPublicKeyString();
            return string.Compare(PublicKey, pk, StringComparison.OrdinalIgnoreCase) == 0;
        }
    }

    public static class BackendUtil
    {
        public const int TimeoutSec = 15;

        public static readonly string TokenCacheKey = "aa";
        private static readonly ITokenCache<UserTokens> TokenCache = new EncryptedTokenCachePrefs(TokenCacheKey);

        private static VersionObject versionObject;
        private static readonly UserTokens userTokens = TokenCache.LoadToken();

        public static readonly Dictionary<string, string> GlobalHeaders = new()
        {
            { "X-Ube-Api-Key", $"{ApiKey}" },
            { "X-Ube-Device-ID", $"{DeviceUniqueIdentifier}" },
            { "X-Ube-App-Version", $"{VersionObject}" },
            { "X-Kumu-Access-Token", $"{SsoToken}" },
            { "Authorization", $"Bearer {AuthToken}" },
            { "Content-Type", "application/json" },
            { "x-ube-fcm-token", "" },
            { "x-ube-app-platform", PlatformLabel }
        };

        public static readonly Dictionary<string, string> GlobalMapping = new()
        {
            { "mobile", "mobile_number" },
            { "accountId", "player_id" },
            { "playerId", "player_id_crc32" },
            { "nickName", "nickname" },
            { "ageRange", "age_range" },
            { "userName", "username" },
            { "hasLinkedKumuAccount", "has_linked_kumu_account" },
            { "itemId", "id" },
            { "hasColor", "has_color" },
            { "colorHex", "color" },
            { "equippedItems", "equipped_items" },
            { "equippedAndItemsToBuy", "items_to_equip_and_buy" },
            { "expectedCost", "expected_cost" },
            { "markUpDownCost", "mark_up_down_cost" },
            { "subcategory", "sub_category" },
            { "info", "information" },
            { "shouldBlock", "block_player" },
            { "photonRoomId", "photon_room_id" },
            { "photonRoomName", "photon_room_name" },
            { "roomId", "room_id" },
            { "friendsOnly", "friends_only" },
            { "followingCount", "following_count" },
            { "followerCount", "follower_count" },
            { "friendCount", "friend_count" }
        };

        public static string AuthToken
        {
            get => userTokens.authToken;
            private set
            {
                userTokens.authToken = value;
                GlobalHeaders["Authorization"] = $"Bearer {userTokens.authToken}";
                TokenCache.SaveToken(userTokens);
            }
        }

        public static string SsoToken
        {
            get => userTokens.ssoToken;
            private set
            {
                userTokens.ssoToken = value;
                GlobalHeaders["X-Kumu-Access-Token"] = $"{userTokens.ssoToken}";
                TokenCache.SaveToken(userTokens);
            }
        }

        public static string SsoRefreshToken
        {
            get => userTokens.ssoRefreshToken;
            private set
            {
                userTokens.ssoRefreshToken = value;
                TokenCache.SaveToken(userTokens);
            }
        }

        public static string PlatformLabel
        {
            get
            {
#if UNITY_ANDROID // can also be true while in editor
                return "android";
#elif UNITY_IOS // can also be true while in editor
                return "ios";
#else
                return "other";
#endif
            }
        }

        public static string DefaultApiUrl
        {
#if UBE_RELEASE
            get => "api.kumuplay.dev";
#elif UBE_STAGING || UBE_DEV
            get => "dev-api.kumuplay.dev";
#else
            get => "dev-api.kumuplay.dev";
#endif
        }

        private static string ApiKey
        {
#if UBE_RELEASE
            get => "$2y$10$7uMtwlyEuvS2Zk5V2w6.IebpI6pE/St46cZ8uF6Si1erWjI3fpTo2";
#elif UBE_STAGING || UBE_DEV
            get => "$2y$10$HQ.gkFZLIGGCkEOhgWnXhe/5gvsOhNytq8rdh8KFsZsDCHQfEilVK";
#else
            get => "$2y$10$HQ.gkFZLIGGCkEOhgWnXhe/5gvsOhNytq8rdh8KFsZsDCHQfEilVK";
#endif
        }

        private static string UrlProtocol => "https://";

        public static bool IsAuthTokenValid => userTokens != null && !string.IsNullOrEmpty(userTokens.authToken);
        public static bool IsSsoTokenValid => userTokens != null && !string.IsNullOrEmpty(userTokens.ssoToken);
        public static bool IsSsoRefreshTokenValid => userTokens != null && !string.IsNullOrEmpty(userTokens.ssoRefreshToken);

        public static VersionObject VersionObject
        {
            get
            {
                if (versionObject == null)
                {
                    versionObject = Resources.Load<VersionObject>(VersionObject.RESOURCE_NAME);
                }

                return versionObject;
            }
        }

        public static string DeviceUniqueIdentifier
        {
            get
            {
                var deviceId = "";

#if !UNITY_EDITOR && UNITY_ANDROID
                var up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                var curActivity = up.GetStatic<AndroidJavaObject>("currentActivity");
                var contentResolver = curActivity.Call<AndroidJavaObject>("getContentResolver");
                var secure = new AndroidJavaClass("android.provider.Settings$Secure");
                deviceId = secure.CallStatic<string>("getString", contentResolver, "android_id");
#else
                deviceId = SystemInfo.deviceUniqueIdentifier;
#endif
                return deviceId;
            }
        }

        public static string GetFullUrl(string apiCall, List<(string Key, string Value)> parameters = null)
        {
            var sb = new StringBuilder(1000);
            var baseUrl = DefaultApiUrl;

            if (!baseUrl.StartsWith(UrlProtocol))
            {
                sb.Append(UrlProtocol);
            }

            sb.Append(baseUrl).Append(apiCall);

            if (parameters == null)
            {
                return sb.ToString();
            }

            var isFirstParam = true;

            foreach (var param in parameters)
            {
                if (isFirstParam)
                {
                    sb.Append("?");
                    isFirstParam = false;
                }
                else
                {
                    sb.Append("&");
                }

                sb.Append(param.Key).Append("=").Append(param.Value);
            }

            return sb.ToString();
        }

        public static void SetAuthToken(string authToken)
        {
            AuthToken = authToken;
        }

        public static bool TryWriteToken(string payloadData)
        {
            var dataMap = JsonConvert.DeserializeObject<IDictionary<string, IDictionary<string, object>>>(payloadData);

            if (dataMap != null && dataMap.TryGetValue("authorization", out var authMap)
             && authMap != null && authMap.TryGetValue("access_token", out var authTokenObj))
            {
                AuthToken = authTokenObj.ToString();
                return true;
            }

            return false;
        }

        public static bool TryWriteSsoToken(string payloadData)
        {
            var dataMap = JsonConvert.DeserializeObject<IDictionary<string, IDictionary<string, object>>>(payloadData);

            if (dataMap == null || !dataMap.TryGetValue("kumu_tokens", out var authMap))
            {
                return false;
            }

            if (authMap == null || !authMap.TryGetValue("access_token", out var authTokenObj)
             || !authMap.TryGetValue("refresh_token", out var refreshTokenObj))
            {
                return false;
            }

            SsoToken = authTokenObj.ToString();
            SsoRefreshToken = refreshTokenObj.ToString();
            return true;
        }

        public static void ClearSsoTokens()
        {
            SsoToken = string.Empty;
            SsoRefreshToken = string.Empty;
        }

        public static void ClearAccessTokens()
        {
            AuthToken = string.Empty;
        }

        public static void ClearAllTokens()
        {
            TokenCache.Delete();
        }

        public static UnityWebRequest Post(string url, string postData = null)
        {
            var downloadHandler = new DownloadHandlerBuffer();
            var uploadHandler = !string.IsNullOrEmpty(postData) ? new UploadHandlerRaw(Encoding.UTF8.GetBytes(postData)) : null;
            var request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST, downloadHandler, uploadHandler);
            SetupRequest(request);
            return request;
        }

        public static UnityWebRequest Get(string url)
        {
            var downloadHandler = new DownloadHandlerBuffer();
            var request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET, downloadHandler, null);
            SetupRequest(request);
            return request;
        }

        public static UnityWebRequest Delete(string url)
        {
            var downloadHandler = new DownloadHandlerBuffer();
            var request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbDELETE, downloadHandler, null);
            SetupRequest(request);
            return request;
        }

        public static UnityWebRequest Put(string url, string postData = null)
        {
            var downloadHandler = new DownloadHandlerBuffer();
            var uploadHandler = !string.IsNullOrEmpty(postData) ? new UploadHandlerRaw(Encoding.UTF8.GetBytes(postData)) : null;
            var request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPUT, downloadHandler, uploadHandler);
            SetupRequest(request);
            return request;
        }

        public static async Task<bool> TrySendWebRequest(UnityWebRequest request, CancellationToken token, int timeout = -1)
        {
            request.timeout = timeout >= 0 ? timeout : request.timeout;
            
            try
            {
                var op = request.SendWebRequest();
                
                while (!op.isDone)
                {
                    if (token.IsCancellationRequested)
                    {
                        $"Web request cancelled! {request.url}".LogWarning();
                        request.Abort();
                        return false;
                    }

                    await Task.Yield();
                }

                return op.isDone;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public static bool TryGetPayload<T>(UnityWebRequest request, out HttpResponseObject payload, out ServiceResultWrapper<T> errorWrapper) where T : ResultBase
        {
            payload = null;
            errorWrapper = null;

            if (!string.IsNullOrEmpty(request.error))
            {
                if (request.error.Contains("Request timeout"))
                {
                    errorWrapper = new ServiceResultWrapper<T>(ServiceErrors.networkTimeoutError);
                    return false;
                }
            }

            switch (request.result)
            {
                // Connectivity error 
                case UnityWebRequest.Result.ConnectionError:
                    errorWrapper = new ServiceResultWrapper<T>(ServiceErrors.networkUnreachableError);
                    return false;

                case UnityWebRequest.Result.InProgress:
                case UnityWebRequest.Result.DataProcessingError:
                    errorWrapper = new ServiceResultWrapper<T>(ServiceErrors.unknownError);
                    return false;
            }

            var response = request.downloadHandler?.text;

            if (string.IsNullOrEmpty(response))
            {
                errorWrapper = new ServiceResultWrapper<T>(new ServiceError(ServiceErrorCodes.INVALID_DATA, "Connection Error."));
                return false;
            }

            try
            {
                payload = JsonConvert.DeserializeObject<HttpResponseObject>(request.downloadHandler.text);

                var errorCode = payload?.error_code;

                if (errorCode != 0
                    && errorCode != ServiceErrorCodes.INVALID_TOKEN
                    && errorCode != ServiceErrorCodes.TOKEN_EXPIRED)
                {
                    errorWrapper = new ServiceResultWrapper<T>(new ServiceError(payload.error_code, payload.message, payload.errors.ToString()));
                    return false;
                }

                ParseErrorCode(payload?.error_code, payload?.errors);
                return true;
            }
            catch (JsonException e)
            {
                e.LogException();
                errorWrapper = new ServiceResultWrapper<T>(new ServiceError(ServiceErrorCodes.INVALID_DATA, "Invalid JSON Format"));
                return false;
            }
            catch (Exception e)
            {
                e.LogException();
                errorWrapper = new ServiceResultWrapper<T>(ServiceErrors.unknownError);
                return false;
            }
        }

        public static void SetFCMToken(string token)
        {
            GlobalHeaders["x-ube-fcm-token"] = token;
        }

        private static void ParseErrorCode(int? errorCode, object errors)
        {
            // Account Banned!
            if (errorCode == ServiceErrorCodes.ACCOUNT_BANNED_LOGIN || errorCode == ServiceErrorCodes.ACCOUNT_BANNED_AGORA)
            {
                var errorMap = JsonConvert.DeserializeObject<IDictionary<string, object>>(errors.ToString());

                if (errorMap != null && errorMap.TryGetValue("banned", out var bannedObj))
                {
                    var banType = errorCode switch
                    {
                        ServiceErrorCodes.ACCOUNT_BANNED_LOGIN => BanType.Login,
                        ServiceErrorCodes.ACCOUNT_BANNED_AGORA => BanType.Agora,
                        _ => BanType.Login
                    };

                    var bannedObject = JsonConvert.DeserializeObject<BannedObject>(bannedObj.ToString());
                    GlobalNotifier.Instance.Trigger(new AccountBannedEvent(banType, bannedObject));
                }
            }
        }

        private static void SetupRequest(UnityWebRequest request)
        {
            request.timeout = TimeoutSec;

            var headers = GlobalHeaders;
            foreach (var header in headers)
            {
                request.SetRequestHeader(header.Key, header.Value);
            }
        }

        /// <summary>
        /// Returns the error message for a service error if it is considered "displayable".
        /// </summary>
        /// <param name="error">The service error whose error code is to be checked for displayableness.</param>
        /// <param name="displayableErrorCodes">The collection of error codes that are considered to be displayable. If null, will use <see cref="CommonDisplayableErrorCodes"/>.</param>
        /// <returns>An error message. Will be "Connection error. Please try again later." if the error is not displayable. </returns>
        public static string GetDisplayableErrorMessage(ServiceError error, HashSet<int> displayableErrorCodes = null)
        {
            if (displayableErrorCodes == null)
            {
                displayableErrorCodes = CommonDisplayableErrorCodes;
            }

            if (displayableErrorCodes.Contains(error.Code))
            {
                return error.Message;
            }

            return "Connection error. Please try again later.";
        }

        /// <summary>
        /// Error codes whose error messages can be displayed to the user.See error codes at
        /// https://www.notion.so/kumuph/1e5e73f780454f249b4c2d4125a9c122?v=a48989fb4ea6409bba2a4ed1e48ae139.
        /// </summary>
        public static HashSet<int> CommonDisplayableErrorCodes { get; } = new()
        {
            2001, // app version mismatch
            2002, // under maintenance
            2003, // mobile already registered
            2006, // mobile not registered
            2007, // code invalid
            2015, // too many attempts
            2018, // device id invalid
            2020, // username already taken
            2023, // profile already exists
            2024, // profile does not exist
            2025, // no player found connected to mobile,
            2027, // item not found
            2028, // item not owned
            2029, // insufficient balance
            2030, // item already owned
            2031, // mismatched prices
            2033, // items occupy the same slotes
            2035, // kumu account mismatch
            2036, // you already have a linked kumu account
            2037, // kumu account already linked to someone else
            2038, // kumu account not found
            2039, // cannot find kumu account, cannot unlink
            2040, // account linking otp is invalid
            2052, // try again after 1 min
            2053, // otp rate reached, try again after 5 min
            2055, // kumu account has no mobile number
            2057, // purchasing failed, try again later
            2058, // gift not found
            2059, // player not found
            2060, // player already followed
            2061, // player not followed
            2067, // failed to send gifts
            2069, // player has no linked account
            2070, // player not blocked
            2071, // account banned
            2072, // feature banned for account
            2076, // please try again after 1 minute
            ServiceErrorCodes.FIREBASE_ERROR,
        };
    }
}
