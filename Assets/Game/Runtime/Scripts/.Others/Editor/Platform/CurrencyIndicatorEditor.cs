using UnityEngine;
using UnityEditor;

namespace Kumu.Kulitan.UI
{
    [CustomEditor(typeof(CurrencyIndicator))]
    public class CurrencyIndicatorEditor : Editor
    {
        private int testValue;
        private bool isTesting;

        public override void OnInspectorGUI()
        {

            DrawDefaultInspector();

            isTesting = EditorGUILayout.Toggle("Test Mode", isTesting);

            if (isTesting)
            {
                testValue = EditorGUILayout.IntField(testValue);

                if (GUILayout.Button("Set Value"))
                {
                    var currencyIndicator = (CurrencyIndicator)target;
                    currencyIndicator.SetValue(testValue);
                }

                if (GUILayout.Button("Update Value"))
                {
                    var currencyIndicator = (CurrencyIndicator)target;
                    currencyIndicator.UpdateValue(testValue);
                }

            }
        }
    }
}
