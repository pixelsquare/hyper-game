using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "AppConfig", menuName = "Game/Config/App Config")]
    public class AppConfig : ScriptableObject
    {
        [SerializeField] private int targetFrameRate = 30;
        [SerializeField] private bool runInBackground = false;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void RunConfig()
        {
            var appConfig = Resources.Load<AppConfig>(nameof(AppConfig));
            Debug.Log($"{(appConfig ? "AppConfig found." : "<color=red>AppConfig not found. Default settings applied.</color>")}");
            Application.targetFrameRate = appConfig?.targetFrameRate ?? 30;
            Application.runInBackground = appConfig?.runInBackground ?? false;
        }
    }
}
