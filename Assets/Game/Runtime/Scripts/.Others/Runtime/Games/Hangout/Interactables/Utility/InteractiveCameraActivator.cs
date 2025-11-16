using Cinemachine;
using UnityEngine;

namespace Kumu.Kulitan.Hangout
{
    public class InteractiveCameraActivator : MonoBehaviour
    {
        [SerializeField] private GameObject targetObj;
        [SerializeField] private CinemachineVirtualCameraBase vCam;

        private CinemachineVirtualCameraBase cam;
        
        public void SetCameraActive(bool active)
        {
            cam.gameObject.SetActive(!active);
            vCam.gameObject.SetActive(active);
        }

        private void Awake()
        {
            if (vCam != null)
            {
                return;
            }

            vCam = targetObj.GetComponent<CinemachineVirtualCamera>();

            if (vCam == null && targetObj != null)
            {
                vCam = targetObj.AddComponent<CinemachineVirtualCamera>();
            }
        }

        private void Start()
        {
            if (cam == null)
            {
                // Debug.Assert(cam != null, $"[{name}]: Camera not assigned! Very expensive call!");
                cam = FindObjectOfType<CinemachineFreeLook>();
            }

            targetObj.SetActive(false);
        }
    }
}
