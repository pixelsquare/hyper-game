using Kumu.Extensions;
using Kumu.Kulitan.Hangout;
using UnityEditor;
using UnityEngine;

namespace Kumu.Kulitan.EditorTools
{
    public static class SceneInteractableUtility
    {
        [MenuItem("Tools/Ube/Log Interactables")]
        private static void CheckInteractables()
        {
            var views = Object.FindObjectsOfType<InteractiveObjectView>();

            var log = "";

            foreach (var view in views)
            {
                log += $"{view.name}\n";
            }

            log.Log();
            EditorGUIUtility.systemCopyBuffer = log;
        }
    }
}
