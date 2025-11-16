using System;
using Kumu.Kulitan.Common;
using Newtonsoft.Json;
using UnityEngine;

namespace Kumu.Kulitan.Backend
{
    public class EncryptedTokenCachePrefs : ITokenCache<UserTokens>
    {
        private readonly string key;

        public EncryptedTokenCachePrefs(string key)
        {
            this.key = key;
        }

        public void SaveToken(UserTokens token)
        {
            try
            {
                var encodedText = JsonConvert.SerializeObject(token);
                encodedText = RijndaelManagedEncryption.EncryptRijndael(encodedText, "NaqG5EreTtinSKmsakpH");
                PlayerPrefs.SetString(key, encodedText);
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
                var encodedText = PlayerPrefs.GetString(key, string.Empty);

                if (string.IsNullOrEmpty(encodedText))
                {
                    return new();
                }
                
                encodedText = RijndaelManagedEncryption.DecryptRijndael(encodedText, "NaqG5EreTtinSKmsakpH");
                return JsonConvert.DeserializeObject<UserTokens>(encodedText);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw e;
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
