#if UNITY_IOS
using System.IO;
using AppleAuth.Editor;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

namespace Kumu.Kulitan.Common
{
    public class XCodeBuildProcessor : IPostprocessBuildWithReport
    {
        private const string EMBED_SWIFT_LIBRARY_PROPERTY = "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES";
        private const string DYLIB_INSTALL_NAME_BASE_PROPERTY = "DYLIB_INSTALL_NAME_BASE";
        private const string XCODE_YES_STRING = "YES";
        private const string XCODE_NO_STRING = "NO";

        public int callbackOrder { get; }

        public void OnPostprocessBuild(BuildReport report)
        {
            ModifyFrameworks(report.summary.outputPath);
            DisableBitcode(report.summary);
            AddToEntitlements(report.summary.platform, report.summary.outputPath);
            RemoveRPath(report.summary.outputPath);
            ModifyPlist(report.summary.platform, report.summary.outputPath);
        }

        private void RemoveRPath(string path)
        {
            var projPath = PBXProject.GetPBXProjectPath(path);
            var project = new PBXProject();
            project.ReadFromFile(projPath);

            var mainTargetGuid = project.GetUnityMainTargetGuid();
            var frameworkTargetGuid = project.GetUnityFrameworkTargetGuid();

            project.SetBuildProperty(mainTargetGuid, DYLIB_INSTALL_NAME_BASE_PROPERTY, "");
            project.SetBuildProperty(frameworkTargetGuid, DYLIB_INSTALL_NAME_BASE_PROPERTY, "");

            project.WriteToFile(projPath);

            Debug.Log("Removing dynamic install name @rpath");
        }

        private void ModifyFrameworks(string path)
        {
            var projPath = PBXProject.GetPBXProjectPath(path);

            var project = new PBXProject();
            project.ReadFromFile(projPath);

            var mainTargetGuid = project.GetUnityMainTargetGuid();

            foreach (var targetGuid in new[] { mainTargetGuid, project.GetUnityFrameworkTargetGuid() })
            {
                Debug.Log($"Disabling {EMBED_SWIFT_LIBRARY_PROPERTY} for {targetGuid}");
                project.SetBuildProperty(targetGuid, EMBED_SWIFT_LIBRARY_PROPERTY, XCODE_NO_STRING);
            }

            project.SetBuildProperty(mainTargetGuid, EMBED_SWIFT_LIBRARY_PROPERTY, XCODE_YES_STRING);

            // Add UserNotifications
            project.AddFrameworkToProject(mainTargetGuid, "UserNotifications.framework", false);

            project.WriteToFile(projPath);

            Debug.Log("Finished XCodeBuildProcessor - ModifyFrameworks");
        }

        /// Taken from https://support.unity.com/hc/en-us/articles/207942813-How-can-I-disable-Bitcode-support-
        private void DisableBitcode(BuildSummary summary)
        {
            if (summary.platform != BuildTarget.iOS)
            {
                return;
            }

            var projectPath = summary.outputPath + "/Unity-iPhone.xcodeproj/project.pbxproj";

            var pbxProject = new PBXProject();
            pbxProject.ReadFromFile(projectPath);

            //Disabling Bitcode on all targets

            //Main
            var target = pbxProject.GetUnityMainTargetGuid();
            pbxProject.SetBuildProperty(target, "ENABLE_BITCODE", "NO");

            //Unity Tests
            target = pbxProject.TargetGuidByName(PBXProject.GetUnityTestTargetName());
            pbxProject.SetBuildProperty(target, "ENABLE_BITCODE", "NO");

            //Unity Framework
            target = pbxProject.GetUnityFrameworkTargetGuid();
            pbxProject.SetBuildProperty(target, "ENABLE_BITCODE", "NO");

            pbxProject.WriteToFile(projectPath);
        }

        private static void AddToEntitlements(BuildTarget buildTarget, string buildPath)
        {
            if (buildTarget != BuildTarget.iOS)
            {
                return;
            }

            Debug.Log("Adding entitlements...");

            // get project info
            var pbxPath = PBXProject.GetPBXProjectPath(buildPath);
            var proj = new PBXProject();
            proj.ReadFromFile(pbxPath);
            var guid = proj.GetUnityMainTargetGuid();

            // get entitlements path
            var entitlementsPath = "Unity-iPhone/Unity-iPhone.entitlements";

            // create capabilities manager
            var capManager = new ProjectCapabilityManager(pbxPath, entitlementsPath, null, guid);

            // Add necessary capabilities
            var isDevelopment = false;
#if UBE_DEV || UBE_STAGING
            isDevelopment = true;
#endif
            Debug.Log("Adding Push notifications...");
            capManager.AddPushNotifications(isDevelopment);
            capManager.AddSignInWithAppleWithCompatibility(proj.GetUnityFrameworkTargetGuid());

            // Write to file
            capManager.WriteToFile();
        }

        [PostProcessBuild(45)] //must be between 40 and 50 to ensure that it's not overriden by Podfile generation (40) and that it's added before "pod install" (50)
        public static void FixPodFile(BuildTarget buildTarget, string buildPath)
        {
            Debug.Log("Fixing pod file...");

            if (buildTarget != BuildTarget.iOS)
            {
                return;
            }

            using var sw = File.AppendText(buildPath + "/Podfile");

            sw.WriteLine("post_install do |installer|");
            sw.WriteLine("installer.generated_projects.each do |project|");
            sw.WriteLine("project.targets.each do |target|");
            sw.WriteLine("target.build_configurations.each do |config|");
            sw.WriteLine("config.build_settings[\"DEVELOPMENT_TEAM\"] = \"6Z832573MK\""); // kumumedia 
            sw.WriteLine("end\nend\nend\nend");
        }

        private static void ModifyPlist(BuildTarget buildTarget, string path)
        {
            Debug.Log("Modifying Plist...");

            if (buildTarget != BuildTarget.iOS)
            {
                return;
            }

            var plistPath = path + "/Info.plist";

            var plist = new PlistDocument();
            plist.ReadFromString(File.ReadAllText(plistPath));

            var rootDict = plist.root;
            rootDict.SetBoolean("ITSAppUsesNonExemptEncryption", false);

            File.WriteAllText(plistPath, plist.WriteToString());
        }
    }
}
#endif
