using System;

namespace Kumu.Kulitan.Backend
{
    [Flags]
    public enum SocialState
    {
        None = 0, // 1 << 0, 0b
        Blocking = 1, // 1 << 1, 1b - blocked by you
        Blocker = 2, // 1 << 2, 10b - blocked by them
        Favorite = 4, // 1 << 3, 100b
        Following = 8, // 1 << 4, 1000b
        Follower = 16, // 1 << 5, 10000b
        Friends = 32, // 1 << 6, 10000b
    }
}
