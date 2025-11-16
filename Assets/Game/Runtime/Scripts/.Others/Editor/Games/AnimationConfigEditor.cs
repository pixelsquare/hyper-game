using UnityEditor;
using Kumu.Kulitan.Hangout;

namespace Kumu.Kulitan.Common
{
    [CustomEditor(typeof(AnimationConfig))]
    public class AnimationConfigEditor : Editor
    {
        private IInspectorGUI TargetGUI => target as IInspectorGUI;

        public void OnEnable()
        {
            TargetGUI?.OnInspectorInit();
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical();
            DrawDefaultInspector();
            TargetGUI?.OnInspectorDraw();
            EditorGUILayout.EndVertical();
        }
    }
}
