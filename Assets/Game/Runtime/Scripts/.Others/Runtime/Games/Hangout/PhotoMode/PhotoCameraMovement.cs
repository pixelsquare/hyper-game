using System.Collections.Generic;
using Kumu.Extensions;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;
using Lean.Touch;
using UnityEngine;

namespace Kumu.Kulitan.Hangout
{
    public class PhotoCameraMovement : MonoBehaviour
    {
        [SerializeField] private float xMoveFactor;
        [SerializeField] private float yMoveFactor;

        [SerializeField] private float pinchThreshold;
        [SerializeField] private float moveThreshold;
        
        [SerializeField] private float[] extraBounds; //order: top, bottom, front, back, left, right
        
        [SerializeField] private float xRotationSpeed = 50f;
        [SerializeField] private float yRotationSpeed = 50f;
        [SerializeField] private float forwardSpeed = 5f;

        [SerializeField] private List<GameObject> blockingElements;

        private Slot<string> eventSlot;

        private Transform playerTrans; //keeping here in case further functionality is needed that's player-centric e.g. rotate around player

        private float top, bottom, forward, backward, left, right;

        private bool isPinching;
        private bool isPanning;
        private bool isOnUI;

        private Vector3 cameraStartPosition;
        private Quaternion cameraStartRotation;
        
        private bool isNameTagShowing;

        public void ResetCameraPosition()
        {
            var cameraTransform = PhotoModeManager.Instance.PhotoCamera.transform;
            
            cameraTransform.position = cameraStartPosition;
            cameraTransform.rotation = cameraStartRotation;
        }

        public void ToggleNameTag()
        {
            isNameTagShowing = !isNameTagShowing;
            ToggleNameTagVisibility();
        }

        private void ToggleNameTagVisibility()
        {
            PhotoModeManager.Instance.NameTagCamera.enabled = isNameTagShowing;
        }

        private void StartedPhotoMode(IEvent<string> callback)
        {
            var messageObj = (OnPhotoModeStart) callback;
            playerTrans = messageObj.PlayerTransform;

            var cameraTransform = messageObj.PhotoCamera.transform;

            cameraStartPosition = cameraTransform.position;
            cameraStartRotation = cameraTransform.rotation;

            isNameTagShowing = false;
            ToggleNameTagVisibility();
        }
        
        private void StartTouch(LeanFinger obj)
        {
            isPinching = false;
            isPanning = false;

            var results = LeanTouch.RaycastGui(obj.ScreenPosition, -1);

            if (results != null && results.Count > 0)
            {
                foreach (var res in results)
                {
                    if (blockingElements.Contains(res.gameObject))
                    {
                        isOnUI = true;
                    }
                }
            }
        }
        
        private void EndTouch(LeanFinger obj)
        {
            isOnUI = false;
        }

        private void CheckTouch(LeanFinger obj)
        {
            var results = LeanTouch.RaycastGui(obj.ScreenPosition, -1);

            if (results != null && results.Count > 0)
            {
                foreach (var res in results)
                {
                    if (blockingElements.Contains(res.gameObject))
                    {
                        isOnUI = true;
                    }
                }
            }
        }

        private void HandleGesture(List<LeanFinger> fingers)
        {
            if (fingers.Count == 0)
            {
                isOnUI = false;
                return;
            }

            //allow continuation of movement if already started before hitting blocking element
            if (isOnUI && !isPanning && !isPinching)
            {
                return;
            }

            var cameraTransform = PhotoModeManager.Instance.PhotoCamera.transform;

            if (fingers.Count == 1)
            {
                var moveDelta = LeanGesture.GetScreenDelta(fingers);

                cameraTransform.eulerAngles += new Vector3(-moveDelta.y * xRotationSpeed, -moveDelta.x * yRotationSpeed, 0);
            }
            else if (fingers.Count == 2)
            {
                var pinchScale = LeanGesture.GetPinchScale(fingers);
                var pinchTwist = LeanGesture.GetTwistDegrees(fingers);
                var moveDelta = LeanGesture.GetScreenDelta(fingers);
                
                if (Mathf.Abs(pinchScale - 1) > pinchThreshold || isPinching)
                {
                    cameraTransform.localPosition += cameraTransform.forward * (pinchScale - 1f) * forwardSpeed;
                    isPinching = true;
                
                    LimitCameraToBounds(cameraTransform);
                }

                if (moveDelta.magnitude > moveThreshold || isPanning)
                {
                    var currentXMove = -moveDelta.x * xMoveFactor;
                    var currentYMove = -moveDelta.y * yMoveFactor;

                    cameraTransform.position += (cameraTransform.up * currentYMove) + (cameraTransform.right * currentXMove);
                
                    LimitCameraToBounds(cameraTransform);

                    isPanning = true;
                }
            }
        }

        private void LimitCameraToBounds(Transform cameraTransform)
        {
            var xPos = cameraTransform.position.x;
            var yPos = cameraTransform.position.y;
            var zPos = cameraTransform.position.z;

            if (cameraTransform.position.x < left)
            {
                xPos = left;
            }

            if (cameraTransform.position.x > right)
            {
                xPos = right;
            }
            
            if (cameraTransform.position.y < bottom)
            {
                yPos = bottom;
            }

            if (cameraTransform.position.y > top)
            {
                yPos = top;
            }
            
            if (cameraTransform.position.z < backward)
            {
                zPos = backward;
            }

            if (cameraTransform.position.z > forward)
            {
                zPos = forward;
            }

            cameraTransform.position = new Vector3(xPos, yPos, zPos);
        }

        private void ComputeBounds()
        {
            var combinedBounds = new Bounds();
            var colliders = FindObjectsOfType<QuantumStaticBoxCollider3D>();
            
            foreach(var coll in colliders)
            {
                var bc = coll.GetComponent<BoxCollider>();

                if (bc != null)
                {
                    combinedBounds.Encapsulate(bc.bounds);
                }
                else
                {
                    //needed debug to warn for quantum colliders that do not have box colliders
                    $"[PhotoCameraMovement] Quantum collider for {coll.gameObject.name} has no box collider".LogWarning();
                }
            }

            top = combinedBounds.center.y + combinedBounds.extents.y + extraBounds[0];
            bottom = combinedBounds.center.y - combinedBounds.extents.y -  + extraBounds[1];
            
            forward = combinedBounds.center.z + combinedBounds.extents.z + extraBounds[2];
            backward = combinedBounds.center.z - combinedBounds.extents.z - extraBounds[3];
            
            right = combinedBounds.center.x + combinedBounds.extents.x + extraBounds[4];
            left = combinedBounds.center.x - combinedBounds.extents.x - extraBounds[5];
        }
        
        private void Start()
        {
            LeanTouch.OnGesture += HandleGesture;
            LeanTouch.OnFingerDown += StartTouch;
            LeanTouch.OnFingerUpdate += CheckTouch;
            LeanTouch.OnFingerUp += EndTouch;
        }

        private void OnDestroy()
        {
            LeanTouch.OnGesture -= HandleGesture;
            LeanTouch.OnFingerDown -= StartTouch;
            LeanTouch.OnFingerUpdate -= CheckTouch;
            LeanTouch.OnFingerUp -= EndTouch;
        }
        
        private void OnEnable()
        {
            eventSlot = new Slot<string>(GlobalNotifier.Instance);
            eventSlot.SubscribeOn(OnPhotoModeStart.EVENT_NAME, StartedPhotoMode);

            ComputeBounds();
        }

        private void OnDisable()
        {
            eventSlot.Dispose();
        }
    }
}
