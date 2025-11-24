using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Santelmo.Rinsurv.Editor
{
    [CreateAssetMenu(fileName = "Directory", menuName = "Santelmo/ArtTools/DirectoryGenerator/Directory")]
    public class DirectoryScriptableObject : ScriptableObject
    {
        public ArtToolDirectory directory;
    }

    [System.Serializable]
    public class ArtToolDirectory
    {
        public string directoryName;
        public List<ArtToolDirectory> subDirectories = new List<ArtToolDirectory>();

        public void CreateDirectory(string rootFolder)
        {
            AssetDatabase.CreateFolder(rootFolder, directoryName);
            string path = $"{rootFolder}/{directoryName}";

            AssetDatabase.Refresh();

            foreach (ArtToolDirectory subDirectory in subDirectories)
            {
                subDirectory.CreateDirectory(path);
            }

            AssetDatabase.Refresh();
        }
    }
}
