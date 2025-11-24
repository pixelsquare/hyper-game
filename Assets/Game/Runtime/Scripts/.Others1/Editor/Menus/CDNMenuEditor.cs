using System.IO;
using UnityEditor;
using UnityEngine;

namespace Santelmo.Rinsurv.Editor
{
    public class CDNMenuEditor
    {
        [MenuItem("Santelmo/CDN/Clear Cache", false, 0)]
        public static void ClearCache()
        {
            if (Caching.ClearCache())
            {
                Debug.Log("Clear Cache Successful!");
            }
        }

        [MenuItem("Santelmo/CDN/Open Cache Path", false, 0)]
        public static void OpenAssetBundleCache()
        {
            EditorUtility.RevealInFinder(Directory.GetParent(Caching.GetCacheAt(0).path)?.FullName);
        }
    }
}
