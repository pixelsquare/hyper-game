using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Santelmo.Rinsurv.Backend;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace Santelmo.Rinsurv.Editor
{
    [InitializeOnLoad]
    public static class ProjectEnvMenuEditor
    {
        public enum Environment
        {
            DEVELOPMENT,
            STAGING,
            PRODUCTION
        }

        public struct EnvConfig
        {
            public string IconPath;
            public string AppNameSuffix;
            public string BundleIdSuffix;
            public string ConfigName;
            public string MenuName;
            public string[] Symbols;
        }

        // Specify here which build targets to append the symbols.
        private readonly static NamedBuildTarget[] BuildTargets =
        {
            NamedBuildTarget.Android,
            NamedBuildTarget.Standalone,
            NamedBuildTarget.iOS
        };

        private readonly static Dictionary<Environment, EnvConfig> EnvConfigMap = new()
        {
            {
                Environment.DEVELOPMENT, new EnvConfig
                {
                    IconPath = "",
                    AppNameSuffix = "Dev",
                    BundleIdSuffix = ".dev",
                    ConfigName = "Development",
                    MenuName = "Santelmo/Environment/Development",
                    Symbols = new[] { "RINSURV_DEV", "ENABLE_LOGS" }
                }
            },
            {
                Environment.STAGING, new EnvConfig
                {
                    IconPath = "",
                    AppNameSuffix = "Stg",
                    BundleIdSuffix = ".stg",
                    ConfigName = "Staging",
                    MenuName = "Santelmo/Environment/Staging",
                    Symbols = new[] { "RINSURV_STG", "ENABLE_LOGS" }
                }
            },
            {
                Environment.PRODUCTION, new EnvConfig
                {
                    IconPath = "",
                    AppNameSuffix = "",
                    BundleIdSuffix = "",
                    ConfigName = "Production",
                    MenuName = "Santelmo/Environment/Production",
                    Symbols = new[] { "" }
                }
            }
        };

        private static NamedBuildTarget ActiveNamedBuildTarget
        {
            get
            {
                var buildTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
                return NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup);
            }
        }

        private static Environment _curEnvironment;

        public static Environment CurrentEnvironment
        {
            get
            {
                var bundleIdentifier = PlayerSettings.GetApplicationIdentifier(ActiveNamedBuildTarget);

                _curEnvironment = (from envConfig in EnvConfigMap
                                   where bundleIdentifier.Contains(envConfig.Value.BundleIdSuffix)
                                   select envConfig.Key).First();

                return _curEnvironment;
            }
            set
            {
                var newEnvironment = value;

                Debug.Log($"Setting Environment to {newEnvironment}");

                var curEnvConfig = EnvConfigMap[_curEnvironment];
                var newEnvConfig = EnvConfigMap[newEnvironment];

                foreach (var buildTarget in BuildTargets)
                {
                    RemoveScriptingDefineSymbols(buildTarget, curEnvConfig.Symbols);
                    AppendScriptingDefineSymbols(buildTarget, newEnvConfig.Symbols);
                    SetApplicationNameSuffix(buildTarget, newEnvConfig.AppNameSuffix);
                    SetApplicationBundleIdSuffix(buildTarget, newEnvConfig.BundleIdSuffix);
                    SetApplicationIcon(buildTarget, newEnvConfig.IconPath);
                    SetupFirebaseConfigs(buildTarget, newEnvConfig.ConfigName);
                }

                AssetDatabase.SaveAssets();
                EditorUtility.RequestScriptReload();
                _curEnvironment = newEnvironment;
                SetCheckedMenu(_curEnvironment);
            }
        }

        private readonly static Regex SuffixRegex = new(@"[ .]([D-d]ev|[S-s]tg)");

        static ProjectEnvMenuEditor()
        {
            EditorApplication.update += Initialize;
        }

        private static void Initialize()
        {
            EditorApplication.update -= Initialize;
            SetCheckedMenu(CurrentEnvironment);
        }

        [MenuItem("Santelmo/Environment/Development", false, 0)]
        private static void SetDevelopmentEnvironment()
        {
            CurrentEnvironment = Environment.DEVELOPMENT;
        }

        [MenuItem("Santelmo/Environment/Staging", false, 1)]
        private static void SetStagingEnvironment()
        {
            CurrentEnvironment = Environment.STAGING;
        }

        [MenuItem("Santelmo/Environment/Production", false, 2)]
        private static void SetProductionEnvironment()
        {
            CurrentEnvironment = Environment.PRODUCTION;
        }

        private static void SetCheckedMenu(Environment environment)
        {
            foreach (var envConfig in EnvConfigMap)
            {
                Menu.SetChecked(envConfig.Value.MenuName, envConfig.Key == environment);
            }
        }

        private static void AppendScriptingDefineSymbols(NamedBuildTarget buildTarget, params string[] symbols)
        {
            PlayerSettings.GetScriptingDefineSymbols(buildTarget, out var curSymbols);
            var newSymbols = curSymbols.Union(symbols).Distinct().ToArray();
            PlayerSettings.SetScriptingDefineSymbols(buildTarget, newSymbols);
            Debug.Log($"Added symbols for {buildTarget.TargetName}: {string.Join(",", symbols)}");
        }

        private static void RemoveScriptingDefineSymbols(NamedBuildTarget buildTarget, params string[] symbols)
        {
            PlayerSettings.GetScriptingDefineSymbols(buildTarget, out var curSymbols);
            var newSymbols = curSymbols.Except(symbols).Distinct().ToArray();
            PlayerSettings.SetScriptingDefineSymbols(buildTarget, newSymbols);
            Debug.Log($"Removed symbols for {buildTarget.TargetName}: {string.Join(",", symbols)}");
        }

        private static void SetApplicationNameSuffix(NamedBuildTarget buildTarget, string appNameSuffix)
        {
            var curProductName = PlayerSettings.productName;
            curProductName = SuffixRegex.Replace(curProductName, "");
            var newProductName = $"{curProductName} {appNameSuffix}";
            PlayerSettings.productName = newProductName.TrimEnd();
            Debug.Log($"Set application product name for {buildTarget.TargetName}: {newProductName}");
        }

        private static void SetApplicationBundleIdSuffix(NamedBuildTarget buildTarget, string appBundleSuffix)
        {
            var curBundleId = PlayerSettings.GetApplicationIdentifier(buildTarget);
            curBundleId = SuffixRegex.Replace(curBundleId, "");
            var newBundleId = $"{curBundleId}{appBundleSuffix}";
            PlayerSettings.SetApplicationIdentifier(buildTarget, newBundleId);
            Debug.Log($"Set application identifier for {buildTarget.TargetName}: {newBundleId}");
        }

        private static void SetupFirebaseConfigs(NamedBuildTarget buildTarget, string configName)
        {
            var rootConfigPath = "Assets/Firebase/Configs";
            var configMap = (from configPath in Directory.GetDirectories(rootConfigPath)
                             select new { id = Path.GetFileNameWithoutExtension(configPath), path = configPath })
                            .ToDictionary(x => x.id, y => y.path);

            var configFiles = Directory.GetFiles(configMap[configName])
                                              .Where(x => !x.Contains(".meta"))
                                              .ToArray();

            foreach (var configFile in configFiles)
            {
                var fileName = Path.GetFileName(configFile);
                var fileExt = Path.GetExtension(configFile);
                fileName = fileName.Substring(0, fileName.LastIndexOf('-'));
                File.Copy(configFile, $"{rootConfigPath}/{fileName}{fileExt}", true);
            }
            
            var filePath = configFiles.FirstOrDefault(x => x.Contains(".json"));
            
            //Fetches the Web Client ID in google-services.json to add to our Google Services Config
            if (File.Exists(filePath))
            {
                if (filePath != null)
                {
                    var data = File.ReadAllText(filePath);
                    var googleServices = JsonConvert.DeserializeObject<GoogleServices>(data);
                    var path = AssetDatabase.FindAssets("t:GoogleServicesConfig GoogleServicesConfig");
                    var googleConfig = AssetDatabase.LoadAssetAtPath<GoogleServicesConfig>(AssetDatabase.GUIDToAssetPath(path[0]));
                    googleConfig.GoogleWebClientId = googleServices?.client.FirstOrDefault()?.oauth_client.FirstOrDefault(x => x.client_type == 3)?.client_id;
                }
            }
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"Applying firebase configs to {buildTarget.TargetName}: {string.Join(',', configFiles)}");
        }

        private static void SetApplicationIcon(NamedBuildTarget buildTarget, string iconPath)
        {
            if (!File.Exists(iconPath))
            {
                Debug.LogWarning("Application Icon Path not found.");
                return;
            }

            var appIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);

            var curAppIcons = PlayerSettings.GetIconSizes(buildTarget, IconKind.Any);
            var newAppIcons = new Texture2D[curAppIcons.Length];
            Array.Fill(newAppIcons, appIcon);

            PlayerSettings.SetIcons(buildTarget, newAppIcons, IconKind.Any);
        }
    }
}
