using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kumu.Kulitan.Backend.Editor
{
    public static class ServiceInjector
    {
        [RuntimeInitializeOnLoadMethod]
        public static void Initialize()
        {
            var sceneCount = SceneManager.sceneCount;
            var scene = SceneManager.GetActiveScene();

            if (sceneCount == 1)
            {
                var sceneName = scene.name;

                switch (sceneName)
                {
                    case "AvatarCustomization":
                        Services.ShopService = MockedServiceMono.CreateNewInstance<MockedShopServiceMono>();
                        Services.InventoryService = MockedServiceMono.CreateNewInstance<MockedInventoryServiceMono>();
                        break;

                    case "SocialScreen":
                        Services.SocialService = MockedServiceMono.CreateNewInstance<MockedSocialServiceMono>();
                        break;
                }
            }
        }
    }
}
