using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    public class MultiframeProjectile : MonoBehaviour, IProjectile, IWeaponHit, ISpawn
    {
        [SerializeField] private GameObject _visualPivot;
        
        private uint _hits;
        private Vector2 _direction;
        private Vector2 _delta;

        private bool _hitEnabled = true;
        private float _hitInterval = 0.25f;

        private HashSet<GameObject> _entitiesColliding = new();

        public float Speed { get; set; }
        public float Distance { get; set; }
        public uint Damage { get; set; }
        public uint Pierce { get; set; } = 1;

        public event OnWeaponHit OnWeaponHit;
        public event OnProjectileExpire OnProjectileExpire;

        public Vector2 Direction
        {
            get => _direction;
            set
            {
                transform.up = value;
                _direction = value;
            }
        }

        public void OnHit(Transform hitTransform)
        {
            if (hitTransform.TryGetComponent<IHittable>(out var hittable)
                && hittable.IsHittable)
            {
                hittable.Hit(Damage, transform.position);
                OnWeaponHit?.Invoke(hitTransform);
            }
        }

        public void OnSpawn()
        {
            gameObject.SetActive(true);
            _visualPivot.transform.position += Vector3.up;
            StartCoroutine(HitEntities());
        }

        public void OnDespawn()
        {
            gameObject.SetActive(false);
            _visualPivot.transform.localPosition = Vector3.zero;
            _entitiesColliding.Clear();
        }

        private void Update()
        {
            _delta = _direction * (Speed * Time.deltaTime);
            transform.position += (Vector3) _delta;

            if (Distance > 0)
            {
                Distance -= _delta.magnitude;
            }
            else
            {
                OnProjectileExpire?.Invoke(this);
            }

            if (!_hitEnabled)
            {
                return;
            }

            foreach (var entity in _entitiesColliding)
            {
                OnHit(entity.transform);
            }
        }

        private IEnumerator HitEntities()
        {
            _hitEnabled = false;
            yield return new WaitForSeconds(_hitInterval);
            _hitEnabled = true;
            HitEntities();
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            _entitiesColliding.Add(col.gameObject);
            OnHit(col.transform);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (_entitiesColliding.Contains(other.gameObject))
            {
                _entitiesColliding.Remove(other.gameObject);
            }
        }
    }
}
