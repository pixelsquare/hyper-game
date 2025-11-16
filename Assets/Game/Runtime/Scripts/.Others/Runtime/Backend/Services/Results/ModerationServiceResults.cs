namespace Kumu.Kulitan.Backend
{
    public class ReportUserResult : ResultBase
    {
    }

    public class ReportHangoutResult : ResultBase
    {
    }

    public class BlockPlayerResult : ResultBase
    {
    }

    public class UnblockPlayerResult : ResultBase
    {
    }

    public class GetBlockedPlayersResult : ResultBase
    {
        public string[] blockedUserIds;
    }
}
