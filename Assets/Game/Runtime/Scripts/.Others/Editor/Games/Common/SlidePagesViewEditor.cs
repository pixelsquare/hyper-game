using System;
using UnityEngine;
using UnityEditor;


namespace Kumu.Kulitan.Common
{
    [CustomEditor(typeof(SlidePagesView))]
    public class SlidePagesViewEditor : Editor
    {
        private SlidePagesCollection pages;

        private SlidePagesViewButtonLabels labels;

        private void Awake()
        {
            labels = new SlidePagesViewButtonLabels("next", "prev", "done", "skip");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            using (new EditorGUI.DisabledScope(!EditorApplication.isPlaying))
            {
                EditorGUILayout.Separator();
                EditorGUILayout.LabelField("Set data input", EditorStyles.boldLabel);

                var slidePagesView = (SlidePagesView) target;
            
                pages = (SlidePagesCollection) EditorGUILayout.ObjectField("SlidePagesCollection", pages, typeof(SlidePagesCollection), true, null);

                labels.nextLabel = EditorGUILayout.TextField("nextLabel", labels.nextLabel);
                labels.prevLabel = EditorGUILayout.TextField("prevLabel", labels.prevLabel);
                labels.doneLabel = EditorGUILayout.TextField("doneLabel", labels.doneLabel);
                labels.skipLabel = EditorGUILayout.TextField("skipLabel", labels.skipLabel);

                if (GUILayout.Button("Set page data"))
                {
                    slidePagesView.SetData(pages, labels);
                    UnityEngine.Debug.Log($"\nlabels - done:{slidePagesView.ButtonLabels.doneLabel}, next:{slidePagesView.ButtonLabels.nextLabel}, prev:{slidePagesView.ButtonLabels.prevLabel}, skip:{slidePagesView.ButtonLabels.skipLabel}\nobject:{pages.name}");
                }
            }
        }
    }
}
