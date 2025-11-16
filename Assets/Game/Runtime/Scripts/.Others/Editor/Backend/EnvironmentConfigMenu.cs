using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Kumu.Extensions;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace Kumu.Kulitan.Backend.Editor
{
    [InitializeOnLoad]
    public static class EnvironmentConfigMenu
    {
        public enum Environment { DEVELOPMENT_MOCKED, DEVELOPMENT, STAGING, PRODUCTION }

        private static readonly NamedBuildTarget[] BuildTargets = { NamedBuildTarget.Android, NamedBuildTarget.Standalone, NamedBuildTarget.iOS };

        private static Dictionary<Environment, string> env2IconMap = new()
        {
            { Environment.DEVELOPMENT_MOCKED, "Assets/Submodules/kumu-ube-art-assets/UI/02 icons/icn_appStore_dev.png" },
            { Environment.DEVELOPMENT, "Assets/Submodules/kumu-ube-art-assets/UI/02 icons/icn_appStore_dev.png" },
            { Environment.STAGING, "Assets/Submodules/kumu-ube-art-assets/UI/02 icons/icn_appStore_stg.png" },
            { Environment.PRODUCTION, "Assets/Submodules/kumu-ube-art-assets/UI/02 icons/icn_appStore.png" },
        };

        private static readonly Regex PostfixRegex = new Regex(@"[-_](dev|stg)");

        public static Environment CurrentEnvironment
        {
            get
            {
                var identifier = PlayerSettings.GetApplicationIdentifier(NamedBuildTarget.Standalone);

                if (PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.Standalone).Contains("USES_MOCKS"))
                {
                    return Environment.DEVELOPMENT_MOCKED;
                }

                if (identifier.Contains("dev"))
                {
                    return Environment.DEVELOPMENT;
                }

                if (identifier.Contains("stg"))
                {
                    return Environment.STAGING;
                }

                return Environment.PRODUCTION;
            }
            set
            {
                $"Setting environment to {value.ToString()}...".Log();

                foreach (var buildTarget in BuildTargets)
                {
                    RemoveScriptingDefineSymbol(buildTarget, GetExcludedSymbols(EnvironmentSymbolMap[value]));
                    AppendScriptingDefineSymbol(buildTarget, EnvironmentSymbolMap[value]);
                    $"New symbols for {buildTarget.TargetName}: {PlayerSettings.GetScriptingDefineSymbols(buildTarget)}".Log();
                    SetApplicationIdentifierPostfix(buildTarget, value);
                    SetApplicationNamePostfix(buildTarget, value);
                    SetAppIcon(buildTarget, value, env2IconMap);
                }

                // Finalize
                AssetDatabase.SaveAssets();
                EditorUtility.RequestScriptReload();
                SetCheckedMenu(value);
            }
        }

        private static readonly Dictionary<Environment, string[]> EnvironmentSymbolMap = new()
        {
            { Environment.DEVELOPMENT_MOCKED, new[] { "UBE_DEV", "USES_MOCKS", "ENABLE_LOGS" } },
            { Environment.DEVELOPMENT, new[] { "UBE_DEV", "ENABLE_LOGS" } },
            { Environment.STAGING, new[] { "UBE_STAGING", "PHOTON_STAGING", "ENABLE_LOGS" } },
            { Environment.PRODUCTION, new[] { "UBE_RELEASE", "PHOTON_RELEASE", "ENABLE_ROOT" } }
        };

        private static readonly Dictionary<Environment, string> EnvironmentMenuMap = new()
        {
            { Environment.DEVELOPMENT_MOCKED, "Environment/DevelopmentMocked" },
            { Environment.DEVELOPMENT, "Environment/Development" },
            { Environment.STAGING, "Environment/Staging" },
            { Environment.PRODUCTION, "Environment/Production" }
        };

        private static string[] GetExcludedSymbols(IEnumerable<string> symbols)
        {
            var result = EnvironmentSymbolMap.SelectMany(s => s.Value).Distinct().Except(symbols).ToArray();

            return result;
        }

        private static void SetCheckedMenu(Environment environment)
        {
            foreach (var m in EnvironmentMenuMap)
            {
                Menu.SetChecked(m.Value, environment == m.Key);
            }
        }

        static EnvironmentConfigMenu()
        {
            // Initialize on the first frame of editor update.
            EditorApplication.update += Initialize;
        }

        [MenuItem("Environment/Production")]
        private static void SetProductionEnvironment()
        {
            CurrentEnvironment = Environment.PRODUCTION;
        }

        [MenuItem("Environment/Staging")]
        private static void SetStagingEnvironment()
        {
            CurrentEnvironment = Environment.STAGING;
        }

        [MenuItem("Environment/Development")]
        private static void SetDevelopmentEnvironment()
        {
            CurrentEnvironment = Environment.DEVELOPMENT;
        }

        [MenuItem("Environment/DevelopmentMocked")]
        private static void SetDevelopmentMockedEnvironment()
        {
            CurrentEnvironment = Environment.DEVELOPMENT_MOCKED;
        }

        private static void Initialize()
        {
            EditorApplication.update -= Initialize;
            SetCheckedMenu(CurrentEnvironment);
        }

        private static void AppendScriptingDefineSymbol(NamedBuildTarget buildTarget, params string[] symbols)
        {
            PlayerSettings.GetScriptingDefineSymbols(buildTarget, out var curSymbols);
            var newSymbols = curSymbols.Union(symbols).Distinct().ToArray();
            PlayerSettings.SetScriptingDefineSymbols(buildTarget, newSymbols);

            $"Added symbols for {buildTarget.TargetName}: {string.Join(",", symbols)}".Log();
        }

        private static void RemoveScriptingDefineSymbol(NamedBuildTarget buildTarget, params string[] symbols)
        {
            PlayerSettings.GetScriptingDefineSymbols(buildTarget, out var curSymbols);
            var newSymbols = curSymbols.Except(symbols).Distinct().ToArray();
            PlayerSettings.SetScriptingDefineSymbols(buildTarget, newSymbols);

            $"Removed symbols for {buildTarget.TargetName}: {string.Join(",", symbols)}".Log();
        }

        private static void SetApplicationIdentifierPostfix(NamedBuildTarget buildTarget, Environment environment)
        {
            var identifier = PlayerSettings.GetApplicationIdentifier(buildTarget);
            var baseIdentifier = PostfixRegex.Replace(identifier, "");
            var postfix = GetPostfix(environment, buildTarget);
            var newIdentifier = $"{baseIdentifier}{postfix}";
            PlayerSettings.SetApplicationIdentifier(buildTarget, newIdentifier);

            $"Set application identifier for {buildTarget.TargetName}: {newIdentifier}".Log();
        }

        private static void SetApplicationNamePostfix(NamedBuildTarget buildTarget, Environment environment)
        {
            var baseProductName = PostfixRegex.Replace(PlayerSettings.productName, "");
            var postfix = GetPostfix(environment, buildTarget);
            var newProductName = $"{baseProductName}{postfix}";

            PlayerSettings.productName = newProductName;

            $"Set application product name for {buildTarget.TargetName}: {newProductName}".Log();
        }

        private static string GetPostfix(Environment environment, NamedBuildTarget buildTarget)
        {
            string postfix;

            switch (environment)
            {
                case Environment.DEVELOPMENT_MOCKED:
                case Environment.DEVELOPMENT:
                    postfix = "dev";
                    break;

                case Environment.STAGING:
                    postfix = "stg";
                    break;

                case Environment.PRODUCTION:
                    return "";

                default:
                    throw new Exception("Not a valid environment!");
            }

            var separator = buildTarget == NamedBuildTarget.Android ? "_" : "-";
            return $"{separator}{postfix}";
        }

        private static void SetAppIcon(NamedBuildTarget buildTarget, Environment environment, Dictionary<Environment,string> env2IconMap)
        {
            var hasPath = env2IconMap.TryGetValue(environment, out var appIconPath);
            if (!hasPath)
            {
                throw new Exception($"Path not found for environment {environment.ToString()}");
            }
            
            $"Setting app icon to {appIconPath}".Log();
            
            var appIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(appIconPath);
            if (appIcon == null)
            {
                throw new Exception($"Did not find app icon at path {appIconPath}");
            }
            
            var appIconsLength = PlayerSettings.GetIconSizes(buildTarget, IconKind.Any).Length;
            var appIcons = new Texture2D[appIconsLength];
            Array.Fill(appIcons, appIcon);

            PlayerSettings.SetIcons(buildTarget, appIcons, IconKind.Any);
        }
    }
}
