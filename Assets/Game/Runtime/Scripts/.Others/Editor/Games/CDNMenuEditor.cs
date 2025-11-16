using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Kumu.Extensions;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Build.DataBuilders;

namespace Kumu.Kulitan.Common
{
    /// <summary>
    /// The purpose of this editor class is
    /// to setup addressables auto-magically.
    /// Mostly used for testing and will be removing it
    /// until the addressables is fully integrated in the game.
    /// </summary>
    public static class CDNMenuEditor
    {
        [MenuItem("Tools/CDN/Cache/Clear Cache", false, 1)]
        public static void ClearCache()
        {
            Caching.ClearCache();
        }

        [MenuItem("Tools/CDN/Cache/Open Cache Path", false, 1)]
        public static void OpenAssetBundleCache()
        {
            EditorUtility.RevealInFinder(Directory.GetParent(Caching.GetCacheAt(0).path).FullName);
        }

        private static void SetScriptingDefineSymbol(string scriptingDefineSymbol, bool allPlatforms = false)
        {
            if (allPlatforms)
            {
                var buildTargetNames = Enum.GetNames(typeof(BuildTarget));
                for (int i = 0; i < buildTargetNames.Length; i++)
                {
                    if (Enum.TryParse<BuildTarget>(buildTargetNames[i], true, out var buildTarget))
                    {
                        var targetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);

                        if (BuildPipeline.IsBuildTargetSupported(targetGroup, buildTarget))
                        {
                            PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup, out var scriptingDefines);

                            if (!Array.Exists(scriptingDefines, s => s.Equals(scriptingDefineSymbol)))
                            {
                                var definesLen = scriptingDefines.Length;
                                Array.Resize(ref scriptingDefines, definesLen + 1);
                                scriptingDefines[definesLen] = scriptingDefineSymbol;
                                PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, scriptingDefines);
                            }
                        }
                    }
                }
            }
            else
            {
                var targetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
                PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup, out var scriptingDefines);

                if (!Array.Exists(scriptingDefines, s => s.Equals(scriptingDefineSymbol)))
                {
                    var definesLen = scriptingDefines.Length;
                    Array.Resize(ref scriptingDefines, definesLen + 1);
                    scriptingDefines[definesLen] = scriptingDefineSymbol;
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, scriptingDefines);
                }
            }
        }

        [MenuItem("Tools/CDN/Utils/Export Scene Dependencies")]
        public static void ExportSceneDependencies()
        {
            var depsPath = Path.Combine(Directory.GetCurrentDirectory(), "deps", "scenes");

            if (Directory.Exists(depsPath))
            {
                Directory.Delete(depsPath, true);
            }

            Directory.CreateDirectory(depsPath);

            var sb = new StringBuilder();
            var scenes = EditorBuildSettings.scenes;

            var i = 0;
            var sceneLen = scenes.Length;

            foreach (var scene in scenes)
            {
                // if (scene.enabled)
                {
                    var totalSize = 0L;

                    sb.Clear();
                    sb.AppendLine();

                    var depsList = new List<Tuple<long, string>>();
                    var dependencies = AssetDatabase.GetDependencies(scene.path, true);

                    foreach (var deps in dependencies)
                    {
                        var fileInfo = new FileInfo(deps);
                        var fileSize = fileInfo.Length;
                        depsList.Add(Tuple.Create<long, string>(fileSize, deps));
                        totalSize += fileSize;
                    }

                    depsList.Sort((x, y) => y.Item1.CompareTo(x.Item1));

                    foreach (var deps in depsList)
                    {
                        sb.AppendLine(
                            $"{deps.Item1.ToSizeString().PadRight(10)} {deps.Item1.ToString().PadRight(10)} {deps.Item2}");
                    }

                    sb.Insert(0,
                        $"{Path.GetFileNameWithoutExtension(scene.path)} - {totalSize.ToSizeString()} {totalSize}");

                    var sceneDepsPath = Path.Combine(depsPath,
                        $"{i + 1}_{Path.GetFileNameWithoutExtension(scene.path)}_{totalSize.ToSizeString()}.txt");
                    File.WriteAllText(sceneDepsPath, sb.ToString());

                    if (EditorUtility.DisplayCancelableProgressBar("Analyzing Scene Dependencies",
                            $"Scene dependencies ... {Path.GetFileNameWithoutExtension(scene.path)}",
                            (float)i / (float)sceneLen))
                    {
                        break;
                    }

                    i++;
                }
            }

            EditorUtility.ClearProgressBar();
            EditorUtility.RevealInFinder(depsPath);
        }

        [MenuItem("Tools/CDN/Utils/Print Dependency Size", false, 999)]
        public static void PrintDependencySize()
        {
            var totalSize = 0L;

            var depsList = new List<Tuple<long, string>>();
            var targetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            var dependencies = AssetDatabase.GetDependencies(targetPath, true).Where(a => !a.EndsWith(".cs"));

            foreach (var deps in dependencies)
            {
                var fileInfo = new FileInfo(deps);
                var fileSize = fileInfo.Length;
                depsList.Add(Tuple.Create<long, string>(fileSize, deps));
                totalSize += fileSize;
            }

            depsList.Sort((x, y) => y.Item1.CompareTo(x.Item1));

            foreach (var deps in depsList)
            {
                Debug.Log(
                    $"{deps.Item1.ToSizeString().PadRight(10)} {deps.Item1.ToSizeString().PadRight(10)} {deps.Item2}");
            }

            EditorUtility.DisplayDialog("Print Dependency Size",
                $"{Path.GetFileNameWithoutExtension(targetPath)}\nTotal Size\n{totalSize.ToSizeString()}\n{totalSize}",
                "Ok");
            Debug.Log($"TOTAL SIZE: {totalSize.ToSizeString()} {totalSize}");
        }

        [MenuItem("Tools/CDN/Utils/Export Quantum Asset Dependencies")]
        public static void ExportQuantumAssetDependencies()
        {
            var depsPath = Path.Combine(Directory.GetCurrentDirectory(), "deps", "quantum_assets");

            if (Directory.Exists(depsPath))
            {
                Directory.Delete(depsPath, true);
            }

            Directory.CreateDirectory(depsPath);

            var sb = new StringBuilder();
            var assets = UnityDB.AssetResources;

            var i = 0;
            var assetsLen = ((IList<Quantum.AssetResource>)assets).Count;

            foreach (var asset in assets)
            {
                var totalSize = 0L;

                sb.Clear();
                sb.AppendLine();

                var depsList = new List<Tuple<long, string>>();
                var assetPath = Path.Combine("Assets", $"{asset.Path}.asset");
                var dependencies = AssetDatabase.GetDependencies(assetPath, true);

                foreach (var deps in dependencies)
                {
                    var fileInfo = new FileInfo(deps);
                    var fileSize = fileInfo.Length;
                    depsList.Add(Tuple.Create<long, string>(fileSize, deps));
                    totalSize += fileSize;
                }

                depsList.Sort((x, y) => y.Item1.CompareTo(x.Item1));

                foreach (var deps in depsList)
                {
                    sb.AppendLine(
                        $"{deps.Item1.ToSizeString().PadRight(10)} {deps.Item1.ToString().PadRight(10)} {deps.Item2}");
                }

                sb.Insert(0, $"{Path.GetFileNameWithoutExtension(assetPath)} - {totalSize.ToSizeString()} {totalSize}");

                var sceneDepsPath = Path.Combine(depsPath,
                    $"{i + 1}_{Path.GetFileNameWithoutExtension(assetPath)}_{totalSize.ToSizeString()}.txt");
                File.WriteAllText(sceneDepsPath, sb.ToString());

                if (EditorUtility.DisplayCancelableProgressBar("Analyzing Asset Dependencies",
                        $"Asset dependencies ... {Path.GetFileNameWithoutExtension(assetPath)}",
                        (float)i / (float)assetsLen))
                {
                    break;
                }

                i++;
            }

            EditorUtility.ClearProgressBar();
            EditorUtility.RevealInFinder(depsPath);
        }

        private static void BuildAssetBundles()
        {
            var addressablesSettings = AddressableAssetSettingsDefaultObject.Settings;
            var dataBuilders = addressablesSettings.DataBuilders;

            // Clean addressables
            AddressableAssetSettings.CleanPlayerContent(null);

            // Clean build files (should be removed when updating content)
            var buildPath = addressablesSettings.RemoteCatalogBuildPath.GetValue(addressablesSettings);

            if (Directory.Exists(buildPath))
            {
                Directory.Delete(buildPath, true);
            }

            // Build addressables.
            addressablesSettings.ActivePlayerDataBuilderIndex =
                dataBuilders.IndexOf(dataBuilders.Find(s => s.GetType() == typeof(BuildScriptPackedMode)));
            AddressableAssetSettings.BuildPlayerContent();
        }
    }
}
