using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Kumu.Kulitan.Backend
{
    public class TokenCachePrefs : ITokenCache<UserTokens>
    {
        private readonly string key;

        public TokenCachePrefs(string key)
        {
            this.key = key;
        }

        public void SaveToken(UserTokens token)
        {
            try
            {
                var tokenString = JsonConvert.SerializeObject(token);
                PlayerPrefs.SetString(key, tokenString);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw e;
            }
        }

        public UserTokens LoadToken()
        {
            try
            {
                var tokenString = PlayerPrefs.GetString(key, string.Empty);

                if (string.IsNullOrEmpty(tokenString))
                {
                    return new UserTokens();
                }
                
                return JsonConvert.DeserializeObject<UserTokens>(tokenString);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        public bool IsValid()
        {
            return PlayerPrefs.HasKey(key);
        }

        public void Delete()
        {
            PlayerPrefs.DeleteKey(key);
        }
    }
}
