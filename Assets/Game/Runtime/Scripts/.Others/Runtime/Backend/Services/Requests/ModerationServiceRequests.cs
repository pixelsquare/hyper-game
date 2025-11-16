using System;

namespace Kumu.Kulitan.Backend
{
    [Serializable]
    public class ReportUserRequest : RequestCommon
    {
        public string accountId;
        public string category;
        public string subcategory;
        public string info;
        public bool shouldBlock;
    }

    [Serializable]
    public class ReportHangoutRequest : RequestCommon
    {
        public string accountId;
        public string photonRoomId;
        public string photonRoomName;
        public string category;
        public string subcategory;
        public string info;
        public bool shouldBlock;
    }

    [Serializable]
    public class BlockPlayerRequest : RequestCommon
    {
        public string userId;
    }

    [Serializable]
    public class UnblockPlayerRequest : RequestCommon
    {
        public string userId;
    }

    [Serializable]
    public class GetBlockedPlayersRequest : RequestCommon
    {
        public string userId;
    }
}
