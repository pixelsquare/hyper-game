using UnityEngine;

namespace Santelmo.Rinsurv
{
    public class FpsSetter : MonoBehaviour
    {
        [SerializeField] private int _targetFps = -1;
        
        private void Start()
        {
            Application.targetFrameRate = _targetFps;
        }
    }
}
