using Kumu.Kulitan.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Kumu.Kulitan.Hangout
{
    public class InteractiveObjectUI : MonoBehaviour
    {
        [SerializeField] private ObjectUIFollower uiFollower;
        [SerializeField] private GameObject buttonUI;
        [SerializeField] private Image buttonImg;
        
        private IInteractiveObject interactiveObject; 
        private bool isVisible;

        public void Initialize(Sprite icon, IInteractiveObject interactive, Camera cam)
        {
            interactiveObject = interactive;
            uiFollower.InitializeTarget(interactive.Transform);
            uiFollower.SetCamera(cam);
            buttonImg.sprite = icon;
        }

        public void Show()
        {
            // TODO: Add animations/sounds here
            isVisible = true;
            if (interactiveObject.IsAvailable())
            {
                buttonUI.SetActive(true);
            }
        }

        public void Hide()
        {
            // TODO: Add animation/sounds here
            isVisible = false;
            buttonUI.SetActive(false);
        }

        public void OnButtonClicked()
        {
            interactiveObject.OnTryInteract();
        }

        private void Update()
        {
            if (isVisible)
            {
                buttonUI.SetActive(interactiveObject.IsAvailable());
            }
        }
    }
}
