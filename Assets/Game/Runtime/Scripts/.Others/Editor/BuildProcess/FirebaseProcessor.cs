using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Callbacks;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif

namespace Kumu.Kulitan.Common
{
    public class FirebaseProcessor : IPreprocessBuildWithReport
    {
        public int callbackOrder { get; }

        private const string ANDROID_FILENAME = "google-services";
        private const string IOS_FILENAME = "GoogleService-Info";
        private const string ANDROID_FILE_EXT = ".json";
        private const string IOS_FILE_EXT = ".plist";
        
        private static string AbsConfigDirectory => $"{Application.dataPath}/Firebase/Config";
        
        [PostProcessBuild]
        public static void OnPostProcess(BuildTarget buildTarget, string buildPath)
        {
#if UNITY_IOS
            if (!Application.identifier.EndsWith("dev")
                && !Application.identifier.EndsWith("stg"))
            {
                return;
            }
            
            string schemePath = buildPath + "/Unity-iPhone.xcodeproj/xcshareddata/xcschemes/Unity-iPhone.xcscheme";

            var xcscheme = new XcScheme();
            xcscheme.ReadFromFile(schemePath);
            xcscheme.AddArgumentPassedOnLaunch("-FIRDebugEnabled");
            xcscheme.WriteToFile(schemePath);
#endif
        }
        
        public void OnPreprocessBuild(BuildReport report)
        {
            WriteTargetConfig();
        }

        [MenuItem("Tools/Firebase/Test Preprocess")]
        public static void WriteTargetConfig()
        {
            var suffix = "-prod";
            if (Application.identifier.EndsWith("dev"))
            {
                suffix = "-dev";
            }
            else if (Application.identifier.EndsWith("stg"))
            {
                suffix = "-stg";
            }
            
#if UNITY_IOS
            ProcessIos(suffix);
#elif UNITY_ANDROID
            ProcessAndroid(suffix);
#endif
        }

        private static void ProcessIos(string suffix)
        {
            var target = $"{AbsConfigDirectory}/{IOS_FILENAME}{suffix}{IOS_FILE_EXT}";
            var container = $"{AbsConfigDirectory}/{IOS_FILENAME}{IOS_FILE_EXT}";
            
            File.Copy(target, container, true);
            
            Debug.Log($"copying <color=#00ffff>{target}</color>");
        }

        private static void ProcessAndroid(string suffix)
        {
            var target = $"{AbsConfigDirectory}/{ANDROID_FILENAME}{suffix}{ANDROID_FILE_EXT}";
            var container = $"{AbsConfigDirectory}/{ANDROID_FILENAME}{ANDROID_FILE_EXT}";
            
            File.Copy(target, container, true);
            
            Debug.Log($"copying <color=#00ffff>{target}</color>");

        }
    }
}
