using System;

namespace Kumu.Kulitan.Hangout
{
    [Serializable]
    public struct PlayerDetails
    {
        public string accountId;
        public uint playerId;
        public string userName;
        public string nickName;
    }
}
