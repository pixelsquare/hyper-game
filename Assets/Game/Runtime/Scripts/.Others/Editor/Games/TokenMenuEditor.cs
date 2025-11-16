using Kumu.Kulitan.Backend;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Kumu.Kulitan
{
    public static class TokenMenuEditor
    {
        private static readonly ITokenCache<UserTokens> TokenCache = new EncryptedTokenCachePrefs(BackendUtil.TokenCacheKey);

        [MenuItem("Tools/Token/Copy Access Token", false, 2)]
        public static void CopyAccessToken()
        {
            Debug.Log(BackendUtil.AuthToken);
            GUIUtility.systemCopyBuffer = BackendUtil.AuthToken;
        }

        [MenuItem("Tools/Token/Copy SSO Token", false, 2)]
        public static void CopySsoToken()
        {
            Debug.Log(BackendUtil.SsoToken);
            GUIUtility.systemCopyBuffer = BackendUtil.SsoToken;
        }

        [MenuItem("Tools/Token/Print User Tokens", false, 3)]
        public static void PrintUserTokens()
        {
            var userTokens = TokenCache.LoadToken();
            Debug.Log(JsonConvert.SerializeObject(userTokens));
        }

        [MenuItem("Tools/Token/Print Token Cache", false, 3)]
        public static void PrintTokenCache()
        {
            Debug.Log(PlayerPrefs.GetString(BackendUtil.TokenCacheKey));
        }
    }
}
