using System;

namespace Kumu.Kulitan.Backend
{
    [Serializable]
    public class GetUserProfileRequest : RequestCommon
    {
        // empty
    }

    [Serializable]
    public class CreateUserProfileRequest : RequestCommon
    {
        public string username; // username without discriminator
        public string nickname;
        public int ageRange;
        public int gender;
    }
}
