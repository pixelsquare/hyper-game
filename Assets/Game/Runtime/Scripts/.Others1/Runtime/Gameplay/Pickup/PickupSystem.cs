using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class PickupSystem : MonoBehaviour
    {
        [SerializeField] private float _speed = 1f;

        public delegate void OnPickup(GameObject pickupObject);
        public event OnPickup OnPickupEvent;

        private List<Pickup> _pickups;
        private List<Pickup> _magnet;
        private Stack<Pickup> _consume;

        private LazyInject<PickupField> _pickupField;

        [Inject]
        private void Construct(LazyInject<PickupField> pickupField)
        {
            _pickupField = pickupField;
        }

        private void Approach(Vector3 destination)
        {
            foreach (var pickup in new List<Pickup>(_pickups))
            {
                var origin = pickup.transform.position;
                var distance = Vector2.Distance(origin, destination);

                if (distance < _pickupField.Value.Range)
                {
                    _magnet.Add(pickup);
                    _pickups.Remove(pickup);
                }
            }
        }

        private void Attract(Vector3 destination)
        {
            foreach (var pickup in _magnet)
            {
                var origin = pickup.transform.position;
                var distance = Vector2.Distance(origin, destination);

                if (distance < _pickupField.Value.Radius)
                {
                    _consume.Push(pickup);
                }

                var direction = (destination - origin).normalized * (Time.deltaTime * _speed);
                pickup.transform.position += direction;
            }
        }

        private void Consume()
        {
            while (_consume.TryPeek(out var pickup))
            {
                OnPickupEvent?.Invoke(pickup.gameObject);
                _consume.Pop();
                _magnet.Remove(pickup);
                pickup.DeInit();
                pickup.OnObjectSpawn?.Invoke(pickup);

                if (pickup.isActiveAndEnabled)
                {
                    Destroy(pickup.gameObject);
                }
            }
        }

        private void TrackPickup(Pickup pickup)
        {
            _pickups.Add(pickup);
        }

        private void UntrackPickup(Pickup pickup)
        {
            _pickups.Remove(pickup);
        }

        private void Update()
        {
            var destination = _pickupField.Value.transform.position;

            Approach(destination);
            Attract(destination);
            Consume();
        }

        private void Awake()
        {
            _pickups = new List<Pickup>();
            _magnet = new List<Pickup>();
            _consume = new Stack<Pickup>();
        }

        private void OnEnable()
        {
            Pickup._onSpawn += TrackPickup;
            Pickup._onDespawn += UntrackPickup;
        }

        private void OnDisable()
        {
            Pickup._onSpawn -= TrackPickup;
            Pickup._onDespawn -= UntrackPickup;
        }

        private void OnDrawGizmosSelected()
        {
            if (!_pickupField?.Value)
            {
                return;
            }

            var destination = _pickupField.Value.transform.position;
            var range = _pickupField.Value.Range;
            var radius = _pickupField.Value.Radius;

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(destination, range);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(destination, radius);

            foreach (var pickup in _pickups)
            {
                var origin = pickup.transform.position;
                var distance = Vector2.Distance(origin, destination);

                Gizmos.color = Color.magenta;
                if (distance < range)
                {
                    Gizmos.color = Color.cyan;
                }
                if (distance < radius)
                {
                    Gizmos.color = Color.blue;
                }

                Gizmos.DrawSphere(origin, .25f);
                Gizmos.DrawLine(origin, destination);
            }
        }
    }
}
