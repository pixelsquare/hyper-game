using Cinemachine;
using Kumu.Kulitan.Hangout;
using UnityEngine;
using UnityEngine.UI;

namespace Kumu.Kulitan.Common
{
    public class UIFaceMainCamera : MonoBehaviour
    {
        [SerializeField] private float baseCameraDistance = 21.2132f;
        [SerializeField] private float scalingBumper = 1.5f;
        [SerializeField] private Transform parentTransform;
        [SerializeField] private float yOffset = 1.983f;
        [SerializeField] private float closeOffsetPct = 0.59f;
        [SerializeField] private Image bgImage;

        private Transform mainCameraTrans;
        private Camera mainCamera;
        private PlayerUIHandler handler;
        private bool enforceRightSideUp;

        public Transform ParentCanvasTransform => parentTransform;

        public void SetCamera(Camera newCam, bool isRightSideUp = true)
        {
            enforceRightSideUp = isRightSideUp;
            
            if (newCam != null)
            {
                mainCamera = newCam;
                mainCameraTrans = newCam.transform;
            }
            else
            {
                var brainObj = FindObjectOfType<CinemachineBrain>();

                if (brainObj == null)
                {
                    return;
                }
                
                mainCameraTrans = brainObj.transform;
                mainCamera = mainCameraTrans.GetComponent<Camera>();
            }
        }
        
        private int CountCornersVisible(RectTransform rectTransform)
        {
            var screenBounds = new Rect(0f, 0f, Screen.width, Screen.height);
            var objectCorners = new Vector3[4];
            rectTransform.GetWorldCorners(objectCorners);

            var visibleCorners = 0;
            var tempCorner = Vector3.zero;
            
            for (var i = 0; i < objectCorners.Length; i++)
            {
                tempCorner = mainCamera.WorldToScreenPoint(objectCorners[i]);
                if (screenBounds.Contains(tempCorner))
                {
                    visibleCorners++;
                }
            }
            
            return visibleCorners;
        }

        private bool CheckCenterVisible(RectTransform rectTransform)
        {
            var centerPoint= rectTransform.TransformPoint(rectTransform.rect.center);
         
            var screenBounds = new Rect(0f, 0f, Screen.width, Screen.height);

            var tempCorner = mainCamera.WorldToScreenPoint(centerPoint);
            if (screenBounds.Contains(tempCorner))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void LateUpdate()
        {
            if (mainCameraTrans == null)
            {
                var brainObj = FindObjectOfType<CinemachineBrain>();

                if (brainObj == null)
                {
                    return;
                }
                
                mainCameraTrans = brainObj.transform;
                mainCamera = mainCameraTrans.GetComponent<Camera>();
            }
            
            if (CountCornersVisible(bgImage.rectTransform) == 0 && !CheckCenterVisible(bgImage.rectTransform))
            {
                return;
            }

            if (mainCameraTrans == null || PlayerUIHandler.LocalPlayerCanvasTrans == null)
            {
                return;
            }

            var currentDistance = Vector3.Distance(mainCameraTrans.position, PlayerUIHandler.LocalPlayerCanvasTrans.position);
            var scaling = currentDistance / baseCameraDistance * scalingBumper;

            if (scaling > 1)
            {
                scaling = 1;
            }

            if (enforceRightSideUp)
            {
                transform.LookAt(transform.position + mainCameraTrans.rotation * Vector3.forward, mainCameraTrans.rotation * Vector3.up);
            }
            else
            {
                transform.forward = mainCameraTrans.forward;
            }
            
            parentTransform.localScale = new Vector3(scaling, scaling, scaling);

            var compOffset = yOffset * closeOffsetPct;
            var adjustedOffset = compOffset + ((yOffset - compOffset) * (currentDistance / baseCameraDistance));
            parentTransform.localPosition = new Vector3(0, adjustedOffset, 0);
        }
    }
}
