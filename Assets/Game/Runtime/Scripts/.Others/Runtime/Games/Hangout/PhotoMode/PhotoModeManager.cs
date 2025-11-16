using Cinemachine;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Tracking;
using UnityEngine;
using UnityEngine.UI;

namespace Kumu.Kulitan.Hangout
{
    public class PhotoModeManager : MonoBehaviour
    {
        [SerializeField] private Camera photoCamera;
        [SerializeField] private Camera nameTagCamera;
        [SerializeField] private Camera gameCamera;
        [SerializeField] private Camera uiCamera;
        [SerializeField] private Image watermark;
        [SerializeField] private CinemachineFreeLook freeLookDetails;
        [SerializeField] private ChatGroupManager chatManager;
        [SerializeField] private PhotoModeTutorial tutorial;
        [SerializeField] private PhotoFunction photoTaker;
        [SerializeField] private GameObject[] aspectRatioHighlighters;
        [SerializeField] private RectTransform topBlocker;
        [SerializeField] private RectTransform bottomBlocker;
        [SerializeField] private float[] watermarkSizes;
        [SerializeField] private GameObject photoModeUI;
        [SerializeField] private GameObject gameUI;

        public Camera PhotoCamera => photoCamera;
        public Camera NameTagCamera => nameTagCamera;
        public GameObject WatermarkObject => watermark.gameObject;
        public GameObject PhotoModeUI => photoModeUI;
        public GameObject GameUI => gameUI;

        public static PhotoModeManager Instance { get; private set; }
        
        public const string TutorialPrefsKey = "PHOTO_MODE_TUTORIAL";
        
        private Canvas canvas;

        public void SelectAspectRatio(int selected)
        {
            if (canvas == null)
            {
                canvas = watermark.gameObject.GetComponentInParent<Canvas>();
            }
            
            for (int i=0; i<aspectRatioHighlighters.Length; i++)
            {
                if (i == selected)
                {
                    aspectRatioHighlighters[i].SetActive(true);
                }
                else
                {
                    aspectRatioHighlighters[i].SetActive(false);
                }
            }

            var selectedRatio = (PhotoFunction.AspectRatio) selected;

            photoTaker.SetAspectRatio(selectedRatio);
            
            topBlocker.gameObject.SetActive(true);
            bottomBlocker.gameObject.SetActive(true);

            watermark.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, watermarkSizes[selected]);
            watermark.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, watermarkSizes[selected]);
            
            var finalWidth = Screen.width / canvas.scaleFactor;
            var finalHeight = Screen.height / canvas.scaleFactor;
            var gapFromBottom = 0f;
            var adjustedPosition = 0f;

            switch (selectedRatio)
            {
                case PhotoFunction.AspectRatio.Portrait:
                    adjustedPosition = finalHeight;
                    
                    topBlocker.SetBottom(adjustedPosition);
                    bottomBlocker.SetTop(adjustedPosition);
                    
                    gapFromBottom = 0.025f * finalHeight;
                    
                    watermark.rectTransform.anchoredPosition = new Vector2((finalWidth / 2f) - (watermarkSizes[selected] / 2f) - gapFromBottom, 
                        (finalHeight / -2f) + (watermarkSizes[selected] / 2f) + gapFromBottom);
                    break;
                
                case PhotoFunction.AspectRatio.Landscape:
                    finalHeight = finalWidth * photoTaker.LandscapeAspectRatio.y / photoTaker.LandscapeAspectRatio.x;

                    adjustedPosition = (((Screen.height / canvas.scaleFactor) - finalHeight) / 2f) + finalHeight;
                    topBlocker.SetBottom(adjustedPosition);
                    bottomBlocker.SetTop(adjustedPosition);
                    
                    gapFromBottom = 0.025f * finalHeight;
                    watermark.rectTransform.anchoredPosition = new Vector2((finalWidth / 2f) - (watermarkSizes[selected] / 2f) - gapFromBottom, 
                        (finalHeight / -2f) + (watermarkSizes[selected] / 2f) + gapFromBottom);
                    break;
                
                case PhotoFunction.AspectRatio.Square:
                    finalHeight = finalWidth;
                    
                    adjustedPosition = (((Screen.height / canvas.scaleFactor) - finalHeight) / 2f) + finalHeight;
                    topBlocker.SetBottom(adjustedPosition);
                    bottomBlocker.SetTop(adjustedPosition);
                    
                    gapFromBottom = 0.02f * finalHeight;
                    watermark.rectTransform.anchoredPosition = new Vector2((finalWidth / 2f) - (watermarkSizes[selected] / 2f) - gapFromBottom, 
                        (finalHeight / -2f) + (watermarkSizes[selected] / 2f) + gapFromBottom);
                    break;
            }
        }

        public void StartPhotoMode()
        {
            SelectAspectRatio(0);
            photoCamera.transform.position = gameCamera.transform.position;
            photoCamera.transform.rotation = gameCamera.transform.rotation;
            photoCamera.fieldOfView = gameCamera.fieldOfView;

            nameTagCamera.fieldOfView = gameCamera.fieldOfView;

            photoCamera.enabled = true;
            gameCamera.enabled = false;

            var allFollowers = GameObject.FindObjectsOfType<ObjectUIFollower>();
            foreach (var follower in allFollowers)
            {
                follower.SetCamera(photoCamera);
            }

            var allNameTags = GameObject.FindObjectsOfType<UIFaceMainCamera>();
            foreach (var tag in allNameTags)
            {
                tag.SetCamera(photoCamera, false);
            }

            chatManager.ForceEndPosition();
            chatManager.ToggleChatGroup(false);

            GlobalNotifier.Instance.Trigger(new OnPhotoModeStart(photoCamera, freeLookDetails.Follow));

            if (!PlayerPrefs.HasKey(TutorialPrefsKey))
            {
                tutorial.StartTutorial();
            }
        }

        public void EndPhotoMode()
        {
            photoCamera.enabled = false;
            gameCamera.enabled = true;

            var allFollowers = GameObject.FindObjectsOfType<ObjectUIFollower>();
            foreach (var follower in allFollowers)
            {
                follower.SetCamera(uiCamera);
            }
            
            var allNameTags = GameObject.FindObjectsOfType<UIFaceMainCamera>();
            foreach (var tag in allNameTags)
            {
                tag.SetCamera(null);
            }
            
            GlobalNotifier.Instance.Trigger(PhotoModeEvent.END_ID);
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }

            Instance = this;
        }
    }
}
