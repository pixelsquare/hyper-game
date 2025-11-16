using Kumu.Extensions;
using Kumu.Kulitan.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Kumu.Kulitan.Hangout
{
    public class InteractiveObjectWorldUI : MonoBehaviour
    {
        [SerializeField] private Canvas myCanvas;
        [SerializeField] private UIFaceMainCamera camFacer;
        [SerializeField] private GameObject buttonUI;
        [SerializeField] private GameObject childElement;
        [SerializeField] private Image buttonImg;
        
        private IInteractiveObject interactiveObject; 
        private bool isVisible;

        public void Initialize(Sprite icon, IInteractiveObject interactive, Camera cam)
        {
            $"has canvas {myCanvas != null}".Log();
            $"has world cam {myCanvas?.worldCamera != null}".Log();
            myCanvas.worldCamera = cam;
            interactiveObject = interactive;
            camFacer.SetCamera(null);
            buttonImg.sprite = icon;
        }

        public void Show()
        {
            isVisible = true;

            if (interactiveObject.IsAvailable())
            {
                childElement.SetActive(true);
                buttonUI.SetActive(true);
            }
        }

        public void Hide()
        {
            isVisible = false;
            childElement.SetActive(false);
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
                childElement.SetActive(interactiveObject.IsAvailable());
                buttonUI.SetActive(interactiveObject.IsAvailable());
            }
        }
    }
}
