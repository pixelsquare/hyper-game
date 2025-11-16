using System;

namespace Kumu.Kulitan.Backend
{
    [Serializable]
    public class RegisterFCMTokenRequest : RequestCommon
    {
        public string token;
    }
}
