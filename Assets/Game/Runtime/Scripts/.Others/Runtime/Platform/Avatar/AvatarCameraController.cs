using System.Collections.Generic;
using UnityEngine;

namespace Kumu.Kulitan.Avatar
{
    /// <summary>
    /// Controls the avatar camera view.
    /// Must be attached to a Camera.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class AvatarCameraController : MonoBehaviour
    {
        [SerializeField] private AvatarCameraViewType currentView = AvatarCameraViewType.FULL;
        [SerializeField] private List<Transform> cameraTransform = new List<Transform>();
        private int testIdx = 0;
        public AvatarCameraViewType CurrentView { get => currentView; }

        // TODO: Remove when no longer needed for testing.
        /// <summary>
        /// A test method that may be called to cycle through all view types.
        /// </summary>
        public void TestCycleView()
        {
            testIdx++;
            if (testIdx > 2)
            {
                testIdx = 0;
            }
            switch (testIdx)
            {
                case 0:
                    SwitchView(AvatarCameraViewType.FULL);
                    break;
                case 1:
                    SwitchView(AvatarCameraViewType.BUST);
                    break;
                case 2:
                    SwitchView(AvatarCameraViewType.FEET);
                    break;
            }
        }

        /// <summary>
        /// Switches the current view of the Avatar Camera.
        /// </summary>
        /// <param name="_view">The view type to switch to</param>
        public void SwitchView(AvatarCameraViewType _view)
        {
            currentView = _view;
            RefreshCameraPosition();
        }

        private void RefreshCameraPosition()
        {
            int idx = (int)currentView;
            for (int i = 0; i < cameraTransform.Count; i++)
            {
                if (i == idx)
                {
                    transform.SetPositionAndRotation(cameraTransform[i].position, cameraTransform[i].rotation);
                }
            }
        }

        private void Start()
        {
            SwitchView(currentView);
        }

        private void Reset()
        {
            SwitchView(currentView);
        }
    }
}
