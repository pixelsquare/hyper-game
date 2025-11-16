using System.Collections.Generic;
using System.Linq;
using Kumu.Extensions;
using UnityEditor;
using UnityEngine;

namespace Kumu.Kulitan.Gifting
{
    [CustomEditor(typeof(VGController))]
    public class VGControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var vgController = target as VGController;
            
            base.OnInspectorGUI();

            if (GUILayout.Button("Find Giftees"))
            {
                vgController.Giftees = FindGiftees().ToArray();
            }
        }
        
        private IEnumerable<VirtualGiftGifteeData> FindGiftees()
        {
            var notifier = GetNotifierInScene();
            if (notifier == null)
            {
                "[MockedVirtualGiftServiceMonoEditor] No active VG notifier in scene, no test request sent".Log();
                return default;
            }

            var recipientList = new List<VirtualGiftGifteeData>();

            foreach (var pKey in notifier.PlayerMapping.Keys)
            {
                var newGiftee = new VirtualGiftGifteeData();
                newGiftee.id = notifier.PlayerMapping[pKey].AccountId;
                newGiftee.nickname = notifier.PlayerMapping[pKey].Nickname;
                newGiftee.username = notifier.PlayerMapping[pKey].Username;
                    
                recipientList.Add(newGiftee);
            }

            return recipientList;
        }

        private VirtualGiftNotificationHandler GetNotifierInScene()
        {
            return FindObjectOfType<VirtualGiftNotificationHandler>();
        }
    }
}
