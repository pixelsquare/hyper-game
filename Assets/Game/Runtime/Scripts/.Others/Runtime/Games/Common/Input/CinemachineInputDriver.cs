using Cinemachine;
using UnityEngine;

namespace Kumu.Kulitan.Common
{
    [RequireComponent(typeof(CinemachineFreeLook))]
    public class CinemachineInputDriver : MonoBehaviour, AxisState.IInputAxisProvider
    {
        [SerializeField] private VectorData lookData;

        public float GetAxisValue(int axis)
        {
            return axis switch
            {
                0 => lookData.Value.x,
                1 => lookData.Value.y,
                _ => 0
            };
        }
        
        private void Awake()
        {
            lookData.Reset();
        }
    }
}
