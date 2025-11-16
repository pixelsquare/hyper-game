using Kumu.Kulitan.Events;
using UnityEngine;

namespace Kumu.Kulitan.Hangout
{
    public class OnPhotoModeStart : Event<string>
    {
        public const string EVENT_NAME = "OnPhotoModeStart";

        public OnPhotoModeStart(Camera photoCamera, Transform pTrans) : base(EVENT_NAME)
        {
            PhotoCamera = photoCamera;
            PlayerTransform = pTrans;
        }

        public Camera PhotoCamera { get; }
        public Transform PlayerTransform { get; }
    }
}
