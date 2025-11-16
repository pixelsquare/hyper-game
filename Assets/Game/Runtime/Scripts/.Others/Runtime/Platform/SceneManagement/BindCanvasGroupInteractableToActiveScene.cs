using Kumu.Kulitan.Common;
using UniRx;
using UnityEngine;

namespace Kumu.Kulitan.UI
{
    /// <summary>
    /// Binds the interactable and blocksRaycast property of the attached CanvasGroup to the active state of the scene where it's included.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class BindCanvasGroupInteractableToActiveScene : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        private string sceneName;
        private void Awake()
        {
            sceneName = gameObject.scene.name;
        }

        private void Start()
        {
            SceneLoadingManager.Instance.OnSceneSetToActive.Subscribe(activeScene =>
            {
                canvasGroup.interactable = sceneName == activeScene;
                canvasGroup.blocksRaycasts = sceneName == activeScene;
            }).AddTo(this);
        }
    }
}
