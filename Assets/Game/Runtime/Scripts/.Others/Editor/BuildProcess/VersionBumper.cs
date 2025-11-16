using System.IO;
using Kumu.Kulitan.Hangout;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Kumu.Kulitan.Common
{
    public class VersionBumper : IPreprocessBuildWithReport
    {
        private string numberFormat = "D2";
        private string versionObjectName = "ube-version";

        public int callbackOrder { get { return 0; } }

        public void OnPreprocessBuild(BuildReport report)
        {
            var filePath = Path.Combine(Application.dataPath, "../../../", VersionDisplay.VERSION_FILE_NAME);

            var results = AssetDatabase.FindAssets(versionObjectName);
            var assetPath = AssetDatabase.GUIDToAssetPath(results[0]);
            var versionObj = AssetDatabase.LoadAssetAtPath<VersionObject>(assetPath);

            if (results.Length == 0)
            {
                Debug.LogError("[VersionBumper] Version object not found.");
                return;
            }

            if (File.Exists(filePath))
            {
                //version file exists, proceeding to update scriptable object
                var reader = new StreamReader(filePath);
                var readData = JsonUtility.FromJson<VersionData>(reader.ReadToEnd());
                reader.Close();

                versionObj.major = readData.major;
                versionObj.minor = readData.minor;
                versionObj.patch = readData.patch;
                versionObj.build = readData.build;

                EditorUtility.SetDirty(versionObj);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            var projectVersion = $"{versionObj.major}.{versionObj.minor}.{versionObj.patch}";
            var bundleVersionCode = versionObj.major.ToString(numberFormat) + versionObj.minor.ToString(numberFormat) +
                                    versionObj.patch.ToString(numberFormat) + versionObj.build.ToString(numberFormat);

            PlayerSettings.bundleVersion = projectVersion;
            PlayerSettings.Android.bundleVersionCode = int.Parse(bundleVersionCode);
            PlayerSettings.iOS.buildNumber = versionObj.build.ToString();
        }
    }
}
