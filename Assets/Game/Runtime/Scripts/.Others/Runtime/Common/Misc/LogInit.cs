using Photon.Deterministic;
using Quantum;
using UnityEngine;

namespace Kumu.Kulitan
{
    public static class LogInit
    {
        [RuntimeInitializeOnLoadMethod]
        public static void Initialize()
        {
#if !ENABLE_LOGS
            Firebase.FirebaseApp.LogLevel = Firebase.LogLevel.Assert;
            PhotonServerSettings.Instance.AppSettings.NetworkLogging = 0;
#endif

            Log.Reset();
            Log.Init(Debug.Log, Debug.LogWarning, Debug.LogError, Debug.LogException);

            DeterministicLog.Reset();
            DeterministicLog.Init(Debug.Log, Debug.LogWarning, Debug.LogError, Debug.LogException);
        }
    }
}
