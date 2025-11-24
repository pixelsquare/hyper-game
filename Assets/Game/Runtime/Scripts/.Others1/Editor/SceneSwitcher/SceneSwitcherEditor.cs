using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Santelmo.Rinsurv.Editor
{
    [InitializeOnLoad]
    public static class SceneSwitcherEditor
    {
        public enum ToolbarZone
        {
            ToolbarZoneRightAlign,
            ToolbarZoneLeftAlign
        }

        private static ScriptableObject _toolbar;
        private static string[] _scenePaths;
        private static string[] _sceneNames;
        private readonly static ToolbarZone _toolbarZone = ToolbarZone.ToolbarZoneRightAlign;

        static SceneSwitcherEditor()
        {
            EditorApplication.delayCall += () =>
            {
                EditorApplication.update -= Update;
                EditorApplication.update += Update;
            };
        }

        private static void Update()
        {
            if (_toolbar == null)
            {
                var editorAssembly = typeof(UnityEditor.Editor).Assembly;
                var toolbars = Resources.FindObjectsOfTypeAll(editorAssembly.GetType("UnityEditor.Toolbar"));
                _toolbar = toolbars.Length > 0 ? (ScriptableObject)toolbars[0] : null;

                if (_toolbar != null)
                {
                    var root = _toolbar.GetType().GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
                    var rawRoot = root.GetValue(_toolbar);
                    var mRoot = rawRoot as VisualElement;

                    var toolbarZone = mRoot.Q(_toolbarZone.ToString());

                    if (toolbarZone != null)
                    {
                        var parent = new VisualElement
                        {
                            style =
                            {
                                flexGrow = 1,
                                flexDirection = FlexDirection.Row
                            }
                        };

                        var container = new IMGUIContainer();
                        container.onGUIHandler -= OnGUI;
                        container.onGUIHandler += OnGUI;

                        parent.Add(container);
                        toolbarZone.Add(parent);
                    }
                }
            }

            var scenes = EditorBuildSettings.scenes;

            if (_scenePaths == null || _scenePaths.Length != scenes.Length)
            {
                _scenePaths = scenes.Where(x => x != null)
                                    .Where(x => x.path.StartsWith("Assets"))
                                    .Select(x => Application.dataPath + x.path.Substring(6))
                                    .ToArray();
                _sceneNames = _scenePaths.Select(x => Path.GetFileNameWithoutExtension(x))
                                         .ToArray();
            }
        }

        private static void OnGUI()
        {
            using (new EditorGUI.DisabledScope(Application.isPlaying))
            {
                var sceneName = SceneManager.GetActiveScene().name;
                var sceneIndex = Array.FindIndex(_sceneNames,
                    x => string.Compare(x, sceneName, StringComparison.OrdinalIgnoreCase) == 0);

                var newSceneIndex = EditorGUILayout.Popup(sceneIndex, _sceneNames, GUILayout.Width(200f));

                if (sceneIndex == newSceneIndex)
                {
                    return;
                }

                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene(_scenePaths[newSceneIndex], OpenSceneMode.Single);
                }
            }
        }
    }
}
