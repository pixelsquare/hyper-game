using Kumu.Kulitan.Common;
using UnityEngine;

namespace Kumu.Kulitan.UI
{
    public class ActiveSceneChanger : MonoBehaviour
    {
        public void ChangeActiveScene(string activeScene)
        {
            SceneLoadingManager.Instance.SetActiveScene(activeScene);
        }
    }
}
