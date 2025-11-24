using UnityEngine;

namespace Santelmo.Rinsurv
{
    public class MockDamagePlayer : MonoBehaviour
    {
        private IHittable _playerHittable;

        private void Start()
        {
            _playerHittable = GetComponent<IHittable>();
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.P))
            {
                _playerHittable.Hit(10, transform.position);
            }
        }
    }
}
