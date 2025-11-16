using UnityEngine;

namespace Kumu.Kulitan.Hangout
{
    [CreateAssetMenu(menuName = "Config/Hangout/Version Object")]
    public class VersionObject : ScriptableObject
    {
        public const string RESOURCE_NAME = "ube-version";

        public int major;
        public int minor;
        public int patch;
        public int build;

        public override string ToString()
        {
            return $"{major}.{minor}.{patch}b{build}";
        }

        public static VersionObject Fetch()
        {
            return Resources.Load<VersionObject>(RESOURCE_NAME);
        }

        public static string FetchVersionString()
        {
            var versionObject = Fetch();
            return $"{versionObject.major}.{versionObject.minor}.{versionObject.patch}b{versionObject.build}";
        }
    }
}
