using UnityEngine;

namespace Santelmo.Rinsurv.Tests
{
    public class BasicVFXTest : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _testVFX;
        
        void Update()
        {
            if (Input.GetKeyUp(KeyCode.Y))
            {
                _testVFX.Stop(true);
                _testVFX.Clear(true);
                _testVFX.Play(true);
            }
        }
    }
}
