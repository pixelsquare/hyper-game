using System;

namespace Kumu.Kulitan.Backend
{
    [Serializable]
    public class GetRTCTokenRequest : RequestCommon
    {
        public string channel;
    }

    [Serializable]
    public class GetRTMTokenRequest : RequestCommon
    {
    }
}
