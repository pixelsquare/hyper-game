using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kumu.Extensions;
using Kumu.Kulitan.Gifting;
using UnityEditor;
using UnityEngine;

namespace Kumu.Kulitan.Backend
{
    [CustomEditor(typeof(MockedVirtualGiftServiceMono))]
    public class MockedVirtualGiftServiceMonoEditor : UnityEditor.Editor
    {
        private MockedVirtualGiftServiceMono serviceMono;

        private string gifter;

        private string LocalPlayerAccountId => UserProfileLocalDataManager.Instance.GetLocalUserProfile().accountId;

        public override void OnInspectorGUI()
        {
            serviceMono = (MockedVirtualGiftServiceMono)target;
            DrawDefaultInspector();

            var style = new GUIStyle(GUI.skin.button);
            style.normal.textColor = Color.green;
            if (GUILayout.Button("Send Test Request", style))
            {
                SendGift();
            }

            if (GUILayout.Button(("Fill giftees with players in Room")))
            {
                FillGiftees();
            }
        }

        private async void SendGift()
        {
            var notifier = GetNotifierInScene();
            if (notifier == null)
            {
                "[MockedVirtualGiftServiceMonoEditor] No active VG notifier in scene, no test request sent".Log();
                return;
            }

            if (serviceMono.InternalRequest.giftees == null || serviceMono.InternalRequest.giftees.Length == 0 ||
                serviceMono.OverrideGifteesValue)
            {
                var recipientList = new List<VirtualGiftGifteeData>();

                foreach (var pKey in notifier.PlayerMapping.Keys)
                {
                    var newGiftee = new VirtualGiftGifteeData();
                    newGiftee.id = notifier.PlayerMapping[pKey].AccountId;
                    newGiftee.nickname = notifier.PlayerMapping[pKey].Nickname;
                    newGiftee.username = notifier.PlayerMapping[pKey].Username;

                    recipientList.Add(newGiftee);
                }

                var gifteeIds = recipientList.Select(gifteeData => gifteeData.id).ToArray();
                serviceMono.InternalRequest.giftees = gifteeIds;
            }

            $"[MockedVirtualGiftServiceMonoEditor] Test gift send".Log();
            var result = await Services.VirtualGiftService.SendVirtualGiftAsync(serviceMono.InternalRequest);

            if (result.HasError)
            {
                $"[MockedVirtualGiftServiceMonoEditor] Error gift send - {result.Error}".Log();
                return;
            }

            "[MockedVirtualGiftServiceMonoEditor] Gift sent.".Log();

            await Task.Delay(serviceMono.VirtualGiftNotificationDelayInMilliseconds);

            serviceMono.InvokeVirtualGiftReceivedEvent(serviceMono.InternalRequest,
                serviceMono.OverrideGifterValue ? gifter : LocalPlayerAccountId);
        }

        private void FillGiftees()
        {
            var notifier = GetNotifierInScene();
            if (notifier == null)
            {
                "[MockedVirtualGiftServiceMonoEditor] No active VG notifier in scene, no test request sent".Log();
                return;
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

            var gifteeIds = recipientList.Select(gifteeData => gifteeData.id).ToArray();
            serviceMono.InternalRequest.giftees = gifteeIds;
        }

        private VirtualGiftNotificationHandler GetNotifierInScene()
        {
            return FindObjectOfType<VirtualGiftNotificationHandler>();
        }
    }
}
