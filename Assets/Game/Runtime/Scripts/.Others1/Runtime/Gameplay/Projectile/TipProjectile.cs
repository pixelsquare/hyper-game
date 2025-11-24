using System.Collections.Generic;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    public delegate void OnProjectileExpire(IProjectile projectile);
    
    public class TipProjectile : MonoBehaviour, IProjectile, IWeaponHit, ISpawn
    {
        [SerializeField] private GameObject _visualPivot;
        
        private uint _hits;
        private Vector2 _direction;
        private Vector2 _delta;

        private HashSet<GameObject> _entitiesHit = new();

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
            _entitiesHit.Clear();
        }

        public void OnDespawn()
        {
            gameObject.SetActive(false);
            _visualPivot.transform.localPosition = Vector3.zero;
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
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (_entitiesHit.Contains(col.gameObject))
            {
                return;
            }

            _entitiesHit.Add(col.gameObject);
            OnHit(col.transform);

            if (_entitiesHit.Count == Pierce)
            {
                OnProjectileExpire?.Invoke(this);
            }
        }
    }
}
