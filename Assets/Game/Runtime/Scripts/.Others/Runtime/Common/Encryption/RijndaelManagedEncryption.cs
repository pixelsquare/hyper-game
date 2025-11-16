using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Kumu.Kulitan.Common
{
    public class RijndaelManagedEncryption
    {
        internal const string InputKey = "AD10B5AE-0949-4C11-BF43-5605F7D1BEE7";

        public static string EncryptRijndael(string text, string salt)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException(nameof(text));
            }

            var aesAlgo = NewRijndaelManaged(salt);

            var encryptor = aesAlgo.CreateEncryptor(aesAlgo.Key, aesAlgo.IV);
            using var msEncrypt = new MemoryStream();
            using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);

            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(text);
            }

            return Convert.ToBase64String(msEncrypt.ToArray());
        }

        public static bool IsBase64String(string base64String)
        {
            base64String = base64String.Trim();
            return base64String.Length % 4 == 0
                 && Regex.IsMatch(base64String, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);
        }

        public static string DecryptRijndael(string cipherText, string salt)
        {
            if (string.IsNullOrEmpty(cipherText))
            {
                throw new ArgumentNullException(nameof(cipherText));
            }

            if (!IsBase64String(cipherText))
            {
                throw new Exception("The cipherText input parameter is not base64 encoded.");
            }

            var resultStr = "";

            var aesAlgo = NewRijndaelManaged(salt);
            var decryptor = aesAlgo.CreateDecryptor(aesAlgo.Key, aesAlgo.IV);
            var cipher = Convert.FromBase64String(cipherText);

            using var msDecrypt = new MemoryStream(cipher);
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);

            using (var srDecrypt = new StreamReader(csDecrypt))
            {
                resultStr = srDecrypt.ReadToEnd();
            }

            return resultStr;
        }

        public static RijndaelManaged NewRijndaelManaged(string salt)
        {
            if (salt == null)
            {
                throw new ArgumentException(nameof(salt));
            }

            var saltBytes = Encoding.ASCII.GetBytes(salt);
            var key = new Rfc2898DeriveBytes(InputKey, saltBytes);

            var aesAlgo = new RijndaelManaged();
            aesAlgo.Key = key.GetBytes(aesAlgo.KeySize / 8);
            aesAlgo.IV = key.GetBytes(aesAlgo.BlockSize / 8);

            return aesAlgo;
        }
    }
}
