using Kumu.Kulitan.Multiplayer;
using UnityEngine;

namespace Kumu.Kulitan.Backend
{
    public class MockedSocialServiceUtil : MonoBehaviour
    {
        private static HangoutMatchmakingHandler handler;
        
        public static void CreateFriendsOnlyRoom()
        {
            if (handler == null)
            {
                handler = GameObject.FindObjectOfType<HangoutMatchmakingHandler>();
            }
            
            if (handler != null)
            {
                handler.CreateRandomFriendsOnlyRoom();
            }
        }
        
        public static MockedSocialServiceUtil CreateNewInstance()
        {
            var go = new GameObject(nameof(MockedSocialServiceUtil));
            DontDestroyOnLoad(go);
            return go.AddComponent<MockedSocialServiceUtil>();
        }
    }
}
