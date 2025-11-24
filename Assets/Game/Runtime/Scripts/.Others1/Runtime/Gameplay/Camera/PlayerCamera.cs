using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class PlayerCamera : MonoBehaviour
    {
        private Transform _playerTransform;
        private DiContainer _diContainer;

        [Inject]
        private void Construct(DiContainer diContainer)
        {
            _diContainer = diContainer;
        }

        private void LateUpdate()
        {
            if (_playerTransform == null)
            {
                return;
            }

            transform.position = new Vector3(_playerTransform.position.x, _playerTransform.position.y, transform.position.z);
        }

        private void OnEnable()
        {
            _playerTransform = _diContainer.TryResolve<UnitPlayer>().transform;
        }

        private void OnDisable()
        {
            _playerTransform = null;
        }
    }
}
