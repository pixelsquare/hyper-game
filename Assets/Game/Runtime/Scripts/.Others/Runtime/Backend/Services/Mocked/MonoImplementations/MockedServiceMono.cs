using UnityEngine;

namespace Kumu.Kulitan.Backend
{
    public class MockedServiceMono : MonoBehaviour
    {
        public static T CreateNewInstance<T>() where T : MockedServiceMono
        {
            var go = new GameObject(typeof(T).ToString());
            DontDestroyOnLoad(go);

            return go.AddComponent<T>();
        }
    }
}
