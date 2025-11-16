using System;
using UnityEngine;
using UnityEditor;

namespace Kumu.Kulitan.Common
{
    [CustomEditor(typeof(AddressablePrefabLoader))]
    public class AddressablePrefabLoaderEditor : Editor
    {
        private AddressablePrefabLoader Loader => (AddressablePrefabLoader) target;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            Button("Load Addressables", LoadAddressables);
            Button("Clear Cache", ClearCache);
        }

        private void Button(string label, Action action)
        {
            if (GUILayout.Button(label))
            {
                action.Invoke();
            }
        }

        private void ClearCache()
        {
            AddressablePrefabLoader.Release();
        }

        private void LoadAddressables()
        {
            Loader.LoadAddressables();
        }
    }
}
