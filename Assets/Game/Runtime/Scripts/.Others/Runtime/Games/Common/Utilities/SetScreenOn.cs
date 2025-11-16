using UnityEngine;

namespace Kumu.Kulitan.Hangout
{
    public class SetScreenOn
    {
        [RuntimeInitializeOnLoadMethod]
        private static void KeepScreenOn()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }
    }
}
