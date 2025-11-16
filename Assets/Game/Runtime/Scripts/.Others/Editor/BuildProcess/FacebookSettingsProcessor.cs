using System.Linq;
using Facebook.Unity.Editor;
using Facebook.Unity.Settings;
using Kumu.Extensions;
using Kumu.Kulitan.Backend.Editor;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Kumu.Kulitan.Common
{
    /// <summary>
    /// Sets FacebookSettings properties based on environment selected (dev/stg/prod) while application is being built.
    /// </summary>
    public class FacebookSettingsProcessor : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        public int callbackOrder => 50;

        public void OnPreprocessBuild(BuildReport report)
        {
            SelectFBSettingsAppIndexBasedOnEnvironment();
        }

        public void OnPostprocessBuild(BuildReport report)
        {
            if (report.summary.platform != BuildTarget.Android)
            {
                return;
            }
            
            // Force the manifest to be regenerated to apply the properties that were set in the build preprocess
            RegenerateManifest();
        }

        private static void RegenerateManifest()
        {
            "Regenerating Facebook manifest...".Log();
            ManifestMod.GenerateManifest();
        }

        private static void SelectFBSettingsAppIndexBasedOnEnvironment()
        {
            "Selecting FacebookSettings app index...".Log();

            var facebookSettings = FindAndLoadAssetOfType<FacebookSettings>();
            var facebookSettingsSerializedObject = new SerializedObject(facebookSettings);
            var appIndexProp = facebookSettingsSerializedObject.FindProperty("selectedAppIndex");

            var appIndex = EnvironmentConfigMenu.CurrentEnvironment switch
            {
                EnvironmentConfigMenu.Environment.PRODUCTION => 2,
                EnvironmentConfigMenu.Environment.STAGING => 1,
                _ => 0 // dev
            };
            
            $"Index selected: {appIndex.ToString()}".Log();

            appIndexProp.intValue = appIndex;
            facebookSettingsSerializedObject.ApplyModifiedProperties();
            
            var appLabel = facebookSettingsSerializedObject.FindProperty("appLabels").GetArrayElementAtIndex(appIndex).stringValue;
            var appId = facebookSettingsSerializedObject.FindProperty("appIds").GetArrayElementAtIndex(appIndex).stringValue;
            var clientToken = facebookSettingsSerializedObject.FindProperty("clientTokens").GetArrayElementAtIndex(appIndex).stringValue;
            
            $"FacebookSettings applied. AppLabel: {appLabel}, AppId: {appId}, ClientToken: {clientToken}".Log();
        }

        private static T FindAndLoadAssetOfType<T>() where T : UnityEngine.Object
        {
            var guid = AssetDatabase.FindAssets($"t:{typeof(T)}").FirstOrDefault();
            UnityEngine.Assertions.Assert.IsFalse(string.IsNullOrWhiteSpace(guid));
            
            var path = AssetDatabase.GUIDToAssetPath(guid);
            UnityEngine.Assertions.Assert.IsFalse(string.IsNullOrWhiteSpace(path));
            
            return AssetDatabase.LoadAssetAtPath<T>(path);
        }
    }
}
