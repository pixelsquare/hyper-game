using UnityEngine;

namespace Kumu.Kulitan.Hangout
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Agora App Config")]
    public class AgoraAppConfigScriptableObject : ScriptableObject
    {
        [SerializeField] private string devAppID;
        [SerializeField] private string devCertificate;
        [SerializeField] private string releaseAppId;
        [SerializeField] private string releaseCertificate;

        public string DevAppID => devAppID;
        public string DevCertificate => devCertificate;
        public string ReleaseAppID => releaseAppId;
        public string ReleaseCertificate => releaseCertificate;
    }
}
