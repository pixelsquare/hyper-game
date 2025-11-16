using Kumu.Extensions;
using Kumu.Kulitan.Social;
using UnityEditor;
using UnityEngine;
using Task = System.Threading.Tasks.Task;

namespace Kumu.Kulitan.Backend
{
    [CustomEditor(typeof(MockedSocialServiceMono))]
    public class MockedSocialServiceMonoEditor : UnityEditor.Editor
    {
        private MockedSocialServiceMono serviceMono;

        private string LocalPlayerAccountId => UserProfileLocalDataManager.Instance.GetLocalUserProfile().accountId;

        public override void OnInspectorGUI()
        {
            serviceMono = (MockedSocialServiceMono)target;
            DrawDefaultInspector ();
            
            var style = new GUIStyle(GUI.skin.button);
            style.normal.textColor = Color.green;
            if (GUILayout.Button("Get Socials Test (Friends Only)",style))
            {
                GetSocials();
            }

            if (GUILayout.Button("Create Friends-only Room",style))
            {
                MockedSocialServiceUtil.CreateFriendsOnlyRoom();
            }
            
            if (GUILayout.Button("Join Room Directly",style))
            {
                SocialManager.Instance.JoinRoomDirect(serviceMono.RoomID);
            }
        }
        
        private async void GetSocials()
        {
            var request = new GetSocialRelationshipsRequest()
            {
                userConstraint = "friends"
            };
            var task = Services.SocialService.GetSocialRelationshipsAsync(request);
                
            await Task.Delay(1);

            if (task.IsCanceled)
            {
                "Task was cancelled!".LogError();
                return;
            }

            var result = task.Result;

            if (result.HasError)
            {
                "Get socials failed.".LogError();
                return;
            }

            "Listing Friends Profiles:".Log();
            foreach (var profile in result.Result.OtherPlayerProfiles)
            {
                $"{profile.profile.userName}".Log();
            }
        }
    }
}
