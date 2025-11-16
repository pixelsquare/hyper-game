using UnityEditor;
using UnityEngine;

namespace Kumu.Kulitan.Hangout
{
    [CustomEditor(typeof(ChatGroupManager))]
    public class ChatGroupManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var targetManager = (ChatGroupManager)target;
            
            DrawDefaultInspector ();

            if (GUILayout.Button("Enable Group"))
            {
                targetManager.ToggleChatGroup(true);
            }
            
            if (GUILayout.Button("Disable Group"))
            {
                targetManager.ToggleChatGroup(false);
            }
        }
    }
}
