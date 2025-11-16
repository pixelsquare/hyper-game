using UnityEngine;

namespace Kumu.Kulitan.Multiplayer
{
    public class CustomPlayerInitialData
    {
        private bool hasCustomInitTransform = false;
        private Vector3 customInitPosition = Vector3.zero;

        public bool HasCustomInitTransform
        {
            get => hasCustomInitTransform;
            set => hasCustomInitTransform = value;
        }

        public Vector3 CustomInitPosition
        {
            get => customInitPosition;
            set => customInitPosition = value;
        }

        public void Reset()
        {
            hasCustomInitTransform = false;
            customInitPosition = Vector3.zero;
        }
    }
}
