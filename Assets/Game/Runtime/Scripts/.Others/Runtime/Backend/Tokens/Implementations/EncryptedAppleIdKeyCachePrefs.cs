using System;
using Kumu.Extensions;
using Kumu.Kulitan.Common;
using Newtonsoft.Json;
using UnityEngine;

namespace Kumu.Kulitan.Backend
{
    public class EncryptedAppleIdKeyCachePrefs : ITokenCache<string>
    {
        private readonly string key;
        
        public EncryptedAppleIdKeyCachePrefs(string key)
        {
            this.key = key;
        }
        
        public void SaveToken(string userId)
        {
            try
            {
                var encodedText = JsonConvert.SerializeObject(userId);
                encodedText = RijndaelManagedEncryption.EncryptRijndael(encodedText, "NaqG5EreTtinSKmsakpH");
                PlayerPrefs.SetString(key, encodedText);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw e;
            }
        }

        public string LoadToken()
        {
            try
            {
                var encodedText = PlayerPrefs.GetString(key, string.Empty);

                if (string.IsNullOrEmpty(encodedText))
                {
                    return "";
                }
                
                encodedText = RijndaelManagedEncryption.DecryptRijndael(encodedText, "NaqG5EreTtinSKmsakpH");
                return JsonConvert.DeserializeObject<string>(encodedText);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw e;
            }
        }

        public bool IsValid()
        {
            $"has key:{key} {PlayerPrefs.HasKey(key)}".Log();
            return PlayerPrefs.HasKey(key);
        }

        public void Delete()
        {
            PlayerPrefs.DeleteKey(key);
        }
    }
}
