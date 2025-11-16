using UnityEngine;
using UnityEditor;

namespace Kumu.Kulitan.UI
{
    [CustomEditor(typeof(CurrencyTweener))]
    public class CurrencyTweenerEditor : Editor
    {
        private bool isTesting;
        private int spawnAmount;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            isTesting = EditorGUILayout.Toggle("Test Mode", isTesting);

            if (isTesting)
            {
                spawnAmount = EditorGUILayout.IntField("Amount", spawnAmount);

                if (GUILayout.Button("Spawn"))
                {
                    var tweener = (CurrencyTweener)target;
                    tweener.SpawnCurrency(spawnAmount);
                }
            }
        }
    }
}