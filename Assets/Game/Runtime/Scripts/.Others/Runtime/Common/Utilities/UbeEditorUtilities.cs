using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kumu.Kulitan.EditorTools
{
    public static class UbeEditorUtilities
    {
        public static IEnumerable<T> FindAllObjectsOfTypeInScene<T>(Scene scene)
            where T : MonoBehaviour
        {
            return scene.GetRootGameObjects().SelectMany(obj => obj.GetComponentsInChildren<T>(true));
        }
    }
}
