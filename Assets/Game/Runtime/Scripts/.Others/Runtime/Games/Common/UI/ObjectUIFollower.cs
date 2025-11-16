using Kumu.Extensions;
using UnityEngine;

namespace Kumu.Kulitan.Common
{
    /// <summary>
    /// Script for UI objects to follow non-UI objects
    /// </summary>
    public class ObjectUIFollower : MonoBehaviour
    {
        [SerializeField] private Camera camera;
        [SerializeField] private Transform objectToFollow;
        [SerializeField] private Vector3 offset;
        [SerializeField] private float baselineCameraXAngle = 46.848f;

        private RectTransform canvasRectTransform;
        private RectTransform thisRectTransform;
        private bool isTargetInitialized;
        private Vector2 wantedPosition;

        public void InitializeTarget(Transform target)
        {
            objectToFollow = target;
            isTargetInitialized = true;
        }

        public void SetCamera(Camera camera)
        {
            this.camera = camera;
        }
        
        private void Awake()
        {
            if (camera == null)
            {
                camera = Camera.main;
            }
            
            var parentCanvas = GetComponentInParent<Canvas>();
            canvasRectTransform = parentCanvas.GetComponent<RectTransform>();
            thisRectTransform = GetComponent<RectTransform>();

            if (objectToFollow != null)
            {
                isTargetInitialized = true;
            }
        }

        private void FixedUpdate()
        {
            if (!isTargetInitialized || objectToFollow == null)
            {
                return;
            }

            var canvasPoint = canvasRectTransform.WorldToCanvasPosition(objectToFollow.position, camera);
            var cameraAngleScale = camera.transform.parent.eulerAngles.x / baselineCameraXAngle;
            
            thisRectTransform.anchoredPosition = (canvasPoint + offset) / cameraAngleScale;
            thisRectTransform.ForceUpdateRectTransforms();
        }
    }
}
